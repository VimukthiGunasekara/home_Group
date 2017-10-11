using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvancedTCP.Shared.Enums;
using AdvancedTCP.Shared.Messages;

namespace AdvancedTCP.Server
{
    public class Receiver
    {
        private Thread receivingThread;
        private Thread sendingThread;

        #region Properties

        /// <summary>
        /// The receiver unique id.
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// The reference to the parent Server.
        /// </summary>
        public Server Server { get; set; }
        /// <summary>
        /// The real TcpClient working in the background.
        /// </summary>
        public TcpClient Client { get; set; }
        /// <summary>
        /// Contains a reference to the currently in session with this receiver instance is exists.
        /// </summary>
        public Receiver OtherSideReceiver { get; set; }
        /// <summary>
        /// The current status of the reciever instance.
        /// </summary>
        public StatusEnum Status { get; set; }
        /// <summary>
        /// The message queue that contains all the messages to deliver to the remote client.
        /// </summary>
        public List<MessageBase> MessageQueue { get; private set; }
        /// <summary>
        /// The Total bytes processed by this receiver instance.
        /// </summary>
        public long TotalBytesUsage { get; set; }
        /// <summary>
        /// The Email address is used to authenticate the remote client.
        /// </summary>
        public String Email { get; set; }
        /// <summary>
        /// If true will produce and exception in some cases.
        /// </summary>
        public bool DebugMode { get; set; }
        
        #endregion

        #region Constructors

        public Receiver()
        {
            ID = Guid.NewGuid();
            MessageQueue = new List<MessageBase>();
            Status = StatusEnum.Connected;
        }

