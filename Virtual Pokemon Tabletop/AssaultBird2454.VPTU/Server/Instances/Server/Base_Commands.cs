using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssaultBird2454.VPTU.Networking.Server.TCP;
using AssaultBird2454.VPTU.Server.Instances.CommandData.Pokedex;

namespace AssaultBird2454.VPTU.Server.Instances.Server
{
    public class Base_Commands
    {
        ServerInstance Instance;

        /// <summary>
        /// Creates a new instance of a Base_Commands class for handeling command calls over the network
        /// </summary>
        public Base_Commands(ServerInstance _Instance)
        {
            Instance = _Instance;
        }

        /// <summary>
        /// Registers all base commands
        /// </summary>
        /// <param name="CommandHandeler">Command Handeler that the commands are being Registered to</param>
        /// 
        public void Register_Commands(Networking.Server.Command_Handeler.Server_CommandHandeler CommandHandeler)
        {
            // CommandHandeler.GetCommand("").SetRateLimit(true);

            CommandHandeler.RegisterCommand<CommandData.Connection.Connect>("ConnectionState");
            CommandHandeler.GetCommand("ConnectionState").SetRateLimit(false);
            CommandHandeler.GetCommand("ConnectionState").Command_Executed += ConnectionState_Executed;

            #region Auth
            CommandHandeler.RegisterCommand<CommandData.Auth.Login>("Auth_Login");
            CommandHandeler.GetCommand("Auth_Login").SetRateLimit(false);
            CommandHandeler.GetCommand("Auth_Login").Command_Executed += Auth_Login_Executed;
            CommandHandeler.RegisterCommand<CommandData.Auth.Logout>("Auth_Logout");
            CommandHandeler.GetCommand("Auth_Logout").SetRateLimit(false);
            CommandHandeler.GetCommand("Auth_Logout").Command_Executed += Auth_Logout_Executed;

            CommandHandeler.RegisterCommand<object>("Auth_Create");
            CommandHandeler.GetCommand("Auth_Create").SetRateLimit(false);
            CommandHandeler.RegisterCommand<object>("Auth_Delete");
            CommandHandeler.GetCommand("Auth_Delete").SetRateLimit(false);
            CommandHandeler.RegisterCommand<object>("Auth_Edit");
            CommandHandeler.GetCommand("Auth_Edit").SetRateLimit(false);
            CommandHandeler.RegisterCommand<object>("Auth_List");
            CommandHandeler.GetCommand("Auth_List").SetRateLimit(true);
            CommandHandeler.RegisterCommand<object>("Auth_Get");
            CommandHandeler.GetCommand("Auth_Get").SetRateLimit(false);
            #endregion

            #region Base
            CommandHandeler.RegisterCommand<object>("Base_SaveData_Save");
            CommandHandeler.GetCommand("Base_SaveData_Save").SetRateLimit(false);
            CommandHandeler.GetCommand("Base_SaveData_Save").Command_Executed += Base_SaveData_Save_Executed;

            CommandHandeler.RegisterCommand<object>("Base_SaveData_Load");
            CommandHandeler.GetCommand("Base_SaveData_Load").SetRateLimit(false);
            CommandHandeler.RegisterCommand<object>("Base_SaveData_Upload");
            CommandHandeler.GetCommand("Base_SaveData_Upload").SetRateLimit(false);
            CommandHandeler.RegisterCommand<object>("Base_SaveData_Download");
            CommandHandeler.GetCommand("Base_SaveData_Download").SetRateLimit(false);
            CommandHandeler.RegisterCommand<object>("Base_Settings_Get");
            CommandHandeler.GetCommand("Base_Settings_Get").SetRateLimit(false);
            CommandHandeler.RegisterCommand<object>("Base_Settings_Set");
            CommandHandeler.GetCommand("Base_Settings_Set").SetRateLimit(false);
            // CommandHandeler.RegisterCommand<object>("");
            #endregion

            #region Permissions (Not Implemented)

            #endregion

            #region Pokedex
            // All
            CommandHandeler.RegisterCommand<CommandData.Pokedex.Pokedex_Pokemon_GetList>("Pokedex_Get");
            CommandHandeler.GetCommand("Pokedex_Get").SetRateLimit(true, 10);
            CommandHandeler.GetCommand("Pokedex_Get").Command_Executed += Pokedex_Get_Executed;

            // Pokemon
            CommandHandeler.RegisterCommand<CommandData.Pokedex.Pokedex_Pokemon_GetList>("Pokedex_Pokemon_GetList");
            CommandHandeler.GetCommand("Pokedex_Pokemon_GetList").SetRateLimit(true, 10);
            CommandHandeler.GetCommand("Pokedex_Pokemon_GetList").Command_Executed += Pokedex_Pokemon_GetList_Executed;
            CommandHandeler.RegisterCommand<CommandData.Pokedex.Pokedex_Pokemon>("Pokedex_Pokemon_Get");
            CommandHandeler.GetCommand("Pokedex_Pokemon_Get").SetRateLimit(true);
            CommandHandeler.GetCommand("Pokedex_Pokemon_Get").Command_Executed += Pokedex_Pokemon_Get_Executed;

            CommandHandeler.RegisterCommand<CommandData.Pokedex.Pokedex_Pokemon>("Pokedex_Pokemon_Add");
            CommandHandeler.GetCommand("Pokedex_Pokemon_Add").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<CommandData.Pokedex.Pokedex_Pokemon>("Pokedex_Pokemon_Edit");
            CommandHandeler.GetCommand("Pokedex_Pokemon_Edit").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<CommandData.Pokedex.Pokedex_Pokemon>("Pokedex_Pokemon_Remove");
            CommandHandeler.GetCommand("Pokedex_Pokemon_Remove").SetRateLimit(true, 100);

            // Moves
            CommandHandeler.RegisterCommand<string>("Pokedex_Moves_Get");
            CommandHandeler.GetCommand("Pokedex_Moves_Get").SetRateLimit(true);
            CommandHandeler.GetCommand("Pokedex_Moves_Get").Command_Executed += Pokedex_Moves_Get_Executed;

            CommandHandeler.RegisterCommand<string>("Pokedex_Moves_Add");
            CommandHandeler.GetCommand("Pokedex_Moves_Add").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<string>("Pokedex_Moves_Edit");
            CommandHandeler.GetCommand("Pokedex_Moves_Edit").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<string>("Pokedex_Moves_Remove");
            CommandHandeler.GetCommand("Pokedex_Moves_Remove").SetRateLimit(true, 100);
            #endregion

            #region Entities
            CommandHandeler.RegisterCommand<CommandData.Entities.Entities_All_GetList>("Entities_All_GetList");
            CommandHandeler.GetCommand("Entities_All_GetList").SetRateLimit(true, 5);
            CommandHandeler.GetCommand("Entities_All_GetList").Command_Executed += Entities_All_GetList_Executed;

            CommandHandeler.RegisterCommand<object>("Entities_Pokemon_GetList");
            CommandHandeler.GetCommand("Entities_Pokemon_GetList").SetRateLimit(true, 10);
            CommandHandeler.RegisterCommand<CommandData.Entities.Entities_Pokemon_Get>("Entities_Pokemon_Get");
            CommandHandeler.GetCommand("Entities_Pokemon_Get").SetRateLimit(true);
            CommandHandeler.GetCommand("Entities_Pokemon_Get").Command_Executed += Entities_Pokemon_Get_Executed;
            CommandHandeler.RegisterCommand<object>("Entities_Pokemon_Create");
            CommandHandeler.GetCommand("Entities_Pokemon_Create").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<object>("Entities_Pokemon_Edit");
            CommandHandeler.GetCommand("Entities_Pokemon_Edit").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<object>("Entities_Pokemon_Delete");
            CommandHandeler.GetCommand("Entities_Pokemon_Delete").SetRateLimit(true, 100);

            CommandHandeler.RegisterCommand<object>("Entities_Trainer_GetList");
            CommandHandeler.GetCommand("Entities_Trainer_GetList").SetRateLimit(true, 10);
            CommandHandeler.RegisterCommand<object>("Entities_Trainer_Get");
            CommandHandeler.GetCommand("Entities_Trainer_Get").SetRateLimit(true);
            CommandHandeler.RegisterCommand<object>("Entities_Trainer_Create");
            CommandHandeler.GetCommand("Entities_Trainer_Create").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<object>("Entities_Trainer_Edit");
            CommandHandeler.GetCommand("Entities_Trainer_Edit").SetRateLimit(true, 100);
            CommandHandeler.RegisterCommand<object>("Entities_Trainer_Delete");
            CommandHandeler.GetCommand("Entities_Trainer_Delete").SetRateLimit(true, 100);
            // CommandHandeler.RegisterCommand<string>("Entities_");
            #endregion

            #region Battles
            // Battles
            CommandHandeler.RegisterCommand<object>("Battle_Participants_Add");// Adds Participants to the Battle Instance
            CommandHandeler.GetCommand("Battle_Participants_Add").SetRateLimit(true, 100);
            CommandHandeler.GetCommand("Battle_Participants_Add").Command_Executed += Battle_Participants_Add_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_Participants_Edit");// Edits a participant in the Battle Instnace
            CommandHandeler.GetCommand("Battle_Participants_Edit").SetRateLimit(true, 100);
            CommandHandeler.GetCommand("Battle_Participants_Edit").Command_Executed += Battle_Participants_Edit_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_Participants_Remove");// Removes a participant from the Battle Instance
            CommandHandeler.GetCommand("Battle_Participants_Remove").SetRateLimit(true, 100);
            CommandHandeler.GetCommand("Battle_Participants_Remove").Command_Executed += Battle_Participants_Remove_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_Participants_Get");// Gets the participants in the Battle Instance
            CommandHandeler.GetCommand("Battle_Participants_Get").SetRateLimit(true, 10);
            CommandHandeler.GetCommand("Battle_Participants_Get").Command_Executed += Battle_Participants_Get_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_TurnOrder_Get");// Gets the Turn Order of this Battle Instance
            CommandHandeler.GetCommand("Battle_TurnOrder_Get").SetRateLimit(true, 180);
            CommandHandeler.GetCommand("Battle_TurnOrder_Get").Command_Executed += Battle_TurnOrder_Get_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_TurnOrder_Current");// Gets the current turn in the Battle Instance
            CommandHandeler.GetCommand("Battle_TurnOrder_Current").SetRateLimit(true, 180);
            CommandHandeler.GetCommand("Battle_TurnOrder_Current").Command_Executed += Battle_TurnOrder_Current_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_TurnOrder_Next");// Next Turn in the Battle Instance Turn Order
            CommandHandeler.GetCommand("Battle_TurnOrder_Next").SetRateLimit(false);
            CommandHandeler.GetCommand("Battle_TurnOrder_Next").Command_Executed += Battle_TurnOrder_Next_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_TurnOrder_Prev");// Prev Turn in the Battle Instance Turn Order
            CommandHandeler.GetCommand("Battle_TurnOrder_Prev").SetRateLimit(false);
            CommandHandeler.GetCommand("Battle_TurnOrder_Prev").Command_Executed += Battle_TurnOrder_Prev_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_Action_Execute");// Executes a Battle Action
            CommandHandeler.GetCommand("Battle_Action_Execute").SetRateLimit(false);
            CommandHandeler.GetCommand("Battle_Action_Execute").Command_Executed += Battle_Action_Execute_Executed;

            CommandHandeler.RegisterCommand<object>("Battle_Action_Interupt");// Executes a Battle Interupt
            CommandHandeler.GetCommand("Battle_Action_Interupt").SetRateLimit(false);
            CommandHandeler.GetCommand("Battle_Action_Interupt").Command_Executed += Battle_Action_Interupt_Executed;

            CommandHandeler.RegisterCommand<CommandData.Battle.Battle_Instance_List>("Battle_Instance_List");// Starts the Battle Instance
            CommandHandeler.GetCommand("Battle_Instance_List").SetRateLimit(true, 15);
            CommandHandeler.GetCommand("Battle_Instance_List").Command_Executed += Battle_Instance_List_Executed;

            CommandHandeler.RegisterCommand<CommandData.Battle.Battle_Instance>("Battle_Instance_Start");// Starts the Battle Instance
            CommandHandeler.GetCommand("Battle_Instance_Start").SetRateLimit(true);
            CommandHandeler.GetCommand("Battle_Instance_Start").Command_Executed += Battle_Instance_Start_Executed;

            CommandHandeler.RegisterCommand<CommandData.Battle.Battle_Instance>("Battle_Instance_End");// Ends the Battle Instance
            CommandHandeler.GetCommand("Battle_Instance_End").SetRateLimit(true);
            CommandHandeler.GetCommand("Battle_Instance_End").Command_Executed += Battle_Instance_End_Executed;

            // CommandHandeler.RegisterCommand<string>("Battle_");
            #endregion

            #region Resources
            CommandHandeler.RegisterCommand<CommandData.Resources.ImageResource>("Resources_Image_Get");// Gets an Image Resource
            CommandHandeler.GetCommand("Resources_Image_Get").SetRateLimit(false);
            CommandHandeler.GetCommand("Resources_Image_Get").Command_Executed += Resources_Image_Get_Executed;

            CommandHandeler.RegisterCommand<CommandData.Resources.ImageResource>("Resources_Image_Add");// Adds an Image Resource
            CommandHandeler.GetCommand("Resources_Image_Add").SetRateLimit(false);
            CommandHandeler.RegisterCommand<CommandData.Resources.ImageResource>("Resources_Image_Edit");// Edits and Image Resource
            CommandHandeler.GetCommand("Resources_Image_Edit").SetRateLimit(false);
            CommandHandeler.RegisterCommand<CommandData.Resources.ImageResource>("Resources_Image_Remove");// Removes and Image Resource
            CommandHandeler.GetCommand("Resources_Image_Remove").SetRateLimit(false);

            CommandHandeler.RegisterCommand<string>("Resources_Audio_Play");// Play Audio Signal
            CommandHandeler.GetCommand("Resources_Audio_Play").SetRateLimit(false);
            CommandHandeler.RegisterCommand<string>("Resources_Audio_Get");// Gets an Audio Resource
            CommandHandeler.GetCommand("Resources_Audio_Get").SetRateLimit(false);
            CommandHandeler.RegisterCommand<string>("Resources_Audio_Add");// Adds an Audio Resource
            CommandHandeler.GetCommand("Resources_Audio_Add").SetRateLimit(false);
            CommandHandeler.RegisterCommand<string>("Resources_Audio_Edit");// Edits and Audio Resource
            CommandHandeler.GetCommand("Resources_Audio_Edit").SetRateLimit(false);
            CommandHandeler.RegisterCommand<string>("Resources_Audio_Remove");// Removes an Audio Resource
            CommandHandeler.GetCommand("Resources_Audio_Remove").SetRateLimit(false);
            // CommandHandeler.RegisterCommand<string>("Resources_");
            #endregion
        }

