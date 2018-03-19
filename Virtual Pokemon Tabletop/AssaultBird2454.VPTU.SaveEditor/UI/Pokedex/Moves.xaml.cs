using System;
using System.Collections.Generic;
using System.Windows;
using AssaultBird2454.VPTU.BattleManager.Data;
using AssaultBird2454.VPTU.ContestManager.Data;
using AssaultBird2454.VPTU.Pokedex.Moves;
using AssaultBird2454.VPTU.Pokedex.Pokemon;
using AssaultBird2454.VPTU.SaveManager.Data.SaveFile;

namespace AssaultBird2454.VPTU.SaveEditor.UI.Pokedex
{
    /// <summary>
    ///     Interaction logic for Moves.xaml
    /// </summary>
    public partial class Moves : Window
    {
        public MoveData MoveData;
        private readonly PTUSaveData SaveData;
        private readonly bool Update;

        public Moves(PTUSaveData _SaveData, MoveData _MoveData = null)
        {
            InitializeComponent(); // Creates the Window

            SaveData = _SaveData;

            Setup(); // Executes the setup script

            if (_MoveData == null)
            {
                MoveData = new MoveData(); // If no specified data, then create a new data class
            }
            else
            {
                Update = true;
                MoveData = _MoveData; // Save the specified data class to a variable for latter
                Load(); // Load the data
            }
        }

        /// <summary>
        ///     Set's up the form
        /// </summary>
        public void Setup()
        {
            #region Data Binding

            Battle_ActionType.ItemsSource = Enum.GetValues(typeof(Action_Type));
            Battle_Class.ItemsSource = Enum.GetValues(typeof(MoveClass));
            Battle_Frequency.ItemsSource = Enum.GetValues(typeof(Move_Frequency));
            Battle_Type.ItemsSource = Enum.GetValues(typeof(Type));

            Contest_Effect.ItemsSource = Enum.GetValues(typeof(Contest_Effects));
            Contest_Type.ItemsSource = Enum.GetValues(typeof(Contest_Type));

            #endregion

            #region Data Defaulting

            Battle_ActionType.SelectedIndex = 0;
            Battle_Class.SelectedIndex = 0;
            Battle_Frequency.SelectedIndex = 0;
            Battle_Type.SelectedIndex = 0;

            Contest_Effect.SelectedIndex = 0;
            Contest_Type.SelectedIndex = 0;

            #endregion
        }

        /// <summary>
        ///     Loads the data from the MoveData Object
        /// </summary>
        public void Load()
        {
            #region Basic Info

            Basic_Name.Text = MoveData.Name;
            Basic_Desc.Text = MoveData.Description;

            #endregion

            #region Battle Details

            Battle_ActionType.SelectedItem = MoveData.Move_ActionType;
            Battle_Class.SelectedItem = MoveData.Move_Class;
            Battle_Frequency.SelectedItem = MoveData.Move_Frequency;
            Battle_Type.SelectedItem = MoveData.Move_Type;

            Battle_AC.Value = MoveData.Move_Accuracy;
            Battle_DB.Value = (int) MoveData.Move_DamageBase;
            Battle_UseLimit.Value = MoveData.Move_Frequency_Limit;

            #endregion

            #region Contest Details

            Contest_Effect.SelectedItem = MoveData.Contest_Effect;
            Contest_Type.SelectedItem = MoveData.Contest_Type;

            #endregion

            #region Range Data

            if (MoveData.Range_Data == null) MoveData.Range_Data = new List<Move_RangeData>();

            #region All Adjacent Foes

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.All_Adjacent_Foes);
                AllAdj_Enabled.IsChecked = Data.Enabled;
            }
            catch
            {
            }

            #endregion

