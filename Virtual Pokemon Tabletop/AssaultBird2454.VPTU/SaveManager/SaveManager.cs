using AssaultBird2454.VPTU.SaveManager.Resource_Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssaultBird2454.VPTU.SaveManager
{
    public enum SaveData_Dir { Pokedex_Pokemon, Pokedex_Moves, Pokedex_Abilitys, Pokedex_Items, Resource_Image, Entities_Pokemon, Entities_Trainers, Entities_Folder, Server_Settings, Basic_CampaignInfo, Basic_CampaignSettings, Auth_Users, Auth_Groups, Auth_Identities, Auth_Permissions, Battle_Typing }

    public class No_Data_Found_In_Save_Exception : Exception { }

    public class SaveManager
    {
        #region Variables
        public string SaveFileDir { get; }
        /// <summary>
        /// A Save Data Object for use inside the software, This is Saved to the file when Save_SaveData() is called
        /// </summary>
        public Data.SaveFile.PTUSaveData SaveData { get; set; }

        public System.Data.SQLite.SQLiteConnection Connection { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new instance of a Save File Manager
        /// </summary>
        /// <param name="SelectedSaveFile">The Directory of the save file that will be used</param>
        public SaveManager(string SelectedSaveFile)
        {
            SaveFileDir = SelectedSaveFile;// Sets the property containing the Save File Directory
        }

        /// <summary>
        /// Load save data to save data class ready for use
        /// </summary>
        /// <returns>Save Data</returns>
        public void Load_SaveData()
        {
            try
            {
                Connection = new System.Data.SQLite.SQLiteConnection("Data Source=" + SaveFileDir + ";");
                Connection.Open();

                SaveData = new Data.SaveFile.PTUSaveData(this);
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error while loading the save file...\nPlease confirm that the savefile has no errors...", "Save file loading error");
                MessageBox.Show(e.ToString());
            }
        }
        /// <summary>
        /// Saves save data to save file
        /// </summary>
        public void Save_SaveData()
        {
            // Save
        }

        #region Load and Save
        /// <summary>
        /// Loads Data from the save file
        /// </summary>
        /// <typeparam name="T">Type of data being loaded</typeparam>
        /// <param name="SaveFile_DataDir">The Directory where the data should be loaded from in the save file</param>
        /// <returns>Loaded Object</returns>
        public T LoadData_FromSave<T>(string SaveFile_DataDir)
        {
            return default(T);
        }
        /// <summary>
        /// Saves Data to the save file
        /// </summary>
        /// <param name="SaveFile_DataDir">The Directory where the data should be saved in the file</param>
        /// <param name="Object">Object to save</param>
        public void SaveData_ToSave(string SaveFile_DataDir, object Object)
        {
            
        }
        /// <summary>
        /// Gets the Path to a specific file in the Save Data Structure (Excluding Resources or Save Files from Plugins)
        /// </summary>
        /// <param name="DirType">File requested</param>
        /// <returns>Path to file</returns>
        public static string GetSaveFile_DataDir(SaveData_Dir DirType)
        {
            switch (DirType)
            {
                case SaveData_Dir.Pokedex_Pokemon:
                    return "Pokedex/Pokemon.json";
                case SaveData_Dir.Pokedex_Moves:
                    return "Pokedex/Moves.json";
                case SaveData_Dir.Pokedex_Abilitys:
                    return "Pokedex/Abilitys.json";
                case SaveData_Dir.Pokedex_Items:
                    return "Pokedex/Items.json";
                case SaveData_Dir.Resource_Image:
                    return "Resource/Data.json";
                case SaveData_Dir.Entities_Pokemon:
                    return "Entities/Pokemon.json";
                case SaveData_Dir.Entities_Trainers:
                    return "Entities/Trainers.json";
                case SaveData_Dir.Entities_Folder:
                    return "Entities/Folders.json";
                case SaveData_Dir.Basic_CampaignInfo:
                    return "CampaignInfo.json";
                case SaveData_Dir.Basic_CampaignSettings:
                    return "Settings.json";
                case SaveData_Dir.Server_Settings:
                    return "Server_Settings.json";
                case SaveData_Dir.Auth_Users:
                    return "Auth/Users.json";
                case SaveData_Dir.Auth_Groups:
                    return "Auth/Groups.json";
                case SaveData_Dir.Auth_Identities:
                    return "Auth/Identities.json";
                case SaveData_Dir.Auth_Permissions:
                    return "Auth/Permissions.json";
                case SaveData_Dir.Battle_Typing:
                    return "Battle/TypingData.json";

                default:
                    return null;
            }
        }
        #endregion

        #region Resource
        #region Import & Export into SaveFile
        /// <summary>
        /// Adds a file to the internal save file
        /// </summary>
        /// <param name="FilePath">Path to the file</param>
        /// <param name="Name">The name of the file inside the save file.</param>
        public void ImportFile(string FilePath, string Name)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        public void ExportFile()
        {

        }
        public void Delete_Resource(string ID)
        {
            
        }

        /// <summary>
        /// Checks if the file exists in the save file
        /// </summary>
        /// <param name="FileName">The Path and File to check</param>
        /// <returns>If it exists or not</returns>
        public bool FileExists(string FileName)
        {
            using (FileStream stream = new FileStream(SaveFileDir, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update))
                {
                    try
                    {
                        ZipArchiveEntry entry = archive.GetEntry(FileName);
                        if (entry != null)
                        {
                            return true;
                        }
                    }
                    catch { /* Dont Care... May Throw an exception if a file does not exist */ }
                }
            }
            return false;
        }
        #endregion
        #region Load File
        public Bitmap LoadImage(string ID)
        {
            return new Bitmap(10, 10);
        }
        #endregion
        #endregion
    }
}
