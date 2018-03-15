using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AssaultBird2454.VPTU.Authentication_Manager.Data;
using Microsoft.Win32;

namespace AssaultBird2454.VPTU.SaveEditor.UI.Users
{
    /// <summary>
    ///     Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        public User User;

        public Users(User _User = null)
        {
            InitializeComponent();

            if (_User == null)
                User = new User(true);
            else
                User = _User;

            MainWindow.SaveManager.SaveData.AuthManager.Identity_GetKey(User.UserID);
            Load();
        }

        public void Load()
        {
            Player_ID.Text = User.UserID;
            Player_Name.Text = User.Name;
            Player_ICN.Text = User.IC_Name;
            isGM.IsChecked = User.isGM;

            PlayerColor_Picker.SelectedColor = User.UserColor;

            // Groups
        }

        public void Save()
        {
            User.Name = Player_Name.Text;
            User.IC_Name = Player_ICN.Text;

            User.UserColor = (Color)PlayerColor_Picker.SelectedColor;

            // Groups
        }

        private void PlayerColor_Picker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (PlayerColor_Picker.SelectedColor != null)
                User.UserColor = (Color)PlayerColor_Picker.SelectedColor;
        }

        private void View_PlayerKey_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(MainWindow.SaveManager.SaveData.AuthManager.Identity_GetKey(User.UserID), "Player Key");
        }

        private void Export_PlayerKey_Click(object sender, RoutedEventArgs e)
        {
            var SFD = new SaveFileDialog();
            SFD.Title = "Save Virtual PTU Identity File";
            SFD.DefaultExt = ".ptuif";
            SFD.Filter = "Pokemon Tabletop User Identity File (*.ptuif)|*.ptuif";
            SFD.CheckPathExists = true;
            SFD.CheckFileExists = false;
            SFD.OverwritePrompt = true;
            SFD.FileOk += SFD_FileOk;

            SFD.ShowDialog();
        }

        private void SFD_FileOk(object sender, CancelEventArgs e)
        {
            var ID = new ClientIdentity();

            ID.AuthKey = MainWindow.SaveManager.SaveData.AuthManager.Identity_GetKey(User.UserID);
            ID.Campaign_Name = MainWindow.SaveManager.SaveData.Campaign_Data.Campaign_Name;
            ID.ICN = User.IC_Name;
            ID.Server_Address = MainWindow.SaveManager.SaveData.Campaign_Data.Server_Address;
            ID.Server_Port = MainWindow.SaveManager.SaveData.Campaign_Data.Server_Port;

            try
            {
                File.Delete(((SaveFileDialog)sender).FileName);
            }
            catch
            {
            }

            using (var SW = new StreamWriter(new FileStream(((SaveFileDialog)sender).FileName, FileMode.OpenOrCreate)))
            {
                SW.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ID));
                SW.Flush();
            }
        }

        private void ReGenerate_PlayerKey_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(MainWindow.SaveManager.SaveData.AuthManager.Identity_GenerateKey(User.UserID), "New Player Key");
        }

        private void Player_ID_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
        }

        private void Player_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            User.Name = Player_Name.Text;
        }

        private void Player_ICN_TextChanged(object sender, TextChangedEventArgs e)
        {
            User.IC_Name = Player_ICN.Text;
        }

        private void isGM_Checked(object sender, RoutedEventArgs e)
        {
            User.isGM = (bool)isGM.IsChecked;
        }
    }
}