        /// <summary>
        /// Initializes a new reciever instance
        /// </summary>
        /// <param name="client">The TcpClient to encapsulate that was obtained by the TcpListener.</param>
        /// <param name="server">The reference to the parent server containing the receivers list.</param>
        public Receiver(TcpClient client, Server server)
            : this()
        {
            Server = server;
            Client = client;
            Client.ReceiveBufferSize = 1024;
            Client.SendBufferSize = 1024;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Initializes the receiver and start transmitting data
        /// </summary>
        public void Start()
        {
            receivingThread = new Thread(ReceivingMethod);
            receivingThread.IsBackground = true;
            receivingThread.Start();

            sendingThread = new Thread(SendingMethod);
            sendingThread.IsBackground = true;
            sendingThread.Start();
        }

        /// <summary>
        /// Stops all data transmition and disconnectes the TcpClient
        /// </summary>
        private void Disconnect()
        {
            if (Status == StatusEnum.Disconnected) return;

            if (OtherSideReceiver != null)
            {
                OtherSideReceiver.OtherSideReceiver = null;
                OtherSideReceiver.Status = StatusEnum.Validated;
                OtherSideReceiver = null;
            }

            Status = StatusEnum.Disconnected;
            Client.Client.Disconnect(false);
            Client.Close();
        }

        /// <summary>
        /// Add the specified message to the message sender queue
        /// </summary>
        /// <param name="message">The message of type MessageBase that should be added to the queue</param>
        public void SendMessage(MessageBase message)
        {
            MessageQueue.Add(message);
        }

        #endregion

        #region Threads Methods

        private void SendingMethod()
        {
            while (Status != StatusEnum.Disconnected)
            {
                if (MessageQueue.Count > 0)
                {
                    var message = MessageQueue[0];

                    try
                    {
                        BinaryFormatter f = new BinaryFormatter();
                        f.Serialize(Client.GetStream(), message);
                    }
                    catch
                    {
                        Disconnect();
                    }
                    finally
                    {
                        MessageQueue.Remove(message);
                    }
                }
                Thread.Sleep(30);
            }
        }

        private void ReceivingMethod()
        {
            while (Status != StatusEnum.Disconnected)
            {
                if (Client.Available > 0)
                {
                    TotalBytesUsage += Client.Available;

                    try
                    {
                        BinaryFormatter f = new BinaryFormatter();
                        MessageBase msg = f.Deserialize(Client.GetStream()) as MessageBase;
                        OnMessageReceived(msg);
                    }
                    catch (Exception e)
                    {
                        if (DebugMode) throw e;
                        Exception ex = new Exception("Unknown message recieved. Could not deserialize the stream.", e);
                        Debug.WriteLine(ex.Message);
                    }
                }

                Thread.Sleep(30);
            }

        }
        
        #endregion

        #region Message Handlers

        private void OnMessageReceived(MessageBase msg)
        {
            Type type = msg.GetType();

            if (type == typeof(ValidationRequest))
            {
                ValidationRequestHandler(msg as ValidationRequest);
            }
            else if (type == typeof(SessionRequest))
            {
                SessionRequestHandler(msg as SessionRequest);
            }
            else if (type == typeof(SessionResponse))
            {
                SessionResponseHandler(msg as SessionResponse);
            }
            else if (type == typeof(EndSessionRequest))
            {
                EndSessionRequestHandler(msg as EndSessionRequest);
            }
            else if (type == typeof(DisconnectRequest))
            {
                DisconnectRequestHandler(msg as DisconnectRequest);
            }
            else if (OtherSideReceiver != null)
            {
                OtherSideReceiver.SendMessage(msg);
            }
        }

        private void EndSessionRequestHandler(EndSessionRequest request)
        {
            if (OtherSideReceiver != null)
            {
                OtherSideReceiver.SendMessage(new EndSessionRequest());
                OtherSideReceiver.Status = StatusEnum.Validated;
                OtherSideReceiver.OtherSideReceiver = null;

                this.OtherSideReceiver = null;
                this.Status = StatusEnum.Validated;
                this.SendMessage(new EndSessionResponse(request));
            }
        }

        private void DisconnectRequestHandler(DisconnectRequest request)
        {
            if (OtherSideReceiver != null)
            {
                OtherSideReceiver.SendMessage(new DisconnectRequest());
                OtherSideReceiver.Status = StatusEnum.Validated;
            }

            Disconnect();
        }

        private void SessionResponseHandler(SessionResponse response)
        {
            foreach (var receiver in Server.Receivers.Where(x => x != this))
            {
                if (receiver.Email == response.Email)
                {
                    response.Email = this.Email;

                    if (response.IsConfirmed)
                    {
                        receiver.OtherSideReceiver = this;
                        this.OtherSideReceiver = receiver;
                        this.Status = StatusEnum.InSession;
                        receiver.Status = StatusEnum.InSession;
                    }
                    else
                    {
                        response.HasError = true;
                        response.Exception = new Exception("The session request was refused by " + response.Email);
                    }

                    receiver.SendMessage(response);
                    return;
                }
            }
        }

        private void SessionRequestHandler(SessionRequest request)
        {
            SessionResponse response;

            if (this.Status != StatusEnum.Validated) //Added after a code project user comment.
            {
                response = new SessionResponse(request);
                response.IsConfirmed = false;
                response.HasError = true;
                response.Exception = new Exception("Could not request a new session. The current client is already in session, or is not loged in.");
                SendMessage(response);
                return;
            }

            foreach (var receiver in Server.Receivers.Where(x => x != this))
            {
                if (receiver.Email == request.Email)
                {
                    if (receiver.Status == StatusEnum.Validated)
                    {
                        request.Email = this.Email;
                        receiver.SendMessage(request);
                        return;
                    }
                }
            }

            response = new SessionResponse(request);
            response.IsConfirmed = false;
            response.HasError = true;
            response.Exception = new Exception(request.Email + " does not exists or not loged in or in session with another user.");
            SendMessage(response);
        }

        private void ValidationRequestHandler(ValidationRequest request)
        {
            ValidationResponse response = new ValidationResponse(request);

            EventArguments.ClientValidatingEventArgs args = new EventArguments.ClientValidatingEventArgs(() =>
            {
                //Confirm Action
                Status = StatusEnum.Validated;
                Email = request.Email;
                response.IsValid = true;
                SendMessage(response);
                Server.OnClientValidated(this);
            },
            () =>
            {
                //Refuse Action
                response.IsValid = false;
                response.HasError = true;
                response.Exception = new AuthenticationException("Login failed for user " + request.Email);
                SendMessage(response);
            });

            args.Receiver = this;
            args.Request = request;

            Server.OnClientValidating(args);
        }
        
        #endregion

        
    }
}
