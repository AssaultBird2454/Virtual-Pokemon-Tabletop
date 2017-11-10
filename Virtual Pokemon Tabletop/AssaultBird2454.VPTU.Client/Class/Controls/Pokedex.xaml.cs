﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssaultBird2454.VPTU.Client.Class.Controls
{
    /// <summary>
    /// Interaction logic for Pokedex.xaml
    /// </summary>
    public partial class Pokedex : UserControl
    {
        public event Button_Pressed Reload_Pressed;
        public event Pokedex_Entry_Selection_Changed Pokedex_Entry_Selection_Changed_Event;
        VPTU.Pokedex.Save_Data.Pokedex PokedexData = new VPTU.Pokedex.Save_Data.Pokedex();

        public Pokedex()
        {
            InitializeComponent();
            PokedexData = new VPTU.Pokedex.Save_Data.Pokedex(true);
        }

        #region List & Searching
        private void Tools_Reload_Click(object sender, RoutedEventArgs e)
        {
            List.Dispatcher.Invoke(new Action(() => List.Items.Clear()));
            Reload_Pressed?.Invoke();
        }

        public void Update_Pokemon_List(List<VPTU.Pokedex.Pokemon.PokemonData> _PokemonData)
        {
            PokedexData.Pokemon = _PokemonData;

            Reload_List();
        }

        private Thread Search_Thread;
        public void Reload_List()
        {
            try
            {
                Search_Thread.Abort();
                Search_Thread = null;
            }
            catch { }

            Search_Thread = new Thread(new ThreadStart(() =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    List.Items.Clear();

                    #region Pokemon
                    if (Search_Pokemon.IsChecked == true)
                    {
                        foreach (VPTU.Pokedex.Pokemon.PokemonData pokemon in PokedexData.Pokemon)
                        {
                            if (pokemon.Species_Name.ToLower().Contains(Search_Name.Text.ToLower()))
                            {
                                List.Items.Add(pokemon);
                            }
                        }
                    }
                    #endregion

                }));
            }));
            Search_Thread.Start();
        }

        public void Update_PokedexData(object Data)
        {
            if (Data is Server.Instances.CommandData.Pokedex.Pokedex_Pokemon_Get)
            {
                try
                {
                    Search_Thread.Abort();
                    Search_Thread = null;
                }
                catch { }

                try
                {
                    PokedexData.Pokemon = ((Server.Instances.CommandData.Pokedex.Pokedex_Pokemon_Get)Data).Pokemon_Dex;
                }
                catch { }

                Reload_List();
            }
        }

        private void Search_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Reload_List();
        }

        private void Search_Options_Changed(object sender, RoutedEventArgs e)
        {
            Reload_List();
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pokedex_Entry_Type type = Pokedex_Entry_Type.None;
            if (List.SelectedItem != null)
            {
                if (List.SelectedItem is VPTU.Pokedex.Pokemon.PokemonData) { type = Pokedex_Entry_Type.Pokemon; }
                else if (List.SelectedItem is VPTU.Pokedex.Moves.MoveData) { type = Pokedex_Entry_Type.Move; }
                else if (List.SelectedItem is VPTU.Pokedex.Abilitys.AbilityData) { type = Pokedex_Entry_Type.Ability; }
                else if (List.SelectedItem is VPTU.Pokedex.Items.ItemData) { type = Pokedex_Entry_Type.Item; }
            }
            else { type = Pokedex_Entry_Type.None; }

            Pokedex_Entry_Selection_Changed_Event?.Invoke(type, List.SelectedItem);
        }
        #endregion

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            SaveEditor.UI.Pokedex.Pokemon pokemon = new SaveEditor.UI.Pokedex.Pokemon(PokedexData);
            bool? pass = pokemon.ShowDialog();

            if(pass == true)
            {
                // Updated Pokedex Entry
            }
        }
    }
}
