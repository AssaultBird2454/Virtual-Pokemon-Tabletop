using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AssaultBird2454.VPTU.Pokedex.Pokemon;

namespace AssaultBird2454.VPTU.SaveEditor.UI.Pokedex.Select
{
    /// <summary>
    ///     Interaction logic for Select_Move.xaml
    /// </summary>
    public partial class Select_Pokemon : Window
    {
        private readonly VPTU.Pokedex.Save_Data.Pokedex Pokedex;

        /// <summary>
        ///     The Move Data that was selected
        /// </summary>
        public PokemonData Selected_Pokemon;

        public Select_Pokemon(VPTU.Pokedex.Save_Data.Pokedex _Pokedex)
        {
            Pokedex = _Pokedex;

            InitializeComponent();

            ReloadList();
        }

        private void ReloadList(string SearchName = "")
        {
            Dispatcher.Invoke(() => Pokemon.Items.Clear());

            foreach (var data in Pokedex.Pokemon_List())
                if (data.Species_Name.ToLower().Contains(SearchName.ToLower()) ||
                    data.Species_DexID.ToString().Contains(SearchName))
                    Dispatcher.Invoke(new Action(() => Pokemon.Items.Add(data)));
        }

        private void Moves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selected_Pokemon = (PokemonData) Pokemon.SelectedItem;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (Selected_Pokemon == null)
                DialogResult = false;
            else
                DialogResult = true;

            Close();
        }

        #region Searching Code

        private Thread SearchThread;

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Stop the thread from searching
            try
            {
                SearchThread.Abort();
                SearchThread = null;
            }
            catch
            {
            }
            var Filter = textBox.Text;

            SearchThread = new Thread(() => ReloadList(Filter));
            SearchThread.IsBackground = true;
            SearchThread.Start();
        }

        #endregion
    }
}