        #region Callbacks
        private void ConnectionState_Executed(object Data, TCP_ClientNode Client)
        {

        }

        #region Auth
        private void Auth_Login_Executed(object Data, TCP_ClientNode Client)
        {
            Instance.Authenticate_Client(Client, (CommandData.Auth.Login)Data);
        }
        private void Auth_Logout_Executed(object Data, TCP_ClientNode Client)
        {
            Instance.DeAuthenticate_Client(Client);
        }
        #endregion

        #region Base Commands
        private void Base_SaveData_Save_Executed(object Data, Networking.Server.TCP.TCP_ClientNode Client)
        {
            Instance.SaveManager.Save_SaveData();
            Client.Send(new Instances.CommandData.SaveData.Save() { State = CommandData.SaveData.SaveStates.Save_Passed });
        }
        #endregion

        #region Pokedex
        #region All
        private void Pokedex_Get_Executed(object Data, Networking.Server.TCP.TCP_ClientNode Client)
        {
            List<Pokedex.Pokemon.PokemonData> PokedexData = new List<Pokedex.Pokemon.PokemonData>();

            foreach (Pokedex.Pokemon.PokemonData pokemon in Instance.SaveManager.SaveData.PokedexData.Pokemon)
            {
                // Permissions Checks Here
                PokedexData.Add(pokemon);
            }

            Client.Send(new CommandData.Pokedex.Pokedex_Pokemon_GetList()
            {
                Command = "Pokedex_Get",
                Pokemon_Dex = PokedexData
            });
        }
        #endregion

