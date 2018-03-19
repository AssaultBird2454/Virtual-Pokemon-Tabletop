using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AssaultBird2454.VPTU.Authentication_Manager.Data;
using AssaultBird2454.VPTU.BattleManager.Data;
using AssaultBird2454.VPTU.EntitiesManager.Pokemon;
using AssaultBird2454.VPTU.Pokedex.Entities;
using AssaultBird2454.VPTU.Pokedex.Moves;
using AssaultBird2454.VPTU.RNG.Generators;
using AssaultBird2454.VPTU.SaveEditor.UI.Entities.Pokemon;
using AssaultBird2454.VPTU.SaveEditor.UI.Pokedex.Select;
using AssaultBird2454.VPTU.SaveEditor.UI.Resources;

namespace AssaultBird2454.VPTU.SaveEditor.UI.Entities
{
    /// <summary>
    ///     Interaction logic for Pokemon_Character.xaml
    /// </summary>
    public partial class Pokemon_Character : Window
    {
        public SaveManager.SaveManager Manager;
        public PokemonCharacter PokemonData;
        private bool Ready;

        private void Change_Species_Click(object sender, RoutedEventArgs e)
        {
            var pokemon = new Select_Pokemon(Manager.SaveData.PokedexData);
            var pass = pokemon.ShowDialog();

            if (pass == true)
            {
                PokemonData.Species_DexID = pokemon.Selected_Pokemon.Species_DexID;
                LoadFromSpecies();
            }
        }

        private void Button_SelectToken_Click(object sender, RoutedEventArgs e)
        {
            var Select = new Search_Resources();

            var dr = Select.ShowDialog();

            if (dr == true)
            {
                PokemonData.Token_ResourceID = Select.Selected_Resource;

                var bitmapSource =
                    Imaging.CreateBitmapSourceFromHBitmap(
                        MainWindow.SaveManager.LoadImage(PokemonData.Token_ResourceID).GetHbitmap(), IntPtr.Zero,
                        Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                Token.Background = new ImageBrush(bitmapSource)
                {
                    Stretch = Stretch.Uniform,
                    TileMode = TileMode.None
                };
            }
        }

        #region Base Functions

        public Pokemon_Character(SaveManager.SaveManager _Mgr, PokemonCharacter _PokemonData = null)
        {
            Manager = _Mgr;
            if (_PokemonData == null)
                PokemonData = new PokemonCharacter(RSG.GenerateString(10));
            else
                PokemonData = _PokemonData;

            InitializeComponent();
            Init();

            Load();
        }

        private void Init()
        {
            #region Events

            Basic_Types.SelectionChangedEvent += Basic_Types_SelectionChangedEvent;

            #endregion

            var itemSource = new Dictionary<string, object>();
            foreach (BattleManager.Typing.Typing_Data type in MainWindow.SaveManager.SaveData.Typing_Manager.Types)
                itemSource.Add(type.Type_Name, type.Type_Name);
            Basic_Types.ItemsSource = itemSource;

            Basic_Weight.ItemsSource = Enum.GetValues(typeof(WeightClass));
            Basic_Size.ItemsSource = Enum.GetValues(typeof(SizeClass));
            Basic_Nature.ItemsSource = Enum.GetValues(typeof(Nature));

            #region Skills

            Skill_Acrobatics_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Athletics_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Charm_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Combat_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Command_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Focus_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_GeneralEDU_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Gulie_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Intimidate_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Intuition_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_MedicineEDU_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_OccultEDU_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Perception_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_PokemonEDU_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Stealth_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_Survival_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));
            Skill_TechnologyEDU_Rank.ItemsSource = Enum.GetValues(typeof(SkillRank));

            #endregion

            #region Defaulting

            Basic_Weight.SelectedIndex = 0;
            Basic_Size.SelectedIndex = 0;
            Basic_Nature.SelectedIndex = 0;

            Skill_Acrobatics_Rank.SelectedIndex = 1;
            Skill_Athletics_Rank.SelectedIndex = 1;
            Skill_Charm_Rank.SelectedIndex = 1;
            Skill_Combat_Rank.SelectedIndex = 1;
            Skill_Command_Rank.SelectedIndex = 1;
            Skill_Focus_Rank.SelectedIndex = 1;
            Skill_GeneralEDU_Rank.SelectedIndex = 1;
            Skill_Gulie_Rank.SelectedIndex = 1;
            Skill_Intimidate_Rank.SelectedIndex = 1;
            Skill_Intuition_Rank.SelectedIndex = 1;
            Skill_MedicineEDU_Rank.SelectedIndex = 1;
            Skill_OccultEDU_Rank.SelectedIndex = 1;
            Skill_Perception_Rank.SelectedIndex = 1;
            Skill_PokemonEDU_Rank.SelectedIndex = 1;
            Skill_Stealth_Rank.SelectedIndex = 1;
            Skill_Survival_Rank.SelectedIndex = 1;
            Skill_TechnologyEDU_Rank.SelectedIndex = 1;

            #endregion
        }

        //private List<ComboBoxItem> Species_List;

        #endregion

        #region Save & Load Functions

