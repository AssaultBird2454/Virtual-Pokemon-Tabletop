using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AssaultBird2454.VPTU.EntitiesManager.Pokemon;

namespace AssaultBird2454.VPTU.SaveEditor.UI.Entities.Pokemon
{
    /// <summary>
    ///     Interaction logic for Select.xaml
    /// </summary>
    public partial class Select : Window
    {
        private readonly SaveManager.SaveManager Manager;

        public Thread search_thread;
        public PokemonCharacter SelectedPokemon;

        public Select(SaveManager.SaveManager _Manager)
        {
            Manager = _Manager;

            InitializeComponent();
        }

        public void Load()
        {
            try
            {
                search_thread.Abort();
            }
            catch
            {
            }
            search_thread = new Thread(() => { Dispatcher.Invoke(() => Reload()); });

            search_thread.Start();
        }

        public void Reload()
        {
            Pokemon_List.Items.Clear();

            if (Search_WildPokemon.IsChecked == true)
                foreach (var pokemon in Manager.SaveData.Pokemon)
                    if (pokemon.Name.ToLower().Contains(Search_Name.Text.ToLower()))
                        Pokemon_List.Items.Add(new Pokemon_DataBind(pokemon,
                            pokemon.Species_DexID + " (" + Manager.SaveData.PokedexData.Pokemon_List()
                                .Find(x => x.Species_DexID == pokemon.Species_DexID).Species_Name + ")"));

            if (Search_Trainer_Pokemon.IsChecked == true)
                foreach (var trainer in Manager.SaveData.Trainers)
                foreach (var pokemon in trainer.PartyPokemon)
                    if (pokemon.Name.ToLower().Contains(Search_Name.Text.ToLower()))
                        Pokemon_List.Items.Add(new Pokemon_DataBind(pokemon,
                            pokemon.Species_DexID + " (" + Manager.SaveData.PokedexData.Pokemon_List()
                                .Find(x => x.Species_DexID == pokemon.Species_DexID).Species_Name + ")", trainer.Name));
        }


        private void Search_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Load();
        }

        private void Search_Trainer_Pokemon_Unchecked(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Search_WildPokemon_Unchecked(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Search_WildPokemon_Checked(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Search_Trainer_Pokemon_Checked(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Select_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SelectedPokemon = ((Pokemon_DataBind) Pokemon_List.SelectedItem).Pokemon;
            Close();
        }
    }

    public class Pokemon_DataBind
    {
        public Pokemon_DataBind(PokemonCharacter _Pokemon, string _Species, string _Owner = "")
        {
            Pokemon = _Pokemon;
            Species = _Species;
            Owner = _Owner;
        }

        public PokemonCharacter Pokemon { get; set; }
        public string Species { get; set; }
        public string Owner { get; set; }
    }
}