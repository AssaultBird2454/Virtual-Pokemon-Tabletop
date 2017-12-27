﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media;

namespace AssaultBird2454.VPTU.Client
{
    public static class Program
    {
        public static ProjectInfo VersioningInfo
        {
            get
            {
                using (Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssaultBird2454.VPTU.Client.ProjectVariables.json"))
                {
                    using (StreamReader read = new StreamReader(str))
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectInfo>(read.ReadToEnd());
                    }
                }
            }
        }

        /// <summary>
        /// Assembly Directory
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(path);
            }
        }

        public static MainWindow MainWindow { get; internal set; }
        public static Server.Instances.ClientInstance ClientInstance { get; set; }
        public static Server.Instances.ServerInstance ServerInstance { get; set; }
        public static SaveManager.Data.SaveFile.PTUSaveData DataCache { get; set; }
        private static System.Timers.Timer Ping_Timer { get; set; }
        public static List<Authentication_Manager.Data.ClientIdentity> Identities { get; set; }

        public static NotifyIcon NotifyIcon { get; internal set; }
        public static object ClientLogger
        {
            get
            {
                return ClientInstance.Client_Logger;
            }
            set
            {
                if (ClientInstance != null)
                {
                    if (value is VPTU.Server.Class.Logging.I_Logger)
                    {
                        ClientInstance.Client_Logger = value;
                    }
                    else
                    {

                    }
                }
            }
        }

        public static void Settings_Save()
        {
            #region Identities
            try
            {
                File.Delete(AssemblyDirectory + @"\Client_Identities.json");
                using (StreamWriter sw = new StreamWriter(new FileStream(AssemblyDirectory + @"\Client_Identities.json", FileMode.OpenOrCreate)))
                {
                    string Client_Identities = Newtonsoft.Json.JsonConvert.SerializeObject(Identities);
                    sw.WriteLine(Client_Identities);
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a error saving identities to file!\nNo Identities have been saved, This means that you will need to create / add them again before you connect to the servers again...", "Error Saving Identities to File");
                MessageBox.Show(ex.ToString(), "Stack Trace");
            }
            #endregion
        }
        public static void Settings_Load()
        {
            #region Identities
            try
            {
                using (StreamReader sr = new StreamReader(new FileStream(AssemblyDirectory + @"\Client_Identities.json", FileMode.OpenOrCreate)))
                {
                    string Client_Identities = sr.ReadToEnd();
                    Identities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Authentication_Manager.Data.ClientIdentity>>(Client_Identities);
                }
            }
            catch
            {
                Identities = new List<Authentication_Manager.Data.ClientIdentity>();
            }

            if (Identities == null) { Identities = new List<Authentication_Manager.Data.ClientIdentity>(); }
            #endregion
        }

        internal static void Setup_Client()
        {
            Ping_Timer = new System.Timers.Timer();// Creates a timer
            Ping_Timer.Interval = 3000;// Sets for 3 Second intervals
            Ping_Timer.Elapsed += Ping_Timer_Elapsed;// Sets event

            ClientInstance.Client.ConnectionStateEvent += Client_ConnectionStateEvent;// Connection State Command
            Setup_Commands();// Configures the commands
        }

        private static void Setup_Commands()
        {
            ClientInstance.Client_CommandHandeler.GetCommand("ConnectionState").Command_Executed += ConnectionState_Executed;

            #region Auth
            ClientInstance.Client_CommandHandeler.GetCommand("Auth_Login").Command_Executed += MainWindow.Auth_Login_Executed;
            #endregion
            #region Pokedex
            ClientInstance.Client_CommandHandeler.GetCommand("Pokedex_Pokemon_GetList").Command_Executed += MainWindow.Pokedex_Pokemon_GetList_Executed;
            ClientInstance.Client_CommandHandeler.GetCommand("Pokedex_Pokemon_Get").Command_Executed += MainWindow.Pokedex_Pokemon_Get_Executed;
            #endregion
            #region Resources
            ClientInstance.Client_CommandHandeler.GetCommand("Resources_Image_Get").Command_Executed += MainWindow.Resources_Image_Get_Pokedex_Executed;
            #endregion
            #region Entities
            ClientInstance.Client_CommandHandeler.GetCommand("Entities_All_GetList").Command_Executed += MainWindow.Entities_All_GetList_Executed;
            ClientInstance.Client_CommandHandeler.GetCommand("Entities_Pokemon_Get").Command_Executed += MainWindow.Entities_Pokemon_Get_Executed;
            ClientInstance.Client_CommandHandeler.GetCommand("Resources_Image_Get").Command_Executed += MainWindow.Resources_Image_Get_Entities_Executed;
            #endregion
        }

        private static void Client_ConnectionStateEvent(Networking.Data.Client_ConnectionStatus ConnectionState)
        {
            if (ConnectionState == Networking.Data.Client_ConnectionStatus.Connected)
            {
                MainWindow.Status_Set_Color((Color)Colors.Green);
                MainWindow.Status_Set_Address(ClientInstance.Server_Address.ToString());
                MainWindow.Status_Set_Port(ClientInstance.Server_Port);
                //MainWindow.Status_Set_PlayerName("");
                //MainWindow.Status_Set_CampaignName("");

                Ping_Timer.Start();
            }
            else if (ConnectionState == Networking.Data.Client_ConnectionStatus.Connecting)
            {
                MainWindow.Status_Set_Color((Color)Colors.Yellow);
                MainWindow.Status_Set_Address(ClientInstance.Server_Address.ToString());
                MainWindow.Status_Set_Port(ClientInstance.Server_Port);
                //MainWindow.Status_Set_PlayerName("");
                //MainWindow.Status_Set_CampaignName("");
            }
            else if (ConnectionState == Networking.Data.Client_ConnectionStatus.Disconnected)
            {
                MainWindow.Status_Set_Color((Color)Colors.Red);
                MainWindow.Status_Set_Address("Not Connected");
                MainWindow.Status_Set_Port(0);
                //MainWindow.Status_Set_PlayerName("");
                //MainWindow.Status_Set_CampaignName("");
            }
            else if (ConnectionState == Networking.Data.Client_ConnectionStatus.Dropped)
            {
                MainWindow.Status_Set_Color((Color)Colors.Red);
                MainWindow.Status_Set_Address("Not Connected");
                MainWindow.Status_Set_Port(0);
                //MainWindow.Status_Set_PlayerName("");
                //MainWindow.Status_Set_CampaignName("");
            }
            else if (ConnectionState == Networking.Data.Client_ConnectionStatus.Encrypted)
            {
                MainWindow.Status_Set_Color((Color)Colors.Green);
                MainWindow.Status_Set_Address(ClientInstance.Server_Address.ToString());
                MainWindow.Status_Set_Port(ClientInstance.Server_Port);
                //MainWindow.Status_Set_PlayerName("");
                //MainWindow.Status_Set_CampaignName("");

                Ping_Timer.Start();
            }
        }

        private static void Ping_Timer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                MainWindow.Status_Set_Ping(Convert.ToInt32(ClientInstance.Client.PingServer));
            }
            catch
            {
                Ping_Timer.Stop();
                MainWindow.Status_Set_Ping(0);
            }
        }

        private static void ConnectionState_Executed(object Data)
        {
            Server.Instances.CommandData.Connection.Connect Connect = (Server.Instances.CommandData.Connection.Connect)Data;

            if (Connect.Connection_State == Server.Instances.CommandData.Connection.ConnectionStatus.Rejected)
            {
                MessageBox.Show("The server rejected your connection request...\n\nReason: Server Locked");
            }
            else if (Connect.Connection_State == Server.Instances.CommandData.Connection.ConnectionStatus.ServerFull)
            {
                MessageBox.Show("The server rejected your connection request...\n\nReason: Server Full");
            }
        }
    }
}
