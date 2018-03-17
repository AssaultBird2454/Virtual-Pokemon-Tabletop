using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using AssaultBird2454.VPTU.Authentication_Manager.Data;
using AssaultBird2454.VPTU.Client.UI.Identity;
using Microsoft.Win32;

namespace AssaultBird2454.VPTU.Client.UI
{
    /// <summary>
    ///     Interaction logic for Manage_Identities.xaml
    /// </summary>
    public partial class Manage_Identities
    {
        public Manage_Identities()
        {
            InitializeComponent();

            Load();
        }

        public void Load()
        {
            ID_List.Items.Clear();

            foreach (var id in Program.Identities)
                ID_List.Items.Add(id);
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var add = new Add();

            var Pass = add.ShowDialog();
            if (Pass == true)
            {
                Program.Identities.Add(add.UID);
                ID_List.Items.Add(add.UID);
            }
        }

        private void Import_Button_Click(object sender, RoutedEventArgs e)
        {
            var OFD = new OpenFileDialog();
            OFD.Title = "Save Virtual PTU Identity File";
            OFD.DefaultExt = ".ptuif";
            OFD.Filter = "Pokemon Tabletop User Identity File (*.ptuif)|*.ptuif";
            OFD.CheckPathExists = true;
            OFD.CheckFileExists = true;
            OFD.FileOk += OFD_FileOk;

            OFD.ShowDialog();
        }

        private void OFD_FileOk(object sender, CancelEventArgs e)
        {
            ClientIdentity ID;

            using (var SR = new StreamReader(new FileStream(((OpenFileDialog)sender).FileName, FileMode.OpenOrCreate)))
            {
                ID = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientIdentity>(SR.ReadToEnd());
            }

            Program.Identities.Add(ID);
            ID_List.Items.Add(ID);
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var edit = new Add((ClientIdentity)ID_List.SelectedItems[0]);
                edit.ShowDialog();

                Load();
            }
            catch (IndexOutOfRangeException) { /* Dont Care, No value selected */ }
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            var id = (ClientIdentity)ID_List.SelectedItems[0];

            Program.Identities.Remove(id);
            ID_List.Items.Remove(id);
        }
    }
}