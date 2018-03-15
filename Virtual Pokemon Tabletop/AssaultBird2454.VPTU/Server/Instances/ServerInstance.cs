using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AssaultBird2454.VPTU.Server.Instances
{
    public class ServerInstance
    {
        #region Variables and Objects
        #region Base Server
        /// <summary>
        /// Server class for handeling server communications
        /// </summary>
        public Networking.Server.TCP.TCP_Server Server { get; private set; }

        /// <summary>
        /// Server_CommandHandeler contains functions for creating and calling command callbacks
        /// </summary>
        public Networking.Server.Command_Handeler.Server_CommandHandeler Server_CommandHandeler { get; private set; }
        public Server.Base_Commands Base_Server_Commands { get; private set; }

        private object Logger;
        /// <summary>
        /// Server_Logger contains functions for server logging
        /// </summary>
        public object Server_Logger
        {
            get
            {
                return Logger;
            }
            set
            {
                if (value is Class.Logging.I_Logger)
                {
                    Logger = value;
                }
                else
                {
                    throw new Class.Logging.Invalid_Logger_Class_Exception();
                }
            }
        }

        /// <summary>
        /// Save Manager class, contains Save Data
        /// </summary>
        public SaveManager.SaveManager SaveManager { get; private set; }
        #endregion

        #region Authentication
        public List<KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>> Authenticated_Clients;

        public bool Authenticate_Client(Networking.Server.TCP.TCP_ClientNode cn, CommandData.Auth.Login Login)
        {
            Authenticated_Clients.RemoveAll(x => x.Key == cn);
            Authentication_Manager.Data.Identity ID = SaveManager.SaveData.AuthManager.Identity_GetIdentity(Login.Client_Key);

            if (ID != null)
            {
                Authentication_Manager.Data.User user = SaveManager.SaveData.AuthManager.Users_GetByID(ID.UserID);
                Authenticated_Clients.Add(new KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>(cn, user));

                ((Class.Logging.I_Logger)Server_Logger).Log("Client @ " + cn.ID + " Passed authenticated as User " + user.Name + " (" + user.IC_Name + ")", Class.Logging.LoggerLevel.Audit);
                cn.Send(new CommandData.Auth.Login() { Client_Key = ID.Key, Auth_State = CommandData.Auth.AuthState.Passed, UserData = user });
                return true;
            }
            else
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Client @ " + cn.ID + " Failed authentication", Class.Logging.LoggerLevel.Audit);
                cn.Send(new CommandData.Auth.Login() { Client_Key = "", Auth_State = CommandData.Auth.AuthState.Failed });
                return false;
            }
        }
        public void DeAuthenticate_Client(Networking.Server.TCP.TCP_ClientNode cn)
        {
            Authenticated_Clients.RemoveAll(x => x.Key == cn);
            ((Class.Logging.I_Logger)Server_Logger).Log("Client @ " + cn.ID + " DeAuthenticated", Class.Logging.LoggerLevel.Audit);
            cn.Send(new CommandData.Auth.Logout() { Auth_State = CommandData.Auth.AuthState.DeAuthenticated });
        }
        public bool CheckAuthentication_Client(Networking.Server.TCP.TCP_ClientNode cn)
        {
            if (Authenticated_Clients.FindAll(x => x.Key == cn).Count >= 1)
                return true;
            return false;
        }
        #endregion
        #endregion

        public ServerInstance(Class.Logging.I_Logger _Logger, string SaveData, int Port = 25444)
        {
            #region Logs
            Server_Logger = _Logger;
            ((Class.Logging.I_Logger)Server_Logger).Setup(Class.Logging.Logger_Type.Server);
            #endregion
            #region SaveManager
            ((Class.Logging.I_Logger)Server_Logger).Log("Initilizing Save Manager", Class.Logging.LoggerLevel.Debug);
            SaveManager = new VPTU.SaveManager.SaveManager(SaveData);// Create Manager Object

            ((Class.Logging.I_Logger)Server_Logger).Log("Loading Save Data", Class.Logging.LoggerLevel.Debug);
            SaveManager.Load_SaveData();// Load Data
            #endregion
            #region Command Handeler
            ((Class.Logging.I_Logger)Server_Logger).Log("Initilizing Command Handeler", Class.Logging.LoggerLevel.Debug);
            Server_CommandHandeler = new Networking.Server.Command_Handeler.Server_CommandHandeler();
            Server_CommandHandeler.CommandRegistered += Server_CommandHandeler_CommandRegistered;
            Server_CommandHandeler.CommandUnRegistered += Server_CommandHandeler_CommandUnRegistered;
            Server_CommandHandeler.RateLimitChanged_Event += Server_CommandHandeler_RateLimitChanged_Event;
            Server_CommandHandeler.RateLimitHit_Event += Server_CommandHandeler_RateLimitHit_Event;

            Base_Server_Commands = new Server.Base_Commands(this);

            ((Class.Logging.I_Logger)Server_Logger).Log("Registering Base Server Commands", Class.Logging.LoggerLevel.Debug);
            Base_Server_Commands.Register_Commands(Server_CommandHandeler);
            #endregion
            #region Networking
            ((Class.Logging.I_Logger)Server_Logger).Log("Initilizing Base Network", Class.Logging.LoggerLevel.Debug);
            Server = new Networking.Server.TCP.TCP_Server(IPAddress.Any, Server_CommandHandeler, Port);
            Server.TCP_AcceptClients_Changed += Server_TCP_AcceptClients_Changed;
            Server.TCP_ClientState_Changed += Server_TCP_ClientState_Changed;
            Server.TCP_Data_Error_Event += Server_TCP_Data_Error_Event;
            Server.TCP_Data_Event += Server_TCP_Data_Event;
            Server.TCP_ServerState_Changed += Server_TCP_ServerState_Changed;
            #endregion

            #region Plugins

            #endregion

            Authenticated_Clients = new List<KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>>();
            ((Class.Logging.I_Logger)Server_Logger).Log("Server Ready!", Class.Logging.LoggerLevel.Info);
        }

        public void StartServerInstance()
        {
            #region Start Server
            ((Class.Logging.I_Logger)Server_Logger).Log("Starting Base Network", Class.Logging.LoggerLevel.Info);
            try
            {
                Authenticated_Clients = new List<KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>>();
                Server.Start();
                ((Class.Logging.I_Logger)Server_Logger).Log("Base Network Started", Class.Logging.LoggerLevel.Info);
            }
            catch (Exception e)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Base Network Failed to Start", Class.Logging.LoggerLevel.Fatil);
                ((Class.Logging.I_Logger)Server_Logger).Log(e.ToString(), Class.Logging.LoggerLevel.Debug);
                Authenticated_Clients = new List<KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>>();
                StopServerInstance();
                return;
            }
            #endregion
        }
        public void StopServerInstance()
        {
            #region Stop Server
            ((Class.Logging.I_Logger)Server_Logger).Log("Stopping Base Network", Class.Logging.LoggerLevel.Info);
            try
            {
                Server.Stop();
                ((Class.Logging.I_Logger)Server_Logger).Log("Base Network Stopped", Class.Logging.LoggerLevel.Info);
            }
            catch (Exception e)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Base Network Failed to Stop", Class.Logging.LoggerLevel.Fatil);
                ((Class.Logging.I_Logger)Server_Logger).Log(e.ToString(), Class.Logging.LoggerLevel.Debug);
                return;
            }
            #endregion
        }

        #region Event Handelers
        private void Server_CommandHandeler_CommandUnRegistered(string Command)
        {
            ((Class.Logging.I_Logger)Server_Logger).Log("Command Unregistered -> Command: \"" + Command + "\"", Class.Logging.LoggerLevel.Debug);
        }
        private void Server_CommandHandeler_CommandRegistered(string Command)
        {
            ((Class.Logging.I_Logger)Server_Logger).Log("Command Registered -> Command: \"" + Command + "\"", Class.Logging.LoggerLevel.Debug);
        }
        private void Server_CommandHandeler_RateLimitChanged_Event(string Command, bool Enabled, int Limit)
        {
            if (Enabled)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Command Rate Limit -> Command: \"" + Command + "\" Setting: \"" + Limit + " Invokes per 5 minute block per client\"", Class.Logging.LoggerLevel.Debug);
            }
            else
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Command Rate Limit -> Command: \"" + Command + "\" Setting: \"No Limit\"", Class.Logging.LoggerLevel.Debug);
            }
        }
        private void Server_CommandHandeler_RateLimitHit_Event(string Name, Networking.Server.TCP.TCP_ClientNode Client)
        {
            ((Class.Logging.I_Logger)Server_Logger).Log("Client \"" + Client.ID + "\" hit the rate limit for command \" " + Name + " \"", Class.Logging.LoggerLevel.Warning);
        }

        private void Server_TCP_ServerState_Changed(Networking.Data.Server_Status Server_State)
        {
            ((Class.Logging.I_Logger)Server_Logger).Log("Base Network is " + Server_State.ToString(), Class.Logging.LoggerLevel.Debug);
        }
        private void Server_TCP_Data_Event(string Data, Networking.Server.TCP.TCP_ClientNode Client, Networking.Server.TCP.DataDirection Direction)
        {
            if (Direction == Networking.Server.TCP.DataDirection.Recieve)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Data Recieved from " + Client.ID, Class.Logging.LoggerLevel.Debug);
            }
            else if (Direction == Networking.Server.TCP.DataDirection.Send)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Data Sent to " + Client.ID, Class.Logging.LoggerLevel.Debug);
            }
        }
        private void Server_TCP_Data_Error_Event(Exception ex, Networking.Server.TCP.DataDirection Direction)
        {
            ((Class.Logging.I_Logger)Server_Logger).Log("Failed to " + Direction.ToString() + " Data (Base Network)", Class.Logging.LoggerLevel.Error);
        }
        private void Server_TCP_ClientState_Changed(Networking.Server.TCP.TCP_ClientNode Client, Networking.Data.Client_ConnectionStatus Client_State)
        {
            if (Client_State == Networking.Data.Client_ConnectionStatus.Connected)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Client Connected (Base Network) Address: " + Client.ID, Class.Logging.LoggerLevel.Info);

                if (Authenticated_Clients.FindAll(x => x.Key == Client).Count == 0)
                {
                    Authenticated_Clients.Add(new KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>(Client, null));
                }
            }
            else if (Client_State == Networking.Data.Client_ConnectionStatus.Connecting)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Client Connecting (Base Network) Address: " + Client.ID, Class.Logging.LoggerLevel.Info);

                if (Authenticated_Clients.FindAll(x => x.Key == Client).Count == 0)
                {
                    Authenticated_Clients.Add(new KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>(Client, null));
                }
            }
            else if (Client_State == Networking.Data.Client_ConnectionStatus.Disconnected)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Client Disconnected (Base Network) Address: " + Client.ID, Class.Logging.LoggerLevel.Info);

                Authenticated_Clients.RemoveAll(x => x.Key == Client);
            }
            else if (Client_State == Networking.Data.Client_ConnectionStatus.Dropped)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Client Dropped Connection (Base Network) Address: " + Client.ID, Class.Logging.LoggerLevel.Info);

                Authenticated_Clients.RemoveAll(x => x.Key == Client);
            }
            else if (Client_State == Networking.Data.Client_ConnectionStatus.Encrypted)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Client Encrypted Network Transmittions (Base Network) Address: " + Client.ID, Class.Logging.LoggerLevel.Info);

                if (Authenticated_Clients.FindAll(x => x.Key == Client).Count == 0)
                {
                    Authenticated_Clients.Add(new KeyValuePair<Networking.Server.TCP.TCP_ClientNode, Authentication_Manager.Data.User>(Client, null));
                }
            }
        }
        private void Server_TCP_AcceptClients_Changed(bool Accepting_Connections)
        {
            if (Accepting_Connections)
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Base Network is Accepting New Connections", Class.Logging.LoggerLevel.Info);
            }
            else
            {
                ((Class.Logging.I_Logger)Server_Logger).Log("Base Network is Refusing New Connections", Class.Logging.LoggerLevel.Info);
            }
        }
        #endregion
    }
}
// ((Class.Logging.I_Logger)Server_Logger).Log("", Class.Logging.LoggerLevel.Info);