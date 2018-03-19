using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AssaultBird2454.VPTU.Authentication_Manager.Data;
using AssaultBird2454.VPTU.EntitiesManager;
using AssaultBird2454.VPTU.EntitiesManager.Pokemon;
using AssaultBird2454.VPTU.Pokedex.Moves;
using AssaultBird2454.VPTU.Pokedex.Pokemon;
using AssaultBird2454.VPTU.RNG.Generators;
using AssaultBird2454.VPTU.SaveEditor.UI;
using AssaultBird2454.VPTU.SaveEditor.UI.About;
using AssaultBird2454.VPTU.SaveEditor.UI.Entities;
using AssaultBird2454.VPTU.SaveEditor.UI.Pokedex;
using AssaultBird2454.VPTU.SaveEditor.UI.Resources;
using AssaultBird2454.VPTU.SaveEditor.UI.Users;
using AssaultBird2454.VPTU.SaveManager;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AssaultBird2454.VPTU.SaveEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void PokedexManager_Export_Pokemon_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PokedexManager_Export_Moves_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OverViewSettings_Server_Address_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveManager.SaveData.Campaign_Data.Server_Address = OverViewSettings_Server_Address.Text;
        }

        private void OverViewSettings_Server_Port_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                SaveManager.SaveData.Campaign_Data.Server_Port = (int)OverViewSettings_Server_Port.Value;
            }
            catch
            {
            }
        }

        #region Form Code

        public static SaveManager.SaveManager SaveManager;
        public static bool DeveloperMode { get; private set; }
        public ProjectInfo VersioningInfo;

        /// <summary>
        ///     Assembly Directory
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public MainWindow()
        {
            try
            {
                if (File.Exists(AssemblyDirectory + "\\SaveEditor.pid"))
                    if (Process.GetProcessById(
                            Convert.ToInt32(File.ReadAllText(AssemblyDirectory + "\\SaveEditor.pid"))).ProcessName ==
                        Process.GetCurrentProcess().ProcessName)
                    {
                        MessageBox.Show("Process Already Running!");
                        Close();
                        return;
                    }
                    else
                    {
                        File.Delete(AssemblyDirectory + "\\SaveEditor.pid");
                    }

                File.WriteAllText(AssemblyDirectory + "\\SaveEditor.pid", Process.GetCurrentProcess().Id.ToString());
            }
            catch
            {
            }

            InitializeComponent();
            Create_EntitiesManager_ContextMenu();

            Setup();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.Delete(AssemblyDirectory + "\\SaveEditor.pid");
        }

        private void Menu_About_Licence_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var fs = new FileStream(AssemblyDirectory + "/LICENSE", FileMode.Open))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        var lic = new License(sr.ReadToEnd(), "License");
                        lic.ShowDialog();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "The \"LICENSE\" File was not found when trying to read it! The \"LICENSE\" file is avalable on GitHub.",
                    "License File Missing");
            }
        }

        private void Menu_About_LegalNotices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var fs = new FileStream(AssemblyDirectory + "/LEGAL NOTICE", FileMode.Open))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        var lic = new License(sr.ReadToEnd(), "Legal Notices");
                        lic.ShowDialog();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "The \"LEGAL NOTICE\" File was not found when trying to read it! The \"LEGAL NOTICE\" file is avalable on GitHub.",
                    "License File Missing");
            }
        }

        #region Setup Code

        private void Setup()
        {
            #region Developer Mode
            #region Check
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                DeveloperMode = true;
            else
                DeveloperMode = false;
            #endregion
            #region Show Dev Tools

            #endregion
            #endregion

            #region Versioning Info
            using (var str = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("AssaultBird2454.VPTU.SaveEditor.ProjectVariables.json"))
            {
                using (var read = new StreamReader(str))
                {
                    VersioningInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectInfo>(read.ReadToEnd());
                    Title = "Virtual Pokemon Tabletop - SaveEditor (Version: " + VersioningInfo.Version +
                            ") (Commit: " + VersioningInfo.Compile_Commit.Remove(7) + ")";

                    if (DeveloperMode)
                        Title += " -> Developer Mode";
                }
            }
            #endregion
        }

        #endregion

        #region Help

        private void Menu_Help_Forums_Click(object sender, RoutedEventArgs e)
        {
            var mbr = MessageBox.Show(
                "Would you like to open a web browser to view the forums?\n\nWeb URL: http://forums.pokemontabletop.com",
                "Open Forums?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start("http://forums.pokemontabletop.com");
        }

        private void Menu_Help_PTUSystem_105_Pokedex_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Pokedex 1.05.pdf";
            var mbr = MessageBox.Show("Would you like to open the PTU 1.05 Pokedex document?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_I_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 1 (Introduction).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 1 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_CC_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 2 (Character Creation).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 2 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_SEaF_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 3 (Skills, Edges And Features).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 3 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_TC_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 4 (Trainer Classes).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 4 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_P_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 5 (Pokemon).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 5 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_PtG_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 6 (Playing the Game).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 6 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_C_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 7 (Combat).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 7 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_PC_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 8 (Pokemon Contests).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 8 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_GaI_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 9 (Gear and Items).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 9 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_IaR_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 10 (Indices and Reference).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 10 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        private void Menu_Help_PTUSystem_105_RtG_Click(object sender, RoutedEventArgs e)
        {
            var dir = AssemblyDirectory + @"\Docs\PTU System 1.05\Chapter 11 (Running the Game).pdf";
            var mbr = MessageBox.Show(
                "Would you like to open the PTU 1.05 core document (Chapter 11 Only)?\n\nFile URI: " + dir,
                "Open Document?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (mbr == MessageBoxResult.Yes)
                Process.Start(dir);
        }

        #endregion

        #endregion

        #region Save Manager Related Code

        #region Triggering Events

        //When The "Open File" Button is clicked
        private void Menu_Menu_Open_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.CheckFileExists = true;
            openFile.CheckPathExists = true;
            openFile.Multiselect = false;
            openFile.Title = "Open Virtual PTU Save File";
            openFile.DefaultExt = ".ptu";

            openFile.FileOk += (obj, args) => { Load(openFile.FileName); };

            openFile.ShowDialog();
        }

        //When The "Save File" Button is clicked
        private void Menu_Menu_Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        //When The "Save File As" Button is clicked
        private void Menu_Menu_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is not currently working...");
        }

        #endregion

        /// <summary>
        ///     Saves the Save Data to Save File
        /// </summary>
        public void Save()
        {
            if (SaveManager == null)
            {
                MessageBox.Show("No File is open... Please open a file before saving");
                return;
            }

            SaveManager.Save_SaveData();
        }

        /// <summary>
        ///     Loads a Save File
        /// </summary>
        /// <param name="Path">Path to Save file to be opened</param>
        public void Load(string Path)
        {
            SaveManager = new SaveManager.SaveManager(Path);
            try
            {
                SaveManager.Load_SaveData();
            }
            catch
            {
                return;
            }
            SaveEditor_TabPanel.IsEnabled = true;

            OverViewSettings_Reload(); // Reload Settings and other info
            //PokedexManager_ReloadList(); //Reload Pokedex List
            //ResourceManager_ReloadList(); //Reload Resource List
            //EntitiesManager_ReloadList(); // Reload Characters List
            UserGroup_Users_Reload(); // Reload Users List
        }

        #region Save Data Tools

        private void Menu_SaveTools_UnPack_Click(object sender, RoutedEventArgs e)
        {
            if (SaveManager != null)
            {
                var mbr = MessageBox.Show(
                    "To Un-Pack the open Save Data File, The data needs to be saved...\nSave and Un-Pack?",
                    "Select Save to Un-Pack", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (mbr == MessageBoxResult.Yes)
                {
                    Save(); // Save the Save File

                    var DataPath = Path.GetDirectoryName(SaveManager.SaveFileDir) + @"\" +
                                   Path.GetFileName(SaveManager.SaveFileDir)
                                       .Split('.')[0]; // Gets the path to extract the save data

                    if (Directory.Exists(DataPath)) // Checks if path exists
                    {
                        //If Path Exists, Notify the user that the data will be deleted and offer the ability to acknoledge or cancel the opperation
                        var mbed = MessageBox.Show(
                            "The Path that the save data will be extracted to already exists...\nAll Data in the folder will be deleted!\n\nDirectory: " +
                            DataPath, "Path Exists", MessageBoxButton.OKCancel, MessageBoxImage.Warning,
                            MessageBoxResult.Cancel);
                        if (mbed == MessageBoxResult.Cancel)
                        {
                            //Cancel the opperation
                            MessageBox.Show(
                                "Un-Packing Save Data was canceled and no files have been changed.\n\nReasion: Canceled By User",
                                "Un-Pack Save Data Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        if (mbed == MessageBoxResult.OK)
                            Directory.Delete(DataPath, true);
                    }

                    SaveFileConverter.Extract_Save(SaveManager.SaveFileDir,
                        DataPath); // Extract the data to the Directory

                    //Ask if the user wants the folder to be opened...
                    var mbr2 = MessageBox.Show("Un-Packing Complete!\nOpen Save Data?\n\nPath to Data: " + DataPath,
                        "Un-Pack Complete", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);
                    if (mbr2 == MessageBoxResult.Yes)
                        Process.Start("Explorer.exe", DataPath);
                }
            }
            else
            {
                //No Save file open, Cant Un-Pack Anything
                MessageBox.Show("You cant Un-Pack nothing!\nOpen a Save File and try again", "Un-Packing Save Data");
            }
        }

        private void Menu_SaveTools_Pack_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog(); // creates a FolderBrowser dialog
            var dr = fbd.ShowDialog(); // Shows the dialog to select the path containing save data

            if (dr == System.Windows.Forms.DialogResult.Cancel) // Check if Pack Opperation was canceled
            {
                MessageBox.Show(
                    "Packing Save Data was canceled and no files have been changed.\n\nReasion: Canceled By User",
                    "Pack Save Data Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var DataPath = fbd.SelectedPath; // Gets the path to the the save data to pack from
            var SavePath = Directory.GetParent(DataPath) + @"\Packed Pokemon Tabletop Save.ptu";

            if (!Directory.Exists(DataPath)) // Checks if path does not exist
            {
                //If Path does not exist, Notify the user that the selected path does not exist
                var mbed = MessageBox.Show("The Path specified does not exist...\n\nSelected Directory: " + DataPath,
                    "Path does not Exist", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            SaveFileConverter.Compress_Save(SavePath, DataPath); // Pack the data to the a file

            //Ask if the user wants the folder to be opened...
            var mbr2 = MessageBox.Show("Packing Complete!\nOpen SaveFile?\n\nPath to File: " + SavePath,
                "Packing Complete", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);
            if (mbr2 == MessageBoxResult.Yes)
                Load(SavePath);
        }

        #endregion

        #endregion

        #region Pokedex Manager Code

        #region Pokedex Manager Variables

        private Thread PokedexSearchThread;

        #endregion

        #region Right SideBar Events

        //When The "Add Pokemon" Button is clicked
        private void PokedexManager_AddDex_Pokemon_Click(object sender, RoutedEventArgs e)
        {
            var pokemon = new Pokemon(SaveManager.SaveData.PokedexData); // Creates Pokemon Editor Page
            var OK = pokemon.ShowDialog(); // Shows the dialog, waits for return

            if (OK == true) // When Return
            {
                SaveManager.SaveData.PokedexData.Pokemon_Add(pokemon.PokemonData); // Add Pokemon to List
                PokedexManager_ReloadList(); // Reload Pokedex List
            }
        }

        //When The "Add Move" Button is clicked
        private void PokedexManager_AddDex_Move_Click(object sender, RoutedEventArgs e)
        {
            var move = new Moves(SaveManager.SaveData); // Creates Move Editor Page
            var OK = move.ShowDialog(); // Shows the Dialog, waits for return

            if (OK == true) // When Return
            {
                SaveManager.SaveData.PokedexData.Moves.Add(move.MoveData); // Add Move to List
                PokedexManager_ReloadList(); // Reload Pokedex List
            }
        }

        //When The "Add Ability" Button is clicked
        private void PokedexManager_AddDex_Ability_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not Available for that Data Type.");
        }

        //When The "Add Item" Button is clicked
        private void PokedexManager_AddDex_Items_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature not Available for that Data Type.");
        }

        //When The "Edit" Button is clicked
        private void PokedexManager_ManageDex_Edit_Click(object sender, RoutedEventArgs e)
        {
            EditSelected_Pokedex();
        }

        //When an item on the list is double clicked
        private void PokedexManager_List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelected_Pokedex();
        }

        //When The "Delete" Button is clicked
        private void PokedexManager_ManageDex_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((PokedexList_DataBind)PokedexManager_List.SelectedValue).DataType == PokedexList_DataType.Pokemon)
                {
                    var Data = (PokemonData)((PokedexList_DataBind)PokedexManager_List.SelectedValue).DataTag;
                    SaveManager.SaveData.PokedexData.Pokemon_Remove(Data);

                    Data.Dispose();
                    Data = null;
                }
                else if (((PokedexList_DataBind)PokedexManager_List.SelectedValue).DataType ==
                         PokedexList_DataType.Move)
                {
                    var Data = (MoveData)((PokedexList_DataBind)PokedexManager_List.SelectedValue).DataTag;
                    SaveManager.SaveData.PokedexData.Moves.Remove(Data);

                    #region Remove Links to the move

                    #region Pokemon

                    foreach (var pokemon in SaveManager.SaveData.PokedexData.Pokemon_List())
                    {
                        if (pokemon.Moves == null)
                        {
                            pokemon.Moves = new List<Link_Moves>();
                            continue;
                        }
                        var moves = pokemon.Moves.FindAll(x => x.MoveName.ToLower() == Data.Name.ToLower());
                        foreach (var move in moves)
                            pokemon.Moves.Remove(move);
                    }

                    #endregion

                    #endregion

                    Data.Dispose();
                    Data = null;
                }

                PokedexManager_ReloadList();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("You cant delete nothing! or can you?");
            }
        }

        //When The Search Box has been changed
        private void PokedexManager_SearchDex_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        #endregion

        #region List Events

        private void PokedexManager_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        //All of these events will reload the Pokedex List
        private void PokedexManager_SearchDex_Pokemon_Checked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        private void PokedexManager_SearchDex_Pokemon_Unchecked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        private void PokedexManager_SearchDex_Moves_Checked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        private void PokedexManager_SearchDex_Moves_Unchecked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        private void PokedexManager_SearchDex_Abilitys_Checked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        private void PokedexManager_SearchDex_Abilitys_Unchecked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        private void PokedexManager_SearchDex_Items_Checked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        private void PokedexManager_SearchDex_Items_Unchecked(object sender, RoutedEventArgs e)
        {
            PokedexManager_ReloadList();
        }

        #endregion

        /// <summary>
        ///     Reloads the Pokedex list
        /// </summary>
        public void PokedexManager_ReloadList()
        {
            try
            {
                PokedexSearchThread.Abort();
                PokedexSearchThread = null;
            }
            catch
            {
            }

            PokedexSearchThread = new Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        PokedexManager_List.Items.Clear();

                        if (SaveManager == null) return;

                        if (PokedexManager_SearchDex_Pokemon.IsChecked == true)
                            foreach (var Pokemon in SaveManager.SaveData.PokedexData.Pokemon_List())
                                if (Pokemon.Species_Name.ToLower()
                                    .Contains(PokedexManager_SearchDex_Search.Text.ToLower()))
                                {
                                    var PokemonDB = new PokedexList_DataBind();
                                    PokemonDB.Name = Pokemon.Species_Name;
                                    PokemonDB.ID = Pokemon.Species_DexID;
                                    PokemonDB.Class = "";
                                    PokemonDB.EntryType = "Pokemon";
                                    //PokemonDB.Type = "";
                                    PokemonDB.DataType = PokedexList_DataType.Pokemon;
                                    PokemonDB.DataTag = Pokemon;

                                    PokedexManager_List.Items.Add(PokemonDB);
                                }

                        if (PokedexManager_SearchDex_Moves.IsChecked == true)
                            foreach (var Move in SaveManager.SaveData.PokedexData.Moves)
                                if (Move.Name.ToLower().Contains(PokedexManager_SearchDex_Search.Text.ToLower()))
                                {
                                    var MoveDB = new PokedexList_DataBind();
                                    MoveDB.Name = Move.Name;
                                    MoveDB.ID = SaveManager.SaveData.PokedexData.Moves.IndexOf(Move) + 1;
                                    //MoveDB.Type = Move.Move_Type.ToString();
                                    MoveDB.Class = Move.Move_Class.ToString();
                                    MoveDB.EntryType = "Move";

                                    MoveDB.DataType = PokedexList_DataType.Move;
                                    MoveDB.DataTag = Move;

                                    PokedexManager_List.Items.Add(MoveDB);
                                }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An Error occured while loading the Pokedex Manager's List!\n\n" + ex,
                            "Pokedex Manager - Loading Error", MessageBoxButton.OK, MessageBoxImage.Error,
                            MessageBoxResult.OK);
                    }
                });
            });
            PokedexSearchThread.IsBackground = true;
            PokedexSearchThread.Start();
        }

        /// <summary>
        ///     Opens the edit page for the selected item in the pokedex panel
        /// </summary>
        public void EditSelected_Pokedex()
        {
            try
            {
                //Edit Pokemon Here!
                if (((PokedexList_DataBind)PokedexManager_List.SelectedValue).DataType == PokedexList_DataType.Pokemon)
                {
                    var Data = (PokemonData)((PokedexList_DataBind)PokedexManager_List.SelectedValue)
                        .DataTag; // Gets the Data
                    var pokemon = new Pokemon(SaveManager.SaveData.PokedexData, Data); // Creates a new window
                    pokemon.ShowDialog(); // Shows the window

                    PokedexManager_ReloadList(); // Updates the list
                }
                //Edit Moves Here!
                else if (((PokedexList_DataBind)PokedexManager_List.SelectedValue).DataType ==
                         PokedexList_DataType.Move)
                {
                    var Data = (MoveData)((PokedexList_DataBind)PokedexManager_List.SelectedItem)
                        .DataTag; // Gets the Data
                    var move = new Moves(SaveManager.SaveData, Data); // Creates a new window
                    move.ShowDialog(); // Shows the window

                    PokedexManager_ReloadList(); // Updates the list
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("You cant edit nothing! or can you?");
            }
        }

        #endregion

        #region Resource Manager Code

        #region Resource Variables

        private Thread ResourceSearchThread;

        #endregion

        #region Right SideBar Events

        /// <summary>
        ///     Add Audio Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceManager_AddRes_Audio_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        ///     Add Image Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceManager_AddRes_Images_Click(object sender, RoutedEventArgs e)
        {
            var imp = new Import_ImageResource();
            var pass = imp.ShowDialog();

            if (pass == true)
                ResourceManager_ReloadList();
        }

        /// <summary>
        ///     Edit Resource Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceManager_ManageRes_Edit_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        ///     Delete Resource
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceManager_ManageRes_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ResourceManager_List.SelectedItem != null)
            {
                SaveManager.Resource_Data.Resources res = (SaveManager.Resource_Data.Resources)((System.Windows.Controls.ListViewItem)ResourceManager_List.SelectedItem).Content;

                SaveManager.Delete_Resource(res.ID);
                SaveManager.SaveData.ImageResources.Remove(res);
                ResourceManager_List.Items.Remove(ResourceManager_List.SelectedItem);
            }
        }

        private void ResourceManager_SearchRes_Images_Checked(object sender, RoutedEventArgs e)
        {
            ResourceManager_ReloadList();
        }

        private void ResourceManager_SearchRes_Audio_Checked(object sender, RoutedEventArgs e)
        {
            ResourceManager_ReloadList();
        }

        private void ResourceManager_SearchRes_Audio_Unchecked(object sender, RoutedEventArgs e)
        {
            ResourceManager_ReloadList();
        }

        private void ResourceManager_SearchRes_Images_Unchecked(object sender, RoutedEventArgs e)
        {
            ResourceManager_ReloadList();
        }

        /// <summary>
        ///     Search for names that contain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceManager_SearchRes_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ResourceSearchThread.Abort();
                ResourceSearchThread = null;
            }
            catch
            {
            }

            ResourceSearchThread = new Thread(() =>
            {
                ResourceManager_List.Dispatcher.Invoke(() => ResourceManager_ReloadList());
            });
            ResourceSearchThread.IsBackground = true;
            ResourceSearchThread.Start();
        }

        #endregion

        /// <summary>
        ///     Reloads the Resource List
        /// </summary>
        public void ResourceManager_ReloadList()
        {
            try
            {
                ResourceManager_List.Items.Clear();

                if (ResourceManager_SearchRes_Images.IsChecked == true)
                {
                    foreach (var res in SaveManager.SaveData.ImageResources)
                    {
                        if (res.Name.ToLower().Contains(ResourceManager_SearchRes_Search.Text.ToLower()))
                        {
                            System.Windows.Controls.ListViewItem lvi = new System.Windows.Controls.ListViewItem();
                            lvi.Content = res;
                            lvi.ToolTip = new StackPanel();
                            lvi.ToolTipClosing += new ToolTipEventHandler((sender, e) =>
                            {
                                ((StackPanel)lvi.ToolTip).Children.Clear();
                            });

                            lvi.ToolTipOpening += new ToolTipEventHandler((sender, e) =>
                            {
                                if (res.Type == VPTU.SaveManager.Resource_Data.Resource_Type.Image)
                                {
                                    ((StackPanel)lvi.ToolTip).Children.Clear();

                                    ((StackPanel)lvi.ToolTip).Children.Add(new Image()
                                    {
                                        Source = Imaging.CreateBitmapSourceFromHBitmap(SaveManager.LoadImage(res.ID).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                                        MaxHeight = 150,
                                        MaxWidth = 150,
                                        Stretch = Stretch.Uniform
                                    }
                                    );

                                    ((StackPanel)lvi.ToolTip).Children.Add(
                                    new TextBlock()
                                    {
                                        Text = "ID: " + res.ID + "\nName: " + res.Name
                                    }
                                    );
                                }
                                else if (res.Type == VPTU.SaveManager.Resource_Data.Resource_Type.Audio)
                                {
                                    /* Load Audio Preview ToolTip */
                                }
                            });
                            ResourceManager_List.Items.Add(lvi);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Entities Manager Code

        private readonly List<TreeViewItem> EntitiesManager_Folders = new List<TreeViewItem>();
        private readonly List<TreeViewItem> EntitiesManager_Entrys = new List<TreeViewItem>();
        private ContextMenu EntitiesManager_Root;

        public void EntitiesManager_ReloadList()
        {
            EntitiesManager_Folders.Clear();
            EntitiesManager_Entrys.Clear();
            EntitiesManager_Tree.Items.Clear();

            EntitiesManager_Display();
        }

        public void Create_EntitiesManager_ContextMenu()
        {
            #region Root

            EntitiesManager_Root = new ContextMenu();
            var ctxm_Root_CreateFolder = new MenuItem();
            ctxm_Root_CreateFolder.Header = "Create Folder in Root";
            ctxm_Root_CreateFolder.Click += Ctxm_Root_CreateFolder_Click;
            var ctxm_Root_CreatePokemonEntities = new MenuItem();
            ctxm_Root_CreatePokemonEntities.Header = "Create Pokemon Entities in Root";
            ctxm_Root_CreatePokemonEntities.Click += Ctxm_Root_CreatePokemonEntities_Click;
            var ctxm_Root_CreateTrainerEntities = new MenuItem();
            ctxm_Root_CreateTrainerEntities.Header = "Create Trainer Entities in Root";
            ctxm_Root_CreateTrainerEntities.Click += Ctxm_Root_CreateTrainerEntities_Click;
            ctxm_Root_CreateTrainerEntities.IsEnabled = false;

            EntitiesManager_Root.Items.Add(ctxm_Root_CreateFolder);
            EntitiesManager_Root.Items.Add(ctxm_Root_CreatePokemonEntities);
            EntitiesManager_Root.Items.Add(ctxm_Root_CreateTrainerEntities);
            EntitiesManager_Tree.ContextMenu = EntitiesManager_Root;

            #endregion
        }

        #region Context Menu Events

        private void Ctxm_Entities_Delete_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;

            EntitiesManager_DeleteEntry(((Entry_Data)ctxm.Tag).ID);
        }

        private void Ctxm_Entities_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;
        }

        private void Ctxm_Entities_Edit_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;

            EntitiesManager_EditPokemonEntities(
                SaveManager.SaveData.Pokemon.Find(x => x.ID == ((Entry_Data)ctxm.Tag).ID));
        }

        private void Ctxm_Folder_Delete_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;

            EntitiesManager_DeleteDir(((Folder)ctxm.Tag).ID);
        }

        private void Ctxm_Folder_CreatePokemonEntities_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;

            EntitiesManager_CreatePokemonEntities(((Folder)ctxm.Tag).ID);
        }

        private void Ctxm_Folder_CreateTrainerEntities_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;
        }

        private void Ctxm_Folder_CreateFolder_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;

            var SP = new String_Prompt("Folder Name");
            var Pass = SP.ShowDialog();

            if (Pass == true)
                EntitiesManager_CreateDir(SP.Input, ((Folder)ctxm.Tag).ID);
        }

        private void Ctxm_Root_CreatePokemonEntities_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;

            EntitiesManager_CreatePokemonEntities();
        }

        private void Ctxm_Root_CreateTrainerEntities_Click(object sender, RoutedEventArgs e)
        {
            var ctxm = (ContextMenu)((MenuItem)sender).Parent;
        }

        private void Ctxm_Root_CreateFolder_Click(object sender, RoutedEventArgs e)
        {
            var SP = new String_Prompt("Folder Name");
            var Pass = SP.ShowDialog();

            if (Pass == true)
                EntitiesManager_CreateDir(SP.Input);
        }

        #endregion

        public void EntitiesManager_CreateDir(string Name, string Parent = null)
        {
            var folder = new Folder
            {
                ID = RSG.GenerateString(16),
                Name = Name,
                Parent = Parent
            };

            #region Context Menu

            var EntitiesManager_Folder = new ContextMenu();
            EntitiesManager_Folder.Tag = folder;
            var ctxm_Folder_CreateFolder = new MenuItem();
            ctxm_Folder_CreateFolder.Header = "Create Folder";
            ctxm_Folder_CreateFolder.Click += Ctxm_Folder_CreateFolder_Click;
            var ctxm_Folder_CreatePokemonEntities = new MenuItem();
            ctxm_Folder_CreatePokemonEntities.Header = "Create Pokemon Entities";
            ctxm_Folder_CreatePokemonEntities.Click += Ctxm_Folder_CreatePokemonEntities_Click;
            var ctxm_Folder_CreateTrainerEntities = new MenuItem();
            ctxm_Folder_CreateTrainerEntities.Header = "Create Trainer Entities";
            ctxm_Folder_CreateTrainerEntities.Click += Ctxm_Folder_CreateTrainerEntities_Click;
            ctxm_Folder_CreateTrainerEntities.IsEnabled = false;
            var ctxm_Folder_S1 = new Separator();
            var ctxm_Folder_Delete = new MenuItem();
            ctxm_Folder_Delete.Header = "Delete";
            ctxm_Folder_Delete.Click += Ctxm_Folder_Delete_Click;

            EntitiesManager_Folder.Items.Add(ctxm_Folder_CreateFolder);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_CreatePokemonEntities);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_CreateTrainerEntities);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_S1);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_Delete);

            #endregion

            if (Parent != null)
            {
                var ParentTVI = EntitiesManager_Folders.Find(x => ((Folder)x.Tag).ID == Parent);
                var TVI = new TreeViewItem
                {
                    Header = folder.Name,
                    Tag = folder,
                    ContextMenu = EntitiesManager_Folder
                };

                SaveManager.SaveData.Folders.Add(folder);
                ParentTVI.Items.Add(TVI);
                EntitiesManager_Folders.Add(TVI);
            }
            else
            {
                var TVI = new TreeViewItem
                {
                    Header = folder.Name,
                    Tag = folder,
                    ContextMenu = EntitiesManager_Folder
                };

                SaveManager.SaveData.Folders.Add(folder);
                EntitiesManager_Tree.Items.Add(TVI);
                EntitiesManager_Folders.Add(TVI);
            }
        }

        public void EntitiesManager_DeleteDir(string ID)
        {
            foreach (var ChildFolder in SaveManager.SaveData.Folders.FindAll(x => x.Parent == ID))
                EntitiesManager_DeleteDir(ChildFolder.ID);
            foreach (Entry Trainer in SaveManager.SaveData.Trainers.FindAll(x => x.Parent_Folder == ID))
                EntitiesManager_DeleteEntry(Trainer.ID);
            foreach (Entry Pokemon in SaveManager.SaveData.Pokemon.FindAll(x => x.Parent_Folder == ID))
                EntitiesManager_DeleteEntry(Pokemon.ID);

            var Folder = SaveManager.SaveData.Folders.Find(x => x.ID == ID);

            var TVI = EntitiesManager_Folders.Find(x => ((Folder)x.Tag).ID == Folder.ID);
            if (Folder.Parent == null)
            {
                EntitiesManager_Tree.Items.Remove(TVI);
            }
            else
            {
                var ParentTVI = EntitiesManager_Folders.Find(x => ((Folder)x.Tag).ID == Folder.Parent);
                ParentTVI.Items.Remove(TVI);
            }

            EntitiesManager_Folders.Remove(TVI);
            SaveManager.SaveData.Folders.Remove(Folder);
        }

        public void EntitiesManager_DeleteEntry(string ID)
        {
            var Entry = (Entry_Data)EntitiesManager_Entrys.Find(x => ((Entry_Data)x.Tag).ID == ID).Tag;

            if (Entry.Entities_Type == Entities_Type.Pokemon)
                try
                {
                    var pchar = SaveManager.SaveData.Pokemon.Find(x => x.ID == ID);
                    SaveManager.SaveData.Pokemon.Remove(pchar);
                }
                catch
                {
                    /* Dont Care */
                }
            else if (Entry.Entities_Type == Entities_Type.Trainer)
                try
                {
                    var tchar = SaveManager.SaveData.Trainers.Find(x => x.ID == ID);
                    SaveManager.SaveData.Trainers.Remove(tchar);
                }
                catch
                {
                    /* Dont Care */
                }

            var TVI = EntitiesManager_Entrys.Find(x => ((Entry_Data)x.Tag).ID == ID);
            if (Entry.Parent_Folder == null)
            {
                EntitiesManager_Tree.Items.Remove(TVI);
            }
            else
            {
                var Parent = EntitiesManager_Folders.Find(x => ((Folder)x.Tag).ID == Entry.Parent_Folder);
                Parent.Items.Remove(TVI);
            }

            EntitiesManager_Entrys.Remove(TVI);
        }

        private void EntitiesManager_DisplayFolder(Folder folder)
        {
            #region Context Menu

            var EntitiesManager_Folder = new ContextMenu();
            EntitiesManager_Folder.Tag = folder;
            var ctxm_Folder_CreateFolder = new MenuItem();
            ctxm_Folder_CreateFolder.Header = "Create Folder";
            ctxm_Folder_CreateFolder.Click += Ctxm_Folder_CreateFolder_Click;
            var ctxm_Folder_CreatePokemonEntities = new MenuItem();
            ctxm_Folder_CreatePokemonEntities.Header = "Create Pokemon Entities";
            ctxm_Folder_CreatePokemonEntities.Click += Ctxm_Folder_CreatePokemonEntities_Click;
            var ctxm_Folder_CreateTrainerEntities = new MenuItem();
            ctxm_Folder_CreateTrainerEntities.Header = "Create Trainer Entities";
            ctxm_Folder_CreateTrainerEntities.Click += Ctxm_Folder_CreateTrainerEntities_Click;
            ctxm_Folder_CreateTrainerEntities.IsEnabled = false;
            var ctxm_Folder_S1 = new Separator();
            var ctxm_Folder_Delete = new MenuItem();
            ctxm_Folder_Delete.Header = "Delete";
            ctxm_Folder_Delete.Click += Ctxm_Folder_Delete_Click;

            EntitiesManager_Folder.Items.Add(ctxm_Folder_CreateFolder);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_CreatePokemonEntities);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_CreateTrainerEntities);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_S1);
            EntitiesManager_Folder.Items.Add(ctxm_Folder_Delete);

            #endregion

            var Parent = EntitiesManager_Folders.Find(x => ((Folder)x.Tag).ID == folder.Parent);

            var Child = new TreeViewItem
            {
                Header = folder.Name,
                Tag = folder,
                ContextMenu = EntitiesManager_Folder
            };

            if (Parent == null)
                EntitiesManager_Tree.Items.Add(Child);
            else
                Parent.Items.Add(Child);
            EntitiesManager_Folders.Add(Child);

            EntitiesManager_Display(folder.ID);
        }

        public void EntitiesManager_CreatePokemonEntities(string Folder = null)
        {
            var pc = new Pokemon_Character(SaveManager);
            pc.PokemonData.Parent_Folder = Folder;
            pc.ShowDialog();

            SaveManager.SaveData.Pokemon.Add(pc.PokemonData);

            EntitiesManager_DisplayEntry(pc.PokemonData.EntryData);
        }

        public void EntitiesManager_EditPokemonEntities(PokemonCharacter Pokemon)
        {
            var View = new List<KeyValuePair<Color, string>>();
            var pc = new Pokemon_Character(SaveManager, Pokemon);
            pc.ShowDialog();

            foreach (var user in pc.PokemonData.View)
            {
                var UData = SaveManager.SaveData.AuthManager.Users_GetByID(user);

                View.Add(new KeyValuePair<Color, string>(UData.UserColor, UData.IC_Name));
            }

            var TVI = EntitiesManager_Entrys.Find(x => ((Entry_Data)x.Tag).ID == Pokemon.ID);
            var ELI = (EntitiesListItem)TVI.Header;

            ELI.Update(SaveManager.LoadImage(Pokemon.Token_ResourceID), Pokemon.Name, View);
        }

        private void EntitiesManager_DisplayEntry(Entry_Data entry)
        {
            var View = new List<KeyValuePair<Color, string>>();
            var ELI = new EntitiesListItem();

            if (entry.View == null)
                entry.View = new List<string>();
            if (entry.Edit == null)
                entry.Edit = new List<string>();

            foreach (var user in entry.View)
            {
                var UData = SaveManager.SaveData.AuthManager.Users_GetByID(user);

                View.Add(new KeyValuePair<Color, string>(UData.UserColor, UData.IC_Name));
            }
            ELI.Update(SaveManager.LoadImage(entry.Token_ResourceID), entry.Name, View);

            #region Context Menu

            var EntitiesManager_Entities = new ContextMenu();
            EntitiesManager_Entities.Tag = entry;
            var ctxm_Entities_Edit = new MenuItem();
            ctxm_Entities_Edit.Header = "Edit";
            ctxm_Entities_Edit.Click += Ctxm_Entities_Edit_Click;
            var ctxm_Entities_Duplicate = new MenuItem();
            ctxm_Entities_Duplicate.Header = "Duplicate";
            ctxm_Entities_Duplicate.Click += Ctxm_Entities_Duplicate_Click;
            ctxm_Entities_Duplicate.IsEnabled = false;
            var ctxm_Entities_S1 = new Separator();
            var ctxm_Entities_Delete = new MenuItem();
            ctxm_Entities_Delete.Header = "Delete";
            ctxm_Entities_Delete.Click += Ctxm_Entities_Delete_Click;

            EntitiesManager_Entities.Items.Add(ctxm_Entities_Edit);
            EntitiesManager_Entities.Items.Add(ctxm_Entities_Duplicate);
            EntitiesManager_Entities.Items.Add(ctxm_Entities_S1);
            EntitiesManager_Entities.Items.Add(ctxm_Entities_Delete);

            #endregion

            var TVI = new TreeViewItem
            {
                Header = ELI,
                Tag = entry,
                ContextMenu = EntitiesManager_Entities
            };

            if (entry.Parent_Folder == null)
            {
                EntitiesManager_Tree.Items.Add(TVI);
            }
            else
            {
                var Parent = EntitiesManager_Folders.Find(x => ((Folder)x.Tag).ID == entry.Parent_Folder);
                Parent.Items.Add(TVI);
            }
            EntitiesManager_Entrys.Add(TVI);
        }

        private void EntitiesManager_Display(string ParentID = null)
        {
            TreeViewItem Child;

            foreach (var pokemon in SaveManager.SaveData.Pokemon.FindAll(x => x.Parent_Folder == ParentID))
            {
                var entry = pokemon.EntryData;
                EntitiesManager_DisplayEntry(entry);
            }

            foreach (var folder in SaveManager.SaveData.Folders.FindAll(x => x.Parent == ParentID))
                EntitiesManager_DisplayFolder(folder);
        }

        #endregion

        #region Overview and Settings Tab

        #region Basic

        /// <summary>
        ///     Loads Campaign Info
        /// </summary>
        public void OverViewSettings_Reload()
        {
            OverViewSettings_Basic_CampaignName.Text = SaveManager.SaveData.Campaign_Data.Campaign_Name;
            OverViewSettings_Basic_GMName.Text = SaveManager.SaveData.Campaign_Data.Campaign_GM_Name;
            OverViewSettings_Basic_Description.Text = SaveManager.SaveData.Campaign_Data.Campaign_Desc;

            OverViewSettings_Server_Address.Text = SaveManager.SaveData.Campaign_Data.Server_Address;
            OverViewSettings_Server_Port.Value = SaveManager.SaveData.Campaign_Data.Server_Port;
        }

        private void OverViewSettings_Basic_CampaignName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SaveManager.SaveData.Campaign_Data.Campaign_Name = OverViewSettings_Basic_CampaignName.Text;
            }
            catch
            {
            }
        }

        private void OverViewSettings_Basic_GMName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SaveManager.SaveData.Campaign_Data.Campaign_GM_Name = OverViewSettings_Basic_GMName.Text;
            }
            catch
            {
            }
        }

        private void OverViewSettings_Basic_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SaveManager.SaveData.Campaign_Data.Campaign_Desc = OverViewSettings_Basic_Description.Text;
            }
            catch
            {
            }
        }

        #endregion

        #region Advanced
        private void OverviewSettings_Advanced_Types_Edit_Click(object sender, RoutedEventArgs e)
        {
            UI.Battle.Typing_Editor Editor = new UI.Battle.Typing_Editor();
            Editor.ShowDialog();
        }
        #endregion

        #region Users & Groups

        public void UserGroup_Users_Reload()
        {
            OverViewSettings_UsersGroups_UserList.Items.Clear();

            foreach (var User in SaveManager.SaveData.AuthManager.Users_List())
                OverViewSettings_UsersGroups_UserList.Items.Add(User);
        }

        public void UserGroup_Groups_Reload()
        {
            OverViewSettings_UsersGroups_GroupList.Items.Clear();
        }

        private void OverViewSettings_UsersGroups_CreateUser_Click(object sender, RoutedEventArgs e)
        {
            var User = new Users();
            var Pass = User.ShowDialog();

            SaveManager.SaveData.AuthManager.Users_Add(User.User);
            OverViewSettings_UsersGroups_UserList.Items.Add(User.User);
        }

        private void OverViewSettings_UsersGroups_EditUser_Click(object sender, RoutedEventArgs e)
        {
            var User = new Users((User)OverViewSettings_UsersGroups_UserList.SelectedItems[0]);
            var Pass = User.ShowDialog();

            UserGroup_Users_Reload();
        }

        private void OverViewSettings_UsersGroups_DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            SaveManager.SaveData.AuthManager.Users_Remove((User)OverViewSettings_UsersGroups_UserList.SelectedItems[0]);

            UserGroup_Users_Reload();
        }
        #endregion

        #endregion
    }

    /// <summary>
    ///     All the types that the pokedex list will display (Used for Pharsing Selections)
    /// </summary>
    public enum PokedexList_DataType
    {
        Pokemon,
        Move,
        Ability,
        Item
    }

    /// <summary>
    ///     A Class designed for Data Binding Pokedex Data to the Pokedex List
    /// </summary>
    public class PokedexList_DataBind
    {
        /// <summary>
        ///     The ID of the Object
        /// </summary>
        public decimal ID { get; set; }

        /// <summary>
        ///     The Name of the Object
        /// </summary>
        public string Name { get; set; }

        public string Type { get; set; }

        /// <summary>
        ///     The Class of move
        /// </summary>
        public string Class { get; set; }

        public string EntryType { get; set; }

        public PokedexList_DataType DataType { get; set; }
        public object DataTag { get; set; }
    }
}