        /// <summary>
        ///     Load the pokemon from the data
        /// </summary>
        public void Load()
        {
            Ready = false;

            try
            {
                Basic_ID.Text = PokemonData.ID;
            }
            catch
            {
            }
            try
            {
                Basic_Name.Text = PokemonData.Name;
            }
            catch
            {
            }
            try
            {
                var pokemon =
                    Manager.SaveData.PokedexData.Pokemon_GetID(PokemonData.Species_DexID);
                Basic_Species.Content = pokemon.Species_DexID + " (" + pokemon.Species_Name + ")";
            }
            catch
            {
            }

            try
            {
                #region Pokemon Types

                var itemso = new Dictionary<string, object>();
                foreach (string type in PokemonData.PokemonType)
                    itemso.Add(type, type);
                Basic_Types.SelectedItems = itemso;

                #endregion
            }
            catch
            {
            }

            try
            {
                Basic_Size.SelectedItem = PokemonData.SizeClass;
            }
            catch
            {
            }
            try
            {
                Basic_Weight.SelectedItem = PokemonData.WeightClass;
            }
            catch
            {
            }
            try
            {
                Basic_Nature.SelectedItem = PokemonData.Nature;
            }
            catch
            {
            }
            try
            {
                Basic_Desc.Text = PokemonData.Notes;
            }
            catch
            {
            }

            try
            {
                #region Gender

                if (PokemonData.Gender == Gender.Male)
                    Basic_SexMale.IsChecked = true;
                else if (PokemonData.Gender == Gender.Female)
                    Basic_SexFemale.IsChecked = true;
                else if (PokemonData.Gender == Gender.Genderless)
                    Basic_SexNone.IsChecked = true;

                #endregion
            }
            catch
            {
            }

            try
            {
                Basic_EXP.Value = PokemonData.EXP;
            }
            catch
            {
            }
            try
            {
                Basic_CurrentHP.Value = PokemonData.Current_HP;
            }
            catch
            {
            }
            try
            {
                Basic_Injuries.Value = PokemonData.Injuries;
            }
            catch
            {
            }

            #region Skills

            if (PokemonData.Skills == null)
                PokemonData.Skills = new Skill_Data();

            Skill_Acrobatics_Rank.SelectedItem = PokemonData.Skills.Acrobatics_Rank;
            Skill_Athletics_Rank.SelectedItem = PokemonData.Skills.Athletics_Rank;
            Skill_Combat_Rank.SelectedItem = PokemonData.Skills.Combat_Rank;
            Skill_Intimidate_Rank.SelectedItem = PokemonData.Skills.Intimidate_Rank;
            Skill_Stealth_Rank.SelectedItem = PokemonData.Skills.Stealth_Rank;
            Skill_Survival_Rank.SelectedItem = PokemonData.Skills.Survival_Rank;

            Skill_Acrobatics_Mod.Value = PokemonData.Skills.Acrobatics_Mod;
            Skill_Athletics_Mod.Value = PokemonData.Skills.Athletics_Mod;
            Skill_Combat_Mod.Value = PokemonData.Skills.Combat_Mod;
            Skill_Intimidate_Mod.Value = PokemonData.Skills.Intimidate_Mod;
            Skill_Stealth_Mod.Value = PokemonData.Skills.Stealth_Mod;
            Skill_Survival_Mod.Value = PokemonData.Skills.Survival_Mod;

            Skill_GeneralEDU_Rank.SelectedItem = PokemonData.Skills.General_Rank;
            Skill_MedicineEDU_Rank.SelectedItem = PokemonData.Skills.Medicine_Rank;
            Skill_OccultEDU_Rank.SelectedItem = PokemonData.Skills.Occult_Rank;
            Skill_PokemonEDU_Rank.SelectedItem = PokemonData.Skills.Pokemon_Rank;
            Skill_TechnologyEDU_Rank.SelectedItem = PokemonData.Skills.Technology_Rank;
            Skill_Gulie_Rank.SelectedItem = PokemonData.Skills.Guile_Rank;
            Skill_Perception_Rank.SelectedItem = PokemonData.Skills.Perception_Rank;

            Skill_GeneralEDU_Mod.Value = PokemonData.Skills.General_Mod;
            Skill_MedicineEDU_Mod.Value = PokemonData.Skills.Medicine_Mod;
            Skill_OccultEDU_Mod.Value = PokemonData.Skills.Occult_Mod;
            Skill_PokemonEDU_Mod.Value = PokemonData.Skills.Pokemon_Mod;
            Skill_TechnologyEDU_Mod.Value = PokemonData.Skills.Technology_Mod;
            Skill_Gulie_Mod.Value = PokemonData.Skills.Guile_Mod;
            Skill_Perception_Mod.Value = PokemonData.Skills.Perception_Mod;

            Skill_Charm_Rank.SelectedItem = PokemonData.Skills.Charm_Rank;
            Skill_Command_Rank.SelectedItem = PokemonData.Skills.Command_Rank;
            Skill_Focus_Rank.SelectedItem = PokemonData.Skills.Focus_Rank;
            Skill_Intuition_Rank.SelectedItem = PokemonData.Skills.Intuition_Rank;

            Skill_Charm_Mod.Value = PokemonData.Skills.Charm_Mod;
            Skill_Command_Mod.Value = PokemonData.Skills.Command_Mod;
            Skill_Focus_Mod.Value = PokemonData.Skills.Focus_Mod;
            Skill_Intuition_Mod.Value = PokemonData.Skills.Intuition_Mod;

            #endregion

            #region Token

            try
            {
                var bitmapSource =
                    Imaging.CreateBitmapSourceFromHBitmap(
                        MainWindow.SaveManager.LoadImage(PokemonData.Token_ResourceID).GetHbitmap(), IntPtr.Zero,
                        Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                Token.Background = new ImageBrush(bitmapSource)
                {
                    Stretch = Stretch.Uniform,
                    TileMode = TileMode.None
                };
            }
            catch
            {
            }

            #endregion

            #region Permissions

            foreach (var user in MainWindow.SaveManager.SaveData.AuthManager.Users_List())
            {
                var db = new Permissions_DB(PokemonData, user);
                UserList.Items.Add(db);
            }

            #endregion

            Reload_Stats();
            Reload_Moves();
            LoadStatusEffects();

            Ready = true;
        }

        /// <summary>
        ///     Loads the pokemon from the species list (Used when creating a new pokemon)
        /// </summary>
        public void LoadFromSpecies()
        {
            var NewPokemon =
                Manager.SaveData.PokedexData.Pokemon_GetID(PokemonData.Species_DexID);

            //try { PokemonData.Name = NewPokemon.Species_Name; } catch { }

            try
            {
                #region Pokemon Types

                var itemso = new Dictionary<string, object>();
                foreach (string type in PokemonData.PokemonType)
                    itemso.Add(type, type);
                Basic_Types.SelectedItems = itemso;

                #endregion
            }
            catch
            {
            }

            try
            {
                PokemonData.SizeClass = NewPokemon.Species_SizeClass;
            }
            catch
            {
            }
            try
            {
                PokemonData.WeightClass = NewPokemon.Species_WeightClass;
            }
            catch
            {
            }

            try
            {
                PokemonData.HP_SpeciesBase = NewPokemon.Species_BaseStats_HP;
                PokemonData.Attack_SpeciesBase = NewPokemon.Species_BaseStats_Attack;
                PokemonData.Defence_SpeciesBase = NewPokemon.Species_BaseStats_Defence;
                PokemonData.SpAttack_SpeciesBase = NewPokemon.Species_BaseStats_SpAttack;
                PokemonData.SpDefence_SpeciesBase = NewPokemon.Species_BaseStats_SpDefence;
                PokemonData.Speed_SpeciesBase = NewPokemon.Species_BaseStats_Speed;
            }
            catch
            {
            }

            Load();
        }

        #endregion

        #region Stats

        public void Reload_Stats()
        {
            // Values
            Stats_Mod_HP.Value = PokemonData.HP_BaseMod;
            Stats_Add_HP.Value = PokemonData.HP_AddStat;
            Stats_Mod_Attack.Value = PokemonData.Attack_BaseMod;
            Stats_Add_Attack.Value = PokemonData.Attack_AddStat;
            Stats_CS_Attack.Value = PokemonData.Attack_CombatStage;
            Stats_Mod_Defence.Value = PokemonData.Defence_BaseMod;
            Stats_Add_Defence.Value = PokemonData.Defence_AddStat;
            Stats_CS_Defence.Value = PokemonData.Defence_CombatStage;
            Stats_Mod_SpAttack.Value = PokemonData.SpAttack_BaseMod;
            Stats_Add_SpAttack.Value = PokemonData.SpAttack_AddStat;
            Stats_CS_SpAttack.Value = PokemonData.SpAttack_CombatStage;
            Stats_Mod_SpDefence.Value = PokemonData.SpDefence_BaseMod;
            Stats_Add_SpDefence.Value = PokemonData.SpDefence_AddStat;
            Stats_CS_SpDefence.Value = PokemonData.SpDefence_CombatStage;
            Stats_Mod_Speed.Value = PokemonData.Speed_BaseMod;
            Stats_Add_Speed.Value = PokemonData.Speed_AddStat;
            Stats_CS_Speed.Value = PokemonData.Speed_CombatStage;
            // HP
            Stats_SBase_HP.Content = PokemonData.HP_SpeciesBase;
            Stats_Base_HP.Content = PokemonData.HP_Base;
            Stats_Total_HP.Content = PokemonData.Stat_HP_Max;
            Basic_MaxHP.Content = "/ " + PokemonData.Stat_HP_Max;
            // Attack
            Stats_SBase_Attack.Content = PokemonData.Attack_SpeciesBase;
            Stats_Base_Attack.Content = PokemonData.Attack_Base;
            Stats_Total_Attack.Content = PokemonData.Attack_Total;
            Stats_Adj_Attack.Content = PokemonData.Attack_Adjusted;
            // Defence
            Stats_SBase_Defence.Content = PokemonData.Defence_SpeciesBase;
            Stats_Base_Defence.Content = PokemonData.Defence_Base;
            Stats_Total_Defence.Content = PokemonData.Defence_Total;
            Stats_Adj_Defence.Content = PokemonData.Defence_Adjusted;
            // Sp. Attack
            Stats_SBase_SpAttack.Content = PokemonData.SpAttack_SpeciesBase;
            Stats_Base_SpAttack.Content = PokemonData.SpAttack_Base;
            Stats_Total_SpAttack.Content = PokemonData.SpAttack_Total;
            Stats_Adj_SpAttack.Content = PokemonData.SpAttack_Adjusted;
            // Sp. Defence
            Stats_SBase_SpDefence.Content = PokemonData.SpDefence_SpeciesBase;
            Stats_Base_SpDefence.Content = PokemonData.SpDefence_Base;
            Stats_Total_SpDefence.Content = PokemonData.SpDefence_Total;
            Stats_Adj_SpDefence.Content = PokemonData.SpDefence_Adjusted;
            // Speed
            Stats_SBase_Speed.Content = PokemonData.Speed_SpeciesBase;
            Stats_Base_Speed.Content = PokemonData.Speed_Base;
            Stats_Total_Speed.Content = PokemonData.Speed_Total;
            Stats_Adj_Speed.Content = PokemonData.Speed_Adjusted;
            // Physical Evasion
            Stats_Mod_PhyEvasion.Value = PokemonData.Evasion_Physical_Mod;
            Stats_Value_PhyEvasion.Content = PokemonData.Evasion_Physical;
            // Special Evasion
            Stats_Mod_SpEvasion.Value = PokemonData.Evasion_Special_Mod;
            Stats_Value_SpEvasion.Content = PokemonData.Evasion_Special;
            // Speed Evasion
            Stats_Mod_SpeedEvasion.Value = PokemonData.Evasion_Speed_Mod;
            Stats_Value_SpeedEvasion.Content = PokemonData.Evasion_Speed;
        }

        #region HP

        private void Stats_Mod_HP_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.HP_BaseMod = (int)Stats_Mod_HP.Value;

                    Stats_Base_HP.Content = PokemonData.HP_Base;
                    Stats_Total_HP.Content = PokemonData.Stat_HP_Max;
                    Basic_MaxHP.Content = "/ " + PokemonData.Stat_HP_Max;
                }
                catch
                {
                }
        }

        private void Stats_Add_HP_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.HP_AddStat = (int)Stats_Add_HP.Value;

                    Stats_Total_HP.Content = PokemonData.Stat_HP_Max;
                    Basic_MaxHP.Content = "/ " + PokemonData.Stat_HP_Max;
                }
                catch
                {
                }
        }

        #endregion

        #region Attack

        private void Stats_Mod_Attack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Attack_BaseMod = (int)Stats_Mod_Attack.Value;

                    Stats_Base_Attack.Content = PokemonData.Attack_Base;
                    Stats_Total_Attack.Content = PokemonData.Attack_Total;
                    Stats_Adj_Attack.Content = PokemonData.Attack_Adjusted;
                }
                catch
                {
                }
        }

        private void Stats_Add_Attack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Attack_AddStat = (int)Stats_Add_Attack.Value;

                    Stats_Total_Attack.Content = PokemonData.Attack_Total;
                    Stats_Adj_Attack.Content = PokemonData.Attack_Adjusted;
                }
                catch
                {
                }
        }

        private void Stats_CS_Attack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Attack_CombatStage = (int)Stats_CS_Attack.Value;

                    Stats_Adj_Attack.Content = PokemonData.Attack_Adjusted;
                }
                catch
                {
                }
        }

        #endregion

        #region Defence

        private void Stats_Mod_Defence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Defence_BaseMod = (int)Stats_Mod_Defence.Value;

                    Stats_Base_Defence.Content = PokemonData.Defence_Base;
                    Stats_Total_Defence.Content = PokemonData.Defence_Total;
                    Stats_Adj_Defence.Content = PokemonData.Defence_Adjusted;

                    Stats_Value_PhyEvasion.Content = PokemonData.Evasion_Physical;
                }
                catch
                {
                }
        }

        private void Stats_Add_Defence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Defence_AddStat = (int)Stats_Add_Defence.Value;

                    Stats_Total_Defence.Content = PokemonData.Defence_Total;
                    Stats_Adj_Defence.Content = PokemonData.Defence_Adjusted;

                    Stats_Value_PhyEvasion.Content = PokemonData.Evasion_Physical;
                }
                catch
                {
                }
        }

        private void Stats_CS_Defence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Defence_CombatStage = (int)Stats_CS_Defence.Value;

                    Stats_Adj_Defence.Content = PokemonData.Defence_Adjusted;

                    Stats_Value_PhyEvasion.Content = PokemonData.Evasion_Physical;
                }
                catch
                {
                }
        }

        #endregion

        #region Sp. Attack

        private void Stats_Mod_SpAttack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.SpAttack_BaseMod = (int)Stats_Mod_SpAttack.Value;

                    Stats_Base_SpAttack.Content = PokemonData.SpAttack_Base;
                    Stats_Total_SpAttack.Content = PokemonData.SpAttack_Total;
                    Stats_Adj_SpAttack.Content = PokemonData.SpAttack_Adjusted;
                }
                catch
                {
                }
        }

        private void Stats_Add_SpAttack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.SpAttack_AddStat = (int)Stats_Add_SpAttack.Value;

                    Stats_Total_SpAttack.Content = PokemonData.SpAttack_Total;
                    Stats_Adj_SpAttack.Content = PokemonData.SpAttack_Adjusted;
                }
                catch
                {
                }
        }

        private void Stats_CS_SpAttack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.SpAttack_CombatStage = (int)Stats_CS_SpAttack.Value;

                    Stats_Adj_SpAttack.Content = PokemonData.SpAttack_Adjusted;
                }
                catch
                {
                }
        }

        #endregion

        #region Sp. Defence

        private void Stats_Mod_SpDefence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.SpDefence_BaseMod = (int)Stats_Mod_SpDefence.Value;

                    Stats_Base_SpDefence.Content = PokemonData.SpDefence_Base;
                    Stats_Total_SpDefence.Content = PokemonData.SpDefence_Total;
                    Stats_Adj_SpDefence.Content = PokemonData.SpDefence_Adjusted;

                    Stats_Value_SpEvasion.Content = PokemonData.Evasion_Special;
                }
                catch
                {
                }
        }

        private void Stats_Add_SpDefence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.SpDefence_AddStat = (int)Stats_Add_SpDefence.Value;

                    Stats_Total_SpDefence.Content = PokemonData.SpDefence_Total;
                    Stats_Adj_SpDefence.Content = PokemonData.SpDefence_Adjusted;

                    Stats_Value_SpEvasion.Content = PokemonData.Evasion_Special;
                }
                catch
                {
                }
        }

        private void Stats_CS_SpDefence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.SpDefence_CombatStage = (int)Stats_CS_SpDefence.Value;

                    Stats_Adj_SpDefence.Content = PokemonData.SpDefence_Adjusted;

                    Stats_Value_SpEvasion.Content = PokemonData.Evasion_Special;
                }
                catch
                {
                }
        }

        #endregion

        #region Speed

        private void Stats_Mod_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Speed_BaseMod = (int)Stats_Mod_Speed.Value;

                    Stats_Base_Speed.Content = PokemonData.Speed_Base;
                    Stats_Total_Speed.Content = PokemonData.Speed_Total;
                    Stats_Adj_Speed.Content = PokemonData.Speed_Adjusted;

                    Stats_Value_SpeedEvasion.Content = PokemonData.Evasion_Speed;
                }
                catch
                {
                }
        }

        private void Stats_Add_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Speed_AddStat = (int)Stats_Add_Speed.Value;

                    Stats_Total_Speed.Content = PokemonData.Speed_Total;
                    Stats_Adj_Speed.Content = PokemonData.Speed_Adjusted;

                    Stats_Value_SpeedEvasion.Content = PokemonData.Evasion_Speed;
                }
                catch
                {
                }
        }

        private void Stats_CS_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Speed_CombatStage = (int)Stats_CS_Speed.Value;

                    Stats_Adj_Speed.Content = PokemonData.Speed_Adjusted;

                    Stats_Value_SpeedEvasion.Content = PokemonData.Evasion_Speed;
                }
                catch
                {
                }
        }

        #endregion

        // Physical Evasion
        private void Stats_Mod_PhyEvasion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Evasion_Physical_Mod = (int)Stats_Mod_PhyEvasion.Value;

                    Stats_Value_PhyEvasion.Content = PokemonData.Evasion_Physical;
                }
                catch
                {
                    /* Dont Care */
                }
        }

        // Special Evasion
        private void Stats_Mod_SpEvasion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Evasion_Special_Mod = (int)Stats_Mod_SpEvasion.Value;

                    Stats_Value_SpEvasion.Content = PokemonData.Evasion_Special;
                }
                catch
                {
                    /* Dont Care */
                }
        }

        // Speed Evasion
        private void Stats_Mod_SpeedEvasion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                try
                {
                    PokemonData.Evasion_Speed_Mod = (int)Stats_Mod_SpeedEvasion.Value;

                    Stats_Value_SpeedEvasion.Content = PokemonData.Evasion_Speed;
                }
                catch
                {
                    /* Dont Care */
                }
        }

        #endregion

        #region Basic Info (Change Events)

        private void Basic_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Ready)
                try
                {
                    PokemonData.Name = Basic_Name.Text;
                }
                catch
                {
                }
        }

        private void Basic_Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
                PokemonData.SizeClass = (SizeClass)Basic_Size.SelectedItem;
        }

        private void Basic_Weight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
                PokemonData.WeightClass = (WeightClass)Basic_Weight.SelectedItem;
        }

        private void Basic_Nature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                PokemonData.Nature = (Nature)Basic_Nature.SelectedItem;
                Reload_Stats();
            }
        }

        private void Basic_SexMale_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.Gender = Gender.Male;
        }

        private void Basic_SexFemale_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.Gender = Gender.Female;
        }

        private void Basic_SexNone_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.Gender = Gender.Genderless;
        }

        private void Basic_Types_SelectionChangedEvent()
        {
            if (Ready)
            {
                if (PokemonData.PokemonType == null)
                    PokemonData.PokemonType = new List<string>();

                PokemonData.PokemonType.Clear();
                foreach (var seltype in Basic_Types.SelectedItems)
                    PokemonData.PokemonType.Add((string)seltype.Value);
            }
        }

        private void Basic_XP_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Ready)
                {
                    PokemonData.EXP = (int)Basic_EXP.Value;

                    Reload_Stats();
                }

                Basic_REXP.Content = "Required: " + (PokemonData.Next_EXP_Requirement - PokemonData.EXP);
                Basic_Level.Content = "Level: " + PokemonData.Level;
            }
            catch
            {
            }
        }

        private void Basic_CurrentHP_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                PokemonData.Current_HP = (int)Basic_CurrentHP.Value;
        }

        private void Basic_Injuries_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                PokemonData.Injuries = (int)Basic_Injuries.Value;
        }

        #endregion

        #region Skills

        #region Body

        private void Skill_Acrobatics_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Acrobatics_Rank = (SkillRank)Skill_Acrobatics_Rank.SelectedItem;
            }
        }

        private void Skill_Athletics_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Athletics_Rank = (SkillRank)Skill_Athletics_Rank.SelectedItem;
            }
        }

        private void Skill_Combat_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Combat_Rank = (SkillRank)Skill_Combat_Rank.SelectedItem;
            }
        }

        private void Skill_Intimidate_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Intimidate_Rank = (SkillRank)Skill_Intimidate_Rank.SelectedItem;
            }
        }

        private void Skill_Stealth_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Stealth_Rank = (SkillRank)Skill_Stealth_Rank.SelectedItem;
            }
        }

        private void Skill_Survival_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Survival_Rank = (SkillRank)Skill_Survival_Rank.SelectedItem;
            }
        }

        private void Skill_Acrobatics_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Acrobatics_Mod = (int)Skill_Acrobatics_Mod.Value;
            }
        }

        private void Skill_Athletics_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Athletics_Mod = (int)Skill_Athletics_Mod.Value;
            }
        }

        private void Skill_Combat_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Combat_Mod = (int)Skill_Combat_Mod.Value;
            }
        }

        private void Skill_Intimidate_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Intimidate_Mod = (int)Skill_Intimidate_Mod.Value;
            }
        }

        private void Skill_Stealth_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Stealth_Mod = (int)Skill_Stealth_Mod.Value;
            }
        }

        private void Skill_Survival_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Survival_Mod = (int)Skill_Survival_Mod.Value;
            }
        }

        #endregion

        #region Mind

        private void Skill_GeneralEDU_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.General_Rank = (SkillRank)Skill_GeneralEDU_Rank.SelectedItem;
            }
        }

        private void Skill_MedicineEDU_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Medicine_Rank = (SkillRank)Skill_MedicineEDU_Rank.SelectedItem;
            }
        }

        private void Skill_OccultEDU_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Occult_Rank = (SkillRank)Skill_OccultEDU_Rank.SelectedItem;
            }
        }

        private void Skill_PokemonEDU_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Pokemon_Rank = (SkillRank)Skill_PokemonEDU_Rank.SelectedItem;
            }
        }

        private void Skill_TechnologyEDU_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Technology_Rank = (SkillRank)Skill_TechnologyEDU_Rank.SelectedItem;
            }
        }

        private void Skill_Gulie_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Guile_Rank = (SkillRank)Skill_Gulie_Rank.SelectedItem;
            }
        }

        private void Skill_Perception_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Perception_Rank = (SkillRank)Skill_Perception_Rank.SelectedItem;
            }
        }

        private void Skill_GeneralEDU_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.General_Mod = (int)Skill_GeneralEDU_Mod.Value;
            }
        }

        private void Skill_MedicineEDU_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Medicine_Mod = (int)Skill_MedicineEDU_Mod.Value;
            }
        }

        private void Skill_OccultEDU_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Occult_Mod = (int)Skill_OccultEDU_Mod.Value;
            }
        }

        private void Skill_PokemonEDU_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Pokemon_Mod = (int)Skill_PokemonEDU_Mod.Value;
            }
        }

        private void Skill_TechnologyEDU_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Technology_Mod = (int)Skill_TechnologyEDU_Mod.Value;
            }
        }

        private void Skill_Gulie_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Guile_Mod = (int)Skill_Gulie_Mod.Value;
            }
        }

        private void Skill_Perception_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Perception_Mod = (int)Skill_Perception_Mod.Value;
            }
        }

        #endregion

        #region Spirit

        private void Skill_Charm_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Charm_Rank = (SkillRank)Skill_Charm_Rank.SelectedItem;
            }
        }

        private void Skill_Command_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Command_Rank = (SkillRank)Skill_Command_Rank.SelectedItem;
            }
        }

        private void Skill_Focus_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Focus_Rank = (SkillRank)Skill_Focus_Rank.SelectedItem;
            }
        }

        private void Skill_Intuition_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Intuition_Rank = (SkillRank)Skill_Intuition_Rank.SelectedItem;
            }
        }

        private void Skill_Charm_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Charm_Mod = (int)Skill_Charm_Mod.Value;
            }
        }

        private void Skill_Command_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Command_Mod = (int)Skill_Command_Mod.Value;
            }
        }

        private void Skill_Focus_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Focus_Mod = (int)Skill_Focus_Mod.Value;
            }
        }

        private void Skill_Intuition_Mod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
            {
                if (PokemonData.Skills == null)
                    PokemonData.Skills = new Skill_Data();
                PokemonData.Skills.Intuition_Mod = (int)Skill_Intuition_Mod.Value;
            }
        }

        #endregion

        #endregion

        #region Moves

        private void Reload_Moves()
        {
            if (PokemonData.Moves == null)
            {
                PokemonData.Moves = new List<string>();
                return;
            }

            Moves_List.Items.Clear();
            var Effects_Moves = new Dictionary<string, object>();
            foreach (var move in PokemonData.Moves)
            {
                var db = new Moves_DB(Manager.SaveData.PokedexData.Moves.Find(x => x.Name.ToLower() == move.ToLower()));
                Effects_Moves.Add(move, move);
                Moves_List.Items.Add(db);
            }
            Status_Volatile_Disable_Value.ItemsSource = Effects_Moves;
        }

        private void Moves_Add_Click(object sender, RoutedEventArgs e)
        {
            Select_Move Move;

            try
            {
                Move = new Select_Move(Manager.SaveData.PokedexData,
                    Manager.SaveData.PokedexData.Pokemon_GetID(PokemonData.Species_DexID));
            }
            catch
            {
                Move = new Select_Move(Manager.SaveData.PokedexData);
            }

            var pass = Move.ShowDialog();

            if (pass == true)
            {
                if (PokemonData.Moves == null)
                    PokemonData.Moves = new List<string>();

                PokemonData.Moves.Add(Move.Selected_Move.Name);
                Reload_Moves();
            }
        }

        private void Moves_Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PokemonData.Moves.Remove(((Moves_DB)Moves_List.SelectedItem).Name);
                Reload_Moves();
                Status_Volatile_Disable_Value.SelectedItems.Remove(((Moves_DB)Moves_List.SelectedItem).Name);
                Status_Volatile_Disable_Value.ItemsSource.Remove(((Moves_DB)Moves_List.SelectedItem).Name);
            }
            catch
            {
                /* Ignore */
            }
        }

        #endregion

        #region Battle Effects

        #region Status Effects

        private string Status_Volitile_Inflatuation_PokemonID = "";

        public void LoadStatusEffects()
        {
            Status_Persistant_Burned.IsChecked = PokemonData.HasStatus(Status_Afflictions.Burned);
            Status_Persistant_Frozen.IsChecked = PokemonData.HasStatus(Status_Afflictions.Frozen);
            Status_Persistant_Paralysis.IsChecked = PokemonData.HasStatus(Status_Afflictions.Paralysis);
            Status_Persistant_Poisioned.IsChecked = PokemonData.HasStatus(Status_Afflictions.Poision);

            Status_Volatile_BadSleep.IsChecked = PokemonData.HasStatus(Status_Afflictions.BadSleep);
            Status_Volatile_Confused.IsChecked = PokemonData.HasStatus(Status_Afflictions.Confusion);
            Status_Volatile_Cursed.IsChecked = PokemonData.HasStatus(Status_Afflictions.Cursed);
            Status_Volatile_Disable.IsChecked = PokemonData.HasStatus(Status_Afflictions.Dissabled);
            Status_Volatile_Rage.IsChecked = PokemonData.HasStatus(Status_Afflictions.Rage);
            Status_Volatile_Flinch.IsChecked = PokemonData.HasStatus(Status_Afflictions.Flinch);
            Status_Volatile_Inflatuation.IsChecked = PokemonData.HasStatus(Status_Afflictions.Inflatuation);
            Status_Volatile_Sleep.IsChecked = PokemonData.HasStatus(Status_Afflictions.Sleep);
            Status_Volatile_Suppressed.IsChecked = PokemonData.HasStatus(Status_Afflictions.Suppressed);
            Status_Volatile_TemporaryHitPoints.IsChecked = PokemonData.HasStatus(Status_Afflictions.TemporaryHitPoints);

            Status_Other_Fainted.IsChecked = PokemonData.HasStatus(Status_Afflictions.Fainted);
            Status_Other_Blindness.IsChecked = PokemonData.HasStatus(Status_Afflictions.Blindness);
            Status_Other_TotalBlindness.IsChecked = PokemonData.HasStatus(Status_Afflictions.TotalBlindness);
            Status_Other_Slowed.IsChecked = PokemonData.HasStatus(Status_Afflictions.Slowed);
            Status_Other_Stuck.IsChecked = PokemonData.HasStatus(Status_Afflictions.Stuck);
            Status_Other_Trapped.IsChecked = PokemonData.HasStatus(Status_Afflictions.Trapped);
            Status_Other_Tripped.IsChecked = PokemonData.HasStatus(Status_Afflictions.Tripped);
            Status_Other_Vulnerable.IsChecked = PokemonData.HasStatus(Status_Afflictions.Vulnerable);

            try
            {
                Status_Volitile_Inflatuation_PokemonID =
                    (string)PokemonData.GetStatusData(Status_Afflictions.Inflatuation);
                Status_Volatile_Inflatuation_Value.Content = Manager.SaveData.Pokemon
                    .Find(x => x.ID == Status_Volitile_Inflatuation_PokemonID).Name;
            }
            catch
            {
            }

            try
            {
                Status_Volatile_Sleep_Duration.Value = (int)PokemonData.GetStatusData(Status_Afflictions.Sleep);
            }
            catch
            {
            }

            try
            {
                Status_Volatile_TemporaryHitPoints_Value.Value =
                    (int)PokemonData.GetStatusData(Status_Afflictions.TemporaryHitPoints);
            }
            catch
            {
            }

            try
            {
                var dis_moves = new Dictionary<string, object>();
                foreach (var move in (List<string>)PokemonData.GetStatusData(Status_Afflictions.Dissabled))
                    if (PokemonData.Moves.Contains(move))
                        dis_moves.Add(move, move);
                Status_Volatile_Disable_Value.SelectedItems = dis_moves;
            }
            catch
            {
            }
        }

        #region Persistant Effects

        private void Status_Persistant_Burned_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Burned);
        }

        private void Status_Persistant_Burned_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Burned);
        }

        private void Status_Persistant_Frozen_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Frozen);
        }

        private void Status_Persistant_Frozen_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Frozen);
        }

        private void Status_Persistant_Paralysis_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Paralysis);
        }

        private void Status_Persistant_Paralysis_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Paralysis);
        }

        private void Status_Persistant_Poisioned_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Poision);
        }

        private void Status_Persistant_Poisioned_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Poision);
        }

        #endregion

        #region Volitile Effects

        private void Status_Volatile_BadSleep_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.BadSleep);
        }

        private void Status_Volatile_BadSleep_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.BadSleep);
        }

        private void Status_Volatile_Confused_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Confusion);
        }

        private void Status_Volatile_Confused_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Confusion);
        }

        private void Status_Volatile_Cursed_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Cursed);
        }

        private void Status_Volatile_Cursed_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Cursed);
        }

        private void Status_Volatile_Disable_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
            {
                var Moves = new List<string>();
                try
                {
                    foreach (var data in Status_Volatile_Disable_Value.SelectedItems)
                        Moves.Add(data.Key);
                }
                catch
                {
                }

                PokemonData.AddStatus(Status_Afflictions.Dissabled, Moves);
            }
            Status_Volatile_Disable_Value.IsEnabled = true;
        }

        private void Status_Volatile_Disable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Dissabled);
            Status_Volatile_Disable_Value.IsEnabled = false;
            Status_Volatile_Disable_Value.SelectedItems = null;
        }

        private void Status_Volatile_Rage_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Rage);
        }

        private void Status_Volatile_Rage_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Rage);
        }

        private void Status_Volatile_Flinch_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Flinch);
        }

        private void Status_Volatile_Flinch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Flinch);
        }

        private void Status_Volatile_Inflatuation_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Inflatuation, Status_Volitile_Inflatuation_PokemonID);
            Status_Volatile_Inflatuation_Select.IsEnabled = true;
        }

        private void Status_Volatile_Inflatuation_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Inflatuation);
            Status_Volatile_Inflatuation_Select.IsEnabled = false;
            Status_Volatile_Inflatuation_Value.Content = "No Selected Entities";
            Status_Volitile_Inflatuation_PokemonID = "";
        }

        private void Status_Volatile_Sleep_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Sleep, Status_Volatile_Sleep_Duration.Value);
            Status_Volatile_Sleep_Duration.IsEnabled = true;
        }

        private void Status_Volatile_Sleep_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Sleep);
            Status_Volatile_Sleep_Duration.IsEnabled = false;
            Status_Volatile_Sleep_Duration.Value = 0;
        }

        private void Status_Volatile_Suppressed_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Suppressed);
        }

        private void Status_Volatile_Suppressed_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Suppressed);
        }

        private void Status_Volatile_TemporaryHitPoints_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.TemporaryHitPoints,
                    Status_Volatile_TemporaryHitPoints_Value.Value);
            Status_Volatile_TemporaryHitPoints_Value.IsEnabled = true;
        }

        private void Status_Volatile_TemporaryHitPoints_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.TemporaryHitPoints);
            Status_Volatile_TemporaryHitPoints_Value.IsEnabled = false;
            Status_Volatile_TemporaryHitPoints_Value.Value = 0;
        }

        private void Status_Volatile_Inflatuation_Select_Click(object sender, RoutedEventArgs e)
        {
            var select = new Select(Manager);
            var pass = select.ShowDialog();

            if (pass == true)
            {
                Status_Volitile_Inflatuation_PokemonID = select.SelectedPokemon.ID;
                Status_Volatile_Inflatuation_Value.Content = select.SelectedPokemon.Name;
                PokemonData.SetStatusData(Status_Afflictions.Inflatuation, select.SelectedPokemon.ID);
            }
        }

        #endregion

        #region Other Effects

        private void Status_Other_Fainted_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Fainted);
        }

        private void Status_Other_Fainted_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Fainted);
        }

        private void Status_Other_Blindness_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Blindness);
        }

        private void Status_Other_Blindness_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Blindness);
        }

        private void Status_Other_TotalBlindness_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.TotalBlindness);
        }

        private void Status_Other_TotalBlindness_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.TotalBlindness);
        }

        private void Status_Other_Slowed_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Slowed);
        }

        private void Status_Other_Slowed_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Slowed);
        }

        private void Status_Other_Stuck_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Stuck);
        }

        private void Status_Other_Stuck_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Stuck);
        }

        private void Status_Other_Trapped_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Trapped);
        }

        private void Status_Other_Trapped_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Trapped);
        }

        private void Status_Other_Tripped_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Tripped);
        }

        private void Status_Other_Tripped_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Tripped);
        }

        private void Status_Other_Vulnerable_Checked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.AddStatus(Status_Afflictions.Vulnerable);
        }

        private void Status_Other_Vulnerable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Ready)
                PokemonData.RemoveStatus(Status_Afflictions.Vulnerable);
        }

        #endregion

        private void Status_Volatile_Disable_Value_SelectionChangedEvent()
        {
            if (Ready)
            {
                var dis_moves = new List<string>();
                foreach (var obj in Status_Volatile_Disable_Value.SelectedItems)
                    dis_moves.Add((string)obj.Value);
                PokemonData.SetStatusData(Status_Afflictions.Dissabled, dis_moves);
            }
        }

        private void Status_Volatile_Sleep_Duration_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                PokemonData.SetStatusData(Status_Afflictions.Sleep, (int)Status_Volatile_Sleep_Duration.Value);
        }

        private void Status_Volatile_TemporaryHitPoints_Value_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            if (Ready)
                PokemonData.SetStatusData(Status_Afflictions.TemporaryHitPoints,
                    (int)Status_Volatile_TemporaryHitPoints_Value.Value);
        }

        #endregion

        #endregion
    }

    public class Moves_DB
    {
        public Moves_DB(MoveData Move)
        {
            Name = Move.Name;

            var RangeSB = "";
            var RangeSBS = 0;
            foreach (var rd in Move.Range_Data)
            {
                if (RangeSBS >= 1)
                    RangeSB = RangeSB + ", ";

                RangeSB = RangeSB + rd.Range;

                //if (rd.Range == BattleManager.Data.Move_Range) { }
            }
            Range = RangeSB;
            Type = Move.Move_Type.ToString();
            Class = Move.Move_Class.ToString();
            DB = (int)Move.Move_DamageBase;
            AC = Move.Move_Accuracy;

            Frequency = Move.Move_Frequency.ToString();
            if (Move.Move_Frequency == Move_Frequency.Daily || Move.Move_Frequency == Move_Frequency.Scene ||
                Move.Move_Frequency == Move_Frequency.Static)
                Frequency = Frequency + " (Limit: " + Move.Move_Frequency_Limit + ")";
        }

        public string Name { get; }
        public string Range { get; }
        public string Type { get; }
        public string Class { get; }
        public int DB { get; }
        public int AC { get; }
        public string Frequency { get; }
    }

    public class Permissions_DB
    {
        private readonly PokemonCharacter PokemonData;

        public Permissions_DB(PokemonCharacter _PokemonData, User User)
        {
            PokemonData = _PokemonData;

            ID = User.UserID;
            Name = User.Name;
            ICN = User.IC_Name;
        }

        public bool View
        {
            get
            {
                return PokemonData.View.Contains(ID);
            }
            set
            {
                if (value)
                {
                    PokemonData.View.RemoveAll(x => x == ID);
                    PokemonData.View.Add(ID);
                }
                else
                {
                    PokemonData.View.RemoveAll(x => x == ID);
                }
            }
        }

        public bool Edit
        {
            get
            {
                return PokemonData.Edit.Contains(ID);
            }
            set
            {
                if (value)
                {
                    PokemonData.Edit.RemoveAll(x => x == ID);
                    PokemonData.Edit.Add(ID);
                }
                else
                {
                    PokemonData.Edit.RemoveAll(x => x == ID);
                }
            }
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string ICN { get; set; }
    }
}