        #region Pokemon
        private void Pokedex_Pokemon_GetList_Executed(object Data, TCP_ClientNode Client)
        {
            List<Pokedex.Pokemon.PokemonData> PokedexData = new List<Pokedex.Pokemon.PokemonData>();

            foreach (Pokedex.Pokemon.PokemonData pokemon in Instance.SaveManager.SaveData.PokedexData.Pokemon)
            {
                // Permissions Checks Here
                PokedexData.Add(pokemon);
            }

            Client.Send(new CommandData.Pokedex.Pokedex_Pokemon_GetList()
            {
                Command = "Pokedex_Pokemon_GetList",
                Pokemon_Dex = PokedexData
            });
        }
        private void Pokedex_Pokemon_Get_Executed(object Data, TCP_ClientNode Client)
        {
            Pokedex.Pokemon.PokemonData PokemonData = Instance.SaveManager.SaveData.PokedexData.Pokemon.Find(x => x.Species_DexID == ((Pokedex_Pokemon)Data).DexID);

            Client.Send(new Pokedex_Pokemon()
            {
                Command = "Pokedex_Pokemon_Get",
                PokemonData = PokemonData,
                DexID = PokemonData.Species_DexID
            });
        }
        #endregion

        #region Moves
        private void Pokedex_Moves_Get_Executed(object Data, TCP_ClientNode Client)
        {
            List<Pokedex.Moves.MoveData> MoveData = new List<Pokedex.Moves.MoveData>();

            foreach (Pokedex.Moves.MoveData move in Instance.SaveManager.SaveData.PokedexData.Moves)
            {
                // Permissions Checks Here
                MoveData.Add(move);
            }

            Client.Send(new CommandData.Pokedex.Pokedex_Moves_Get()
            {
                Command = "Pokedex_Moves_Get",
                Move_Dex = MoveData
            });
        }
        #endregion
        #endregion

