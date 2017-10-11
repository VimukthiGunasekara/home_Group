using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AdvancedTCP.Shared.Messages;

namespace AdvancedTCP.Server
{
    public class Server
    {
        /// <summary>
        /// The TcpListener that is encapsulated behind this Server instance.
        /// </summary>
        public TcpListener Listener { get; set; }
        /// <summary>
        /// The Port that is used to listen to incoming connections.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Returns true if the Server instance is running.
        /// </summary>
        public bool IsStarted { get; private set; }
        /// <summary>
        /// List of currently connected receivers.
        /// </summary>
        public List<Receiver> Receivers { get; private set; }

        #region Events
        /// <summary>
        /// Raises when a new client is waiting for validation.
        /// </summary>
        public event Delegates.ClientValidatingDelegate ClientValidating;
        /// <summary>
        /// Raises when a new client is connected.
        /// </summary>
        public event Delegates.ClientBasicDelegate ClientConnected;
        /// <summary>
        /// Raises when a new client is validated.
        /// </summary>
        public event Delegates.ClientBasicDelegate ClientValidated;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new Server instance.
        /// </summary>
        /// <param name="port">The port number that is used to listen for incoming connections.</param>
        public Server(int port)
        {
            Receivers = new List<Receiver>();
            Port = port;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Start Listening for incoming connections.
        /// </summary>
        public void Start()
        {
            if (!IsStarted)
            {
                Listener = new TcpListener(System.Net.IPAddress.Any, Port);
                Listener.Start();
                IsStarted = true;
                Debug.WriteLine("Server Started!");

                //Start Async pattern for accepting new connections
                WaitForConnection();
            }
        }
        /// <summary>
        /// Stop listening for incoming connections.
        /// </summary>
        public void Stop()
        {
            if (IsStarted)
            {
                Listener.Stop();
                IsStarted = false;

                Debug.WriteLine("Server Stoped!");
            }
        }
        
        #endregion

        #region Incoming Connections Methods

        private void WaitForConnection()
        {
            Listener.BeginAcceptTcpClient(new AsyncCallback(ConnectionHandler), null);
        }

        private void ConnectionHandler(IAsyncResult ar)
        {
            lock (Receivers)
            {
                Receiver newClient = new Receiver(Listener.EndAcceptTcpClient(ar), this);
                newClient.Start();
                Receivers.Add(newClient);
                OnClientConnected(newClient);
            }

            Debug.WriteLine("New Client Connected");
            WaitForConnection();
        }

        
        #endregion

        #region Virtuals
        public virtual void OnClientValidating(EventArguments.ClientValidatingEventArgs args)
        {
            if (ClientValidating != null) ClientValidating(args);
        }

        public virtual void OnClientConnected(Receiver receiver)
        {
            if (ClientConnected != null) ClientConnected(receiver);
        }

        public virtual void OnClientValidated(Receiver receiver)
        {
            if (ClientValidated != null) ClientValidated(receiver);
        }
        #endregion
    }
}
