﻿using AssaultBird2454.VPTU.Networking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AssaultBird2454.VPTU.Networking.Client.TCP
{
    public enum NetworkMode { Standard, SSL }

    public class TCP_Client
    {
        #region Events
        /// <summary>
        /// An event that is fired when the connection state changes
        /// </summary>
        public event TCP_ConnectionState_Handeler ConnectionStateEvent;
        private void Fire_ConnectionStateEvent(Data.Client_ConnectionStatus state)
        {
            ConnectionStateEvent?.Invoke(state);
        }

        /// <summary>
        /// An Event that is fired when the client transmitts or recieves data from the server
        /// </summary>
        public event TCP_Data DataEvent;
        private void Fire_DataEvent(string Data, DataDirection Direction)
        {
            DataEvent?.Invoke(Data, Direction);
        }

        #endregion

        #region Variables
        private TcpClient Client;// Client Object
        private IPAddress TCP_IPAddress;// Servers IPAddress
        private StateObject StateObject;// Client State Object
        private string[] delimiter = new string[] { "|<EOD>|" };// The Delimiting string for commands
        private int TCP_Port;// The portnumber that the server is running on

        private NetworkMode NetMode;
        #region Data Que
        private Queue<string> DataQue;// Data Que
        private Thread DataQueThread;// A Thread to run the data que
        private readonly EventWaitHandle ReadQueWait;// An event for signaling to the reader that data is in Que
        #endregion

        /// <summary>
        /// The Command Handeler that the server will use to invoke commands
        /// </summary>
        public Command_Handeler.Client_CommandHandeler CommandHandeler;
        #endregion

        #region Variable Handelers
        /// <summary>
        /// Get or Set the IPAddress. IPAddress wont be affected if client is connected
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                return TCP_IPAddress;
            }
            set
            {
                if (Client.Connected)
                {
                    return;
                }
                //Event Trigger Here
                TCP_IPAddress = value;
            }
        }

        /// <summary>
        /// Get or Set the Port. Port wont be affected if client is connected
        /// </summary>
        public int Port
        {
            get
            {
                return TCP_Port;
            }
            set
            {
                if (Client.Connected)
                {
                    return;
                }
                //Event Trigger
                TCP_Port = value;
            }
        }

        /// <summary>
        /// Checks if the client is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (Client != null)
                {
                    return Client.Connected;
                }
                else
                {
                    return false;
                }

            }
        }
        #endregion

        public TCP_Client(IPAddress Address, Command_Handeler.Client_CommandHandeler _CommandHandeler, int Port = 25444)
        {
            NetMode = NetworkMode.Standard;
            ReadQueWait = new EventWaitHandle(false, EventResetMode.AutoReset, "ReadQue");
            TCP_IPAddress = Address;// Sets the IPAddress
            TCP_Port = Port;// Sets the Port
            CommandHandeler = _CommandHandeler;// Sets the Command Callback

            try
            {
                CommandHandeler.RegisterCommand<Data.InternalNetworkCommand>("Network Command");

                CommandHandeler.GetCommand("Network Command").Command_Executed += Client_Commands;
            }
            catch (Networking.Client.Command_Handeler.CommandNameTakenException e)
            {

            }
        }

        private void Client_Commands(object _Data)
        {
            Data.InternalNetworkCommand Data = (Data.InternalNetworkCommand)_Data;

            if (Data.CommandType == Networking.Data.Commands.SSL_Enable)
            {
                if (Data.Response == Networking.Data.ResponseCode.Not_Avaliable)
                {
                    // Not Avaliable
                }
                else if (Data.Response == Networking.Data.ResponseCode.Avaliable)
                {
                    SslStream sslStream = new SslStream(Client.GetStream(), true, ValidateCert);
                    StateObject.SSL = sslStream;
                    //new SslStream(Client.GetStream(), true, new RemoteCertificateValidationCallback(ValidateCert), null, EncryptionPolicy.RequireEncryption);
                    SendData(new Data.InternalNetworkCommand(Networking.Data.Commands.SSL_Enable, Networking.Data.ResponseCode.Ready));
                }
                else if (Data.Response == Networking.Data.ResponseCode.Ready)
                {
                    NetMode = NetworkMode.SSL;
                    SendData(new Data.InternalNetworkCommand(Networking.Data.Commands.SSL_Enable, Networking.Data.ResponseCode.OK));
                    Fire_ConnectionStateEvent(Networking.Data.Client_ConnectionStatus.Encrypted);
                }
                else if (Data.Response == Networking.Data.ResponseCode.Error)
                {
                    // Error
                }
            }
            else if (Data.CommandType == Networking.Data.Commands.SSL_Dissable)
            {

            }
            else if (Data.CommandType == Networking.Data.Commands.SSL_Active)
            {

            }
            else if (Data.CommandType == Networking.Data.Commands.SetBufferSize)
            {

            }
            else
            {

            }
        }

        private bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #region Client Methods
        /// <summary>
        /// Connects to the server, it will also disconnect any connections
        /// </summary>
        public void Connect()
        {
            Disconnect();// Disconnects (No Error if it was never connected)

            Fire_ConnectionStateEvent(Data.Client_ConnectionStatus.Connecting);

            Client = new TcpClient();// Creates a new client object

            try
            {
                Client.Connect(TCP_IPAddress, TCP_Port);// Connects to the server and on connect will call the connect call back
            }
            catch (Exception e)
            {
                Client = null;
                Fire_ConnectionStateEvent(Data.Client_ConnectionStatus.Disconnected);
                return;
            }

            DataQue = new Queue<string>();// Creates the que

            DataQueThread = new Thread(new ThreadStart(() =>
            {
                QueRead();
            }));
            DataQueThread.IsBackground = true;
            DataQueThread.Start();// Creates and Starts the Read Thread

            StateObject = new StateObject(Client.Client, 32768);// Creates State Object for Client
            StartListening();// Starts Listening

            Fire_ConnectionStateEvent(Data.Client_ConnectionStatus.Connected);
        }

        /// <summary>
        /// Disconnects the Client from the server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                DataQueThread.Abort();// Stopps the read thread
                DataQueThread = null;// Clears the read Thread

                Client.Close();// Closes connection
                Client = null;// Clears Client

                Fire_ConnectionStateEvent(Data.Client_ConnectionStatus.Disconnected);
            }
            catch
            {
                /* Failed to disconnect or was never connected to anything */
            }
        }

        public void Enable_SSL()
        {
            SendData(new Data.InternalNetworkCommand(Data.Commands.SSL_Enable));
        }

        public long PingServer
        {
            get
            {
                long pingTime = 0;
                Ping pingSender = new Ping();
                
                PingReply reply = pingSender.Send(IPAddress);

                if (reply.Status == IPStatus.Success)
                {
                    pingTime = reply.RoundtripTime;
                }

                return pingTime;
            }
        }
        #endregion

        #region Data Events
        /// <summary>
        /// A Method to start the reading of network data again
        /// </summary>
        private void StartListening()
        {
            if (NetMode == NetworkMode.Standard)
            {
                Client.GetStream().BeginRead(StateObject.buffer, 0, StateObject.BUFFER_SIZE, Client_DataRecv, StateObject);
            }
            else if (NetMode == NetworkMode.SSL)
            {
                StateObject.SSL.BeginRead(StateObject.buffer, 0, StateObject.BUFFER_SIZE, Client_DataSslRecv, StateObject);
            }
        }

        private void Client_DataRecv(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            Socket s = so.workSocket;
            int read;

            try { read = s.EndReceive(ar); }
            catch
            {
                Disconnect();// Disconnects
                return;
            }

            if (read > 0)
            {
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));

                if (so.sb.ToString().Contains("|<EOD>|"))
                {
                    if (so.sb.Length > 1)
                    {
                        string data = Helper.GetUntilOrEmpty(so.sb, "|<EOD>|");
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            DataQue.Enqueue(data);// Ques the data
                            ReadQueWait.Set();// Signals new data is avaliable
                        }
                    }
                }

                StartListening();
            }
            else
            {
                Disconnect();// Disconnects
                return;
            }
        }
        private void Client_DataSslRecv(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            Socket s = so.workSocket;
            int read;

            try { read = s.EndReceive(ar); }
            catch
            {
                Disconnect();// Disconnects
                return;
            }

            if (read > 0)
            {
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));

                if (so.sb.ToString().Contains("|<EOD>|"))
                {
                    if (so.sb.Length > 1)
                    {
                        string data = Helper.GetUntilOrEmpty(so.sb, "|<EOD>|");
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            DataQue.Enqueue(data);// Ques the data
                            ReadQueWait.Set();// Signals new data is avaliable
                        }
                    }
                }

                StartListening();
            }
            else
            {
                Disconnect();// Disconnects
                return;
            }
        }

        private void Client_DataTran(IAsyncResult ar)
        {
            try
            {
                TcpClient tcpc = (TcpClient)ar.AsyncState;//Gets the client data is going to
                tcpc.GetStream().EndWrite(ar);//Ends client write stream
            }
            catch (Exception e)
            {
                //TCP_Data_Error_Event.Invoke(e, DataDirection.Send);
                /* Transmition Error */
            }
        }
        private void Client_DataSslTran(IAsyncResult ar)
        {
            try
            {
                TcpClient tcpc = (TcpClient)ar.AsyncState;//Gets the client data is going to
                tcpc.GetStream().EndWrite(ar);//Ends client write stream
            }
            catch (Exception e)
            {
                //TCP_Data_Error_Event.Invoke(e, DataDirection.Send);
                /* Transmition Error */
            }
        }

        /// <summary>
        /// Sends data to the server
        /// </summary>
        /// <param name="Data">The data being sent to the server</param>
        public void SendData(object Data)
        {
            if (Data is Data.NetworkCommand)
            {
                if (IsConnected)
                {
                    if (NetMode == NetworkMode.Standard)
                    {
                        string JSONData = Newtonsoft.Json.JsonConvert.SerializeObject(Data);// Serialises the data to be sent
                        Fire_DataEvent(JSONData, DataDirection.Send);// Invokes the data recieved event
                        byte[] Tx = Encoding.UTF8.GetBytes(JSONData + "|<EOD>|");
                        Client.GetStream().BeginWrite(Tx, 0, Tx.Length, Client_DataTran, Client);// Sneds the data to the server
                    }
                    else
                    {
                        string JSONData = Newtonsoft.Json.JsonConvert.SerializeObject(Data);// Serialises the data to be sent
                        Fire_DataEvent(JSONData, DataDirection.Send);// Invokes the data recieved event
                        byte[] Tx = Encoding.UTF8.GetBytes(JSONData + "|<EOD>|");
                        StateObject.SSL.BeginWrite(Tx, 0, Tx.Length, Client_DataSslTran, StateObject.SSL);// Sneds the data to the server
                    }
                }
            }
            else
            {
                throw new NotNetworkDataException();
            }
        }
        #endregion

        #region RX Message Que
        public void QueRead()
        {
            while (true)
            {
                ReadQueWait.WaitOne();// Waits for data

                while (DataQue.Count >= 1)// While there is data
                {
                    string Data = DataQue.Dequeue();// Gets the data and then removes it from the que
                    Fire_DataEvent(Data, DataDirection.Recieve);// Invokes the data recieved event
                    CommandHandeler.InvokeCommand(Data);// Handels the data
                }
            }
        }
        #endregion
    }
}