        #region Entities
        private void Entities_All_GetList_Executed(object Data, TCP_ClientNode Client)
        {
            List<EntitiesManager.Entry_Data> Entities = new List<EntitiesManager.Entry_Data>();
            List<EntitiesManager.Folder> Folders = new List<EntitiesManager.Folder>();
            Authentication_Manager.Data.User User = Instance.Authenticated_Clients.Find(x => x.Key.ID == Client.ID).Value;

            foreach (EntitiesManager.Pokemon.PokemonCharacter pokemon in Instance.SaveManager.SaveData.Pokemon)
            {
                if (pokemon.View.Contains(User.UserID) || User.isGM)// Change to check if the user has view permissions on the entry
                {
                    Entities.Add(pokemon.EntryData);
                }
            }
            foreach (EntitiesManager.Trainer.TrainerCharacter trainer in Instance.SaveManager.SaveData.Trainers)
            {
                if (trainer.View.Contains(User.UserID) || User.isGM)// Change to check if the user has view permissions on the entry
                {
                    Entities.Add(trainer.EntryData);
                }
            }

            if (User.isGM)
            {
                Folders = Instance.SaveManager.SaveData.Folders;
            }
            else
            {
                // Get only the folders needed
                foreach (EntitiesManager.Entry_Data entry in Entities)
                {
                    foreach (EntitiesManager.Folder folder in Instance.SaveManager.SaveData.Folders_GetTreeFrom(entry.Parent_Folder))
                    {
                        if (!Folders.Contains(folder))
                        {
                            Folders.Add(folder);
                        }
                    }
                }
            }

            Client.Send(new CommandData.Entities.Entities_All_GetList()
            {
                Entrys = Entities,
                Folders = Folders,
                UserList = Instance.SaveManager.SaveData.AuthManager.Users_List()
            });
        }

        private void Entities_Pokemon_Get_Executed(object Data, TCP_ClientNode Client)
        {
            CommandData.Entities.Entities_Pokemon_Get Pokemon = (CommandData.Entities.Entities_Pokemon_Get)Data;
            Authentication_Manager.Data.User User = Instance.Authenticated_Clients.Find(x => x.Key.ID == Client.ID).Value;

            Pokemon.Pokemon = Instance.SaveManager.SaveData.Pokemon.Find(x => x.ID == Pokemon.ID);
            Pokemon.Image = Instance.SaveManager.LoadImage(Pokemon.Pokemon.Token_ResourceID);


            if (Pokemon.Pokemon.View.Contains(User.UserID) || User.isGM)
                Client.Send(Pokemon);
        }
        #endregion

        #region Battles
        private void Battle_Participants_Add_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_Participants_Edit_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_Participants_Remove_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_Participants_Get_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_TurnOrder_Get_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_TurnOrder_Current_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_TurnOrder_Next_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_TurnOrder_Prev_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_Action_Execute_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_Action_Interupt_Executed(object Data, TCP_ClientNode Client)
        {

        }

        private void Battle_Instance_List_Executed(object Data, TCP_ClientNode Client)
        {
            Client.Send(new CommandData.Battle.Battle_Instance_List() { /*Instances = Instance.GetInstances*/ });
        }

        private void Battle_Instance_Start_Executed(object Data, TCP_ClientNode Client)
        {
            //Client.Send(new CommandData.Battle.Battle_Instance() { Command = "", });
        }

        private void Battle_Instance_End_Executed(object Data, TCP_ClientNode Client)
        {

        }
        #endregion

        #region Resources
        private void Resources_Image_Get_Executed(object Data, TCP_ClientNode Client)
        {
            CommandData.Resources.ImageResource IRD = (CommandData.Resources.ImageResource)Data;
            IRD.Image = Instance.SaveManager.LoadImage(IRD.Resource_ID);

            Client.Send(IRD);
        }
        #endregion
        #endregion

        public void Unregister_Commands(Networking.Server.Command_Handeler.Server_CommandHandeler CommandHandeler)
        {

        }
    }
}