            #region Any Target

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Any);
                AnyTarget_Enabled.IsChecked = Data.Enabled;
            }
            catch
            {
            }

            #endregion

            #region Burst

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Burst);
                Burst_Enabled.IsChecked = Data.Enabled;
                Burst_Size.Value = Data.Size;
            }
            catch
            {
            }

            #endregion

            #region Cardinally Adjacent

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Cardinally_Adjacent);
                CardinallyAdj_Enabled.IsChecked = Data.Enabled;
            }
            catch
            {
            }

            #endregion

            #region Close Blast

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Close_Blast);
                CloseBlast_Enabled.IsChecked = Data.Enabled;
                CloseBlast_Size.Value = Data.Size;
            }
            catch
            {
            }

            #endregion

            #region Cone

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Cone);
                Cone_Enabled.IsChecked = Data.Enabled;
                Cone_Size.Value = Data.Size;
            }
            catch
            {
            }

            #endregion

            #region Field

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Field);
                Field_Enabled.IsChecked = Data.Enabled;
            }
            catch
            {
            }

            #endregion

            #region Line

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Line);
                Line_Enabled.IsChecked = Data.Enabled;
                Line_Size.Value = Data.Size;
            }
            catch
            {
            }

            #endregion

            #region Mele

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Melee);
                Mele_Enabled.IsChecked = Data.Enabled;
            }
            catch
            {
            }

            #endregion

            #region Range

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Range);
                Range_Enabled.IsChecked = Data.Enabled;
                Range_Distance.Value = Data.Distance;
            }
            catch
            {
            }

            #endregion

            #region Ranged Blast

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Range_Blast);
                RangedBlast_Enabled.IsChecked = Data.Enabled;
                RangedBlast_Distance.Value = Data.Distance;
                RangedBlast_Size.Value = Data.Size;
            }
            catch
            {
            }

            #endregion

            #region Self

            try
            {
                var Data = MoveData.Range_Data.Find(x => x.Range == Move_Range.Self);
                Self_Enabled.IsChecked = Data.Enabled;
            }
            catch
            {
            }

            #endregion

            #endregion

            #region KeyWords

            try
            {
                Keyword_Aura.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Aura).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Berry.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Berry).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Blessing.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Blessing).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Coat.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Coat).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Dash.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Dash).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_DubleStrike.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.DubleStrike)
                    .Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Environ.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Environ).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Execute.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Execute).Value;
            }
            catch
            {
            }
            //try { Keyword_Exhaust.IsChecked = (bool)MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Exhaust).Value; } catch { }
            try
            {
                Keyword_FiveStrike.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.FiveStrike)
                    .Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Fling.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Fling).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Friendly.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Friendly).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_GroundSource.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Groundsource)
                    .Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Hail.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Hail).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Hazard.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Hazard).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Illusion.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Illusion).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Interupt.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Interupt).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Pass.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Pass).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Pledge.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Pledge).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Powder.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Powder).Value;
            }
            catch
            {
            }
            //try { Keyword_Priority_Type.SelectedItem = MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Priority).Value; } catch { }
            if (MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Priority).Value != null)
                Keyword_Priority.IsChecked = true;
            try
            {
                Keyword_Push.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Push).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Rainy.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Rainy).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Reaction.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Reaction).Value;
            }
            catch
            {
            }
            if (MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Recoil).Value != null)
                Keyword_Recoil.IsChecked = true;
            //try { Keyword_Recoil_Type.SelectedItem = MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Recoil).Value; } catch { }
            //try { Keyword_SandStorm.IsChecked = (bool)MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.SandStorm).Value; } catch { }
            try
            {
                Keyword_Setup.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.SetUp).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Shield.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Shield).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Smite.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Smite).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Social.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Social).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Sonic.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Sonic).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_SpiritSurge.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.SpiritSurge)
                    .Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Sunny.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Sunny).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Trigger.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Trigger).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Vortex.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Vortex).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_Weather.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.Weather).Value;
            }
            catch
            {
            }
            try
            {
                Keyword_WeightClass.IsChecked = (bool) MoveData.KeyWords.Find(x => x.Key == Move_KeyWords.WeightClass)
                    .Value;
            }
            catch
            {
            }

            #endregion
        }

        /// <summary>
        ///     Saves the data to the MoveData Object
        /// </summary>
        public void Save()
        {
            var OldName = MoveData.Name; // Gets the current name before saving for updateing later.

            #region Basic Info

            MoveData.Name = Basic_Name.Text;
            MoveData.Description = Basic_Desc.Text;

            #endregion

            #region Battle Details

            MoveData.Move_ActionType = (Action_Type) Battle_ActionType.SelectedItem;
            MoveData.Move_Class = (MoveClass) Battle_Class.SelectedItem;
            MoveData.Move_Frequency = (Move_Frequency) Battle_Frequency.SelectedItem;
            MoveData.Move_Type = (string) Battle_Type.SelectedItem;

            MoveData.Move_Accuracy = (int) Battle_AC.Value;
            MoveData.Move_DamageBase = (DamageBase) Battle_DB.Value;
            MoveData.Move_Frequency_Limit = (int) Battle_UseLimit.Value;

            #endregion

            #region Contest Details

            MoveData.Contest_Effect = (Contest_Effects) Contest_Effect.SelectedItem;
            MoveData.Contest_Type = (Contest_Type) Contest_Type.SelectedItem;

            #endregion

            #region Range Data

            MoveData.Range_Data = new List<Move_RangeData>();

            #region All Adjacent Foes

            try
            {
                if ((bool) AllAdj_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.All_Adjacent_Foes, (bool) AllAdj_Enabled.IsChecked, 0, 0);
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Any Target

            try
            {
                if ((bool) AnyTarget_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Any, (bool) AnyTarget_Enabled.IsChecked, 0, 0);
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Burst

            try
            {
                if ((bool) Burst_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Burst, (bool) Burst_Enabled.IsChecked, 0,
                        Convert.ToInt32(decimal.Floor((decimal) Burst_Size.Value)));
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Cardinally Adjacent

            try
            {
                if ((bool) CardinallyAdj_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Cardinally_Adjacent,
                        (bool) CardinallyAdj_Enabled.IsChecked, 0, 0);
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Close Blast

            try
            {
                if ((bool) CloseBlast_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Close_Blast, (bool) CloseBlast_Enabled.IsChecked, 0,
                        Convert.ToInt32(decimal.Floor((decimal) CloseBlast_Size.Value)));
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Cone

            try
            {
                if ((bool) Cone_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Cone, (bool) Cone_Enabled.IsChecked, 0,
                        Convert.ToInt32(decimal.Floor((decimal) Cone_Size.Value)));
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Field

            try
            {
                if ((bool) Field_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Field, (bool) Field_Enabled.IsChecked, 0, 0);
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Line

            try
            {
                if ((bool) Line_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Line, (bool) Line_Enabled.IsChecked,
                        Convert.ToInt32(decimal.Floor((decimal) Line_Size.Value)), 0);
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Mele

            try
            {
                if ((bool) Mele_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Melee, (bool) Mele_Enabled.IsChecked, 0, 0);
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Range

            try
            {
                if ((bool) Range_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Range, (bool) Range_Enabled.IsChecked, 0,
                        Convert.ToInt32(decimal.Floor((decimal) Range_Distance.Value)));
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Ranged Blast

            try
            {
                if ((bool) RangedBlast_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Range_Blast, (bool) RangedBlast_Enabled.IsChecked,
                        Convert.ToInt32(decimal.Floor((decimal) RangedBlast_Size.Value)),
                        Convert.ToInt32(decimal.Floor((decimal) RangedBlast_Distance.Value)));
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #region Self

            try
            {
                if ((bool) Self_Enabled.IsChecked)
                {
                    var Data = new Move_RangeData(Move_Range.Self, (bool) Self_Enabled.IsChecked, 0, 0);
                    MoveData.Range_Data.Add(Data);
                }
            }
            catch
            {
            }

            #endregion

            #endregion

            #region Keywords

            if (MoveData.KeyWords == null)
                MoveData.KeyWords = new List<KeyValuePair<Move_KeyWords, object>>();
            else
                MoveData.KeyWords.Clear();

            if (Keyword_Aura.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Aura, true));
            if (Keyword_Berry.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Berry, true));
            if (Keyword_Blessing.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Blessing, true));
            if (Keyword_Coat.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Coat, true));
            if (Keyword_Dash.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Dash, true));
            if (Keyword_DubleStrike.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.DubleStrike, true));
            if (Keyword_Environ.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Environ, true));
            if (Keyword_Execute.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Execute, true));
            //if (Keyword_Exhaust.IsChecked == true) { MoveData.KeyWords.Add(new KeyValuePair<VPTU.BattleManager.Data.Move_KeyWords, object>(VPTU.BattleManager.Data.Move_KeyWords.Exhaust, true)); }
            if (Keyword_FiveStrike.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.FiveStrike, true));
            if (Keyword_Fling.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Fling, true));
            if (Keyword_Friendly.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Friendly, true));
            if (Keyword_GroundSource.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Groundsource, true));
            if (Keyword_Hail.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Hail, true));
            if (Keyword_Hazard.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Hazard, true));
            if (Keyword_Illusion.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Illusion, true));
            if (Keyword_Interupt.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Interupt, true));
            if (Keyword_Pass.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Pass, true));
            if (Keyword_Pledge.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Pledge, true));
            if (Keyword_Powder.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Powder, true));
            //if (Keyword_Priority.IsChecked == true) { MoveData.KeyWords.Add(new KeyValuePair<VPTU.BattleManager.Data.Move_KeyWords, object>(VPTU.BattleManager.Data.Move_KeyWords.Priority, (Priority)Keyword_Priority_Type.SelectedItem)); }
            if (Keyword_Push.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Push, true));
            if (Keyword_Rainy.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Rainy, true));
            if (Keyword_Reaction.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Reaction, true));
            //if (Keyword_Recoil.IsChecked == true) { MoveData.KeyWords.Add(new KeyValuePair<VPTU.BattleManager.Data.Move_KeyWords, object>(VPTU.BattleManager.Data.Move_KeyWords.Recoil, (Recoil)Keyword_Recoil_Type.SelectedItem)); }
            //if (Keyword_SandStorm.IsChecked == true) { MoveData.KeyWords.Add(new KeyValuePair<VPTU.BattleManager.Data.Move_KeyWords, object>(VPTU.BattleManager.Data.Move_KeyWords.SandStorm, true)); }
            if (Keyword_Setup.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.SetUp, true));
            if (Keyword_Shield.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Shield, true));
            if (Keyword_Smite.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Smite, true));
            if (Keyword_Social.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Social, true));
            if (Keyword_Sonic.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Sonic, true));
            if (Keyword_SpiritSurge.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.SpiritSurge, true));
            if (Keyword_Sunny.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Sunny, true));
            if (Keyword_Trigger.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Trigger, true));
            if (Keyword_Vortex.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Vortex, true));
            if (Keyword_Weather.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.Weather, true));
            if (Keyword_WeightClass.IsChecked == true)
                MoveData.KeyWords.Add(new KeyValuePair<Move_KeyWords, object>(Move_KeyWords.WeightClass, true));

            #endregion

            //Update links in different parts of the save data

            #region Update

            try
            {
                if (Update)
                    if (MoveData.Name != OldName)
                        foreach (var pokemon in SaveData.PokedexData.Pokemon_List())
                        {
                            if (pokemon.Moves == null)
                            {
                                pokemon.Moves = new List<Link_Moves>();
                                continue;
                            }

                            var moves = pokemon.Moves.FindAll(x => x.MoveName.ToLower() == OldName.ToLower());
                            if (moves != null)
                                foreach (var move in moves)
                                    move.MoveName = MoveData.Name;
                        }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating Move Links");
                MessageBox.Show(ex.ToString());
            }

            #endregion
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            Save(); // Saves Data

            try
            {
                DialogResult = true;
            }
            catch
            {
            } // Sets the resault to true (Means Pass) & the error means it is not a dialogue
            Close(); // Closes the dialogue
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Save(); // Saves Data
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch
            {
            } // Sets the resault to true (Means Fail / Canceled) & the error means it is not a dialogue
            Close(); // Closes the dialogue
        }

        private void RawData_Button_Click(object sender, RoutedEventArgs e)
        {
            var impexp = new RAW_JSON();
            impexp.Export(MoveData);
            var dr = impexp.ShowDialog();

            if (dr == true)
            {
                MoveData = impexp.Import<MoveData>();
                Load();
            }
        }

        private void Effect_Designer_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}