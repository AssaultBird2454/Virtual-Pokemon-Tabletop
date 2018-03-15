using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultBird2454.VPTU.SaveManager.Data.SaveFile
{
    /// <summary>
    /// A Save Data Class designed to handle the save data
    /// </summary>
    public class PTUSaveData
    {
        /// <summary>
        /// Defines where the SaveManager is. This allows access to query the database.
        /// </summary>
        SaveManager Manager;
        /// <summary>
        /// Creates a new PTUSaveData class
        /// </summary>
        /// <param name="InitNewSave">Initilises all objects</param>
        public PTUSaveData(SaveManager _Manager)
        {
            Manager = _Manager;

            Campaign_Data = new Campaign_Data(Manager);
            AuthManager = new Auth(Manager);
        }

        #region Data
        public Data.Campaign_Data Campaign_Data;
        public Data.Campaign_Settings Campaign_Settings;
        public Data.Server_Settings Server_Settings;
        #endregion

        #region Auth and Perms
        public Auth AuthManager;
        #endregion

        #region Battles
        public BattleManager.Typing.Manager Typing_Manager;
        #endregion

        public Pokedex.Save_Data.Pokedex PokedexData;

        #region Entities Data
        public List<EntitiesManager.Folder> Folders;
        public List<EntitiesManager.Trainer.TrainerCharacter> Trainers;
        public List<EntitiesManager.Pokemon.PokemonCharacter> Pokemon;

        /// <summary>
        /// Helper Function, This function will return the tree of folders to get to the child folder specified
        /// </summary>
        /// <param name="Child">The ID of the folder that is trying to be retrieved</param>
        /// <returns>List of folders to the desired child folder</returns>
        public List<EntitiesManager.Folder> Folders_GetTreeFrom(string Child)
        {
            List<EntitiesManager.Folder> list;
            EntitiesManager.Folder folder = Folders.Find(x => x.ID == Child);

            if (folder.Parent == null)
            {
                list = new List<EntitiesManager.Folder>();
            }
            else
            {
                list = Folders_GetTreeFrom(folder.Parent);
            }

            list.Add(folder);

            return list;
        }
        #endregion

        #region Tabletop
        //public List<Resources.MapFileData> MapFiles;
        //public List<Resources.MapData> Maps;
        #endregion
        #region Resources
        public List<SoundSystem.SaveData.AudioData> AudioResources;
        public List<Resource_Data.Resources> ImageResources;
        #endregion

        #region Settings

        #endregion
    }
}
