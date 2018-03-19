using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultBird2454.VPTU.Pokedex.Save_Data
{
    /// <summary>
    /// A Save Data Class designed for pokedex related data
    /// </summary>
    public class Pokedex
    {
        SaveManager.SaveManager Manager;

        /// <summary>
        /// Creates a new Pokedex Save Data Manager
        /// </summary>
        /// <param name="InitNewSave">Initialize new data</param>
        public Pokedex(SaveManager.SaveManager _Manager)
        {
            Manager = _Manager;
        }

        #region Moves

        #endregion

        #region Pokemon
        public List<Pokemon.PokemonData> Pokemon_List()
        {
            using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Pokedex_Pokemon", Manager.Connection))
            {
                using (System.Data.SQLite.SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.HasRows)
                        return null;

                    List<Pokemon.PokemonData> Data = new List<Pokemon.PokemonData>();

                    while (dr.Read())
                    {
                        // Permissions Check
                        Pokemon.PokemonData PData = new Pokemon.PokemonData()
                        {
                            Species_DexID = dr.GetDecimal(0),
                            Species_Name = dr.GetString(1),
                            Species_Desc = dr.GetString(2),
                            Species_SizeClass = (Entities.SizeClass)Enum.Parse(typeof(Entities.SizeClass), dr.GetString(3)),
                            Species_WeightClass = (Entities.WeightClass)Enum.Parse(typeof(Entities.WeightClass), dr.GetString(4)),
                            Species_Genders = (Pokemon.PokemonGender)Enum.Parse(typeof(Pokemon.PokemonGender), dr.GetString(5)),
                            Species_Gender_Chance_Male = dr.GetDecimal(6),
                            Species_BaseStats_HP = dr.GetInt32(7),
                            Species_BaseStats_Attack = dr.GetInt32(8),
                            Species_BaseStats_Defence = dr.GetInt32(9),
                            Species_BaseStats_SpAttack = dr.GetInt32(10),
                            Species_BaseStats_SpDefence = dr.GetInt32(11),
                            Species_BaseStats_Speed = dr.GetInt32(12),
                            Sprite_Normal = dr.GetString(13),
                            Sprite_Shiny = dr.GetString(14),
                            Sprite_Egg = dr.GetString(15),
                            Sound_Cry = dr.GetString(16),
                        };

                        using (System.Data.SQLite.SQLiteCommand CD_cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Pokedex_Pokemon_CD WHERE Pokedex_Pokemon_ID = " + PData.Species_DexID, Manager.Connection))
                        {
                            using (System.Data.SQLite.SQLiteDataReader CD_dr = CD_cmd.ExecuteReader())
                            {
                                if (CD_dr.HasRows)
                                {
                                    PData.Species_Capability_Data = new Entities.Capability_Data()
                                    {

                                    };
                                }
                            }
                        }

                        using (System.Data.SQLite.SQLiteCommand SD_cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Pokedex_Pokemon_SD WHERE Pokedex_Pokemon_ID = " + PData.Species_DexID, Manager.Connection))
                        {
                            using (System.Data.SQLite.SQLiteDataReader SD_dr = SD_cmd.ExecuteReader())
                            {
                                if (SD_dr.HasRows)
                                {
                                    PData.Species_Skill_Data = new Entities.Skill_Data()
                                    {

                                    };
                                }
                            }
                        }

                        using (System.Data.SQLite.SQLiteCommand Type_cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Pokedex_Pokemon_Typing WHERE Pokedex_Pokemon_ID = " + PData.Species_DexID, Manager.Connection))
                        {
                            using (System.Data.SQLite.SQLiteDataReader Type_dr = Type_cmd.ExecuteReader())
                            {
                                if (Type_dr.HasRows)
                                {
                                    PData.Species_Types = new List<string>();

                                    while (dr.Read())
                                    {
                                        PData.Species_Types.Add(Type_dr.GetString(3));
                                    }
                                }
                            }
                        }

                        using (System.Data.SQLite.SQLiteCommand Moves_cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Pokedex_Pokemon_Moves WHERE Pokedex_Pokemon_ID = " + PData.Species_DexID, Manager.Connection))
                        {
                            using (System.Data.SQLite.SQLiteDataReader Moves_dr = Moves_cmd.ExecuteReader())
                            {
                                if (Moves_dr.HasRows)
                                {
                                    PData.Moves = new List<Pokemon.Link_Moves>();

                                    while (dr.Read())
                                    {
                                        PData.Moves.Add(new Pokemon.Link_Moves()
                                        {
                                            Egg_Move = Moves_dr.GetBoolean(3),
                                            LevelUp_Level = Moves_dr.GetInt32(2),
                                            //LevelUp_Move = dr.GetBoolean()
                                        });
                                    }
                                }
                            }
                        }

                        Data.Add(PData);
                    }

                    return Data;
                }
            }
        }

        public Pokemon.PokemonData Pokemon_GetID(decimal ID)
        {
            return null;
        }

        public void Pokemon_Add(Pokemon.PokemonData Pokemon)
        {
            new System.Data.SQLite.SQLiteCommand("INSERT INTO Pokedex_Pokemon " +
                "(`Pokedex_Pokemon_ID`, `Pokedex_Pokemon_Name`, `Pokedex_Pokemon_Desc`, `Pokedex_Pokemon_Size`, `Pokedex_Pokemon_Weight`, `Pokedex_Pokemon_Gender`, `Pokedex_Pokemon_Male_Chance`, `Pokedex_Pokemon_BS_HP`, `Pokedex_Pokemon_BS_Attack`, `Pokedex_Pokemon_BS_Defence`, `Pokedex_Pokemon_BS_SpAttack`, `Pokedex_Pokemon_BS_SpDefence`, `Pokedex_Pokemon_BS_Speed`, `Pokedex_Pokemon_RES_Normal`, `Pokedex_Pokemon_RES_Shiny`, `Pokedex_Pokemon_RES_Egg`, `Pokedex_Pokemon_RES_Cry`)" +
                " VALUES " +
                "(\"" + Pokemon.Species_DexID + "\", \"" + Pokemon.Species_Name + "\", \"" + Pokemon.Species_Desc + "\", \"" + Pokemon.Species_SizeClass + "\", \"" + Pokemon.Species_WeightClass + "\", \"" + Pokemon.Species_Genders + "\", " + Pokemon.Species_Gender_Chance_Male + ", " + Pokemon.Species_BaseStats_HP + ", " + Pokemon.Species_BaseStats_Attack + ", " + Pokemon.Species_BaseStats_Defence + ", " + Pokemon.Species_BaseStats_SpAttack + ", " + Pokemon.Species_BaseStats_SpDefence + ", " + Pokemon.Species_BaseStats_Speed + ", \"" + Pokemon.Sprite_Normal + "\", \"" + Pokemon.Sprite_Shiny + "\", \"" + Pokemon.Sprite_Egg + "\", \"" + Pokemon.Sound_Cry + "\");", Manager.Connection).ExecuteNonQuery();

            new System.Data.SQLite.SQLiteCommand("INSERT INTO Pokedex_Pokemon_CD " +
                "(`Pokedex_Pokemon_ID`, `Pokedex_CD_Power`, `Pokedex_CD_ThrowingRange`, `Pokedex_CD_LongJump`, `Pokedex_CD_HighJump`, `Pokedex_CD_Burrow`, `Pokedex_CD_Overland`, `Pokedex_CD_Sky`, `Pokedex_CD_Swim`, `Pokedex_CD_Levitate`, `Pokedex_CD_Teleport`) " +
                "VALUES (\"" + Pokemon.Species_DexID + "\", " + Pokemon.Species_Capability_Data.Power + ", " + Pokemon.Species_Capability_Data.ThrowingRange + ", " + Pokemon.Species_Capability_Data.LongJump + ", " + Pokemon.Species_Capability_Data.HighJump + ", " + Pokemon.Species_Capability_Data.Burrow + ", " + Pokemon.Species_Capability_Data.Overland + ", " + Pokemon.Species_Capability_Data.Sky + ", " + Pokemon.Species_Capability_Data.Swim + ", " + Pokemon.Species_Capability_Data.Levitate + ", " + Pokemon.Species_Capability_Data.Teleport + ");", Manager.Connection).ExecuteNonQuery();

            new System.Data.SQLite.SQLiteCommand("INSERT INTO Pokedex_Pokemon_SD " +
                "(`Pokedex_Pokemon_ID`, `Pokedex_SD_Acro_Mod`, `Pokedex_SD_Acro_Rank`, `Pokedex_SD_Ath_Mod`, `Pokedex_SD_Ath_Rank`, `Pokedex_SD_Combat_Mod`, `Pokedex_SD_Combat_Rank`, `Pokedex_SD_Intimidate_Mod`, `Pokedex_SD_Intimidate_Rank`, `Pokedex_SD_Stealth_Mod`, `Pokedex_SD_Stealth_Rank`, `Pokedex_SD_Survival_Mod`, `Pokedex_SD_Survival_Rank`) " +
                "VALUES (" + Pokemon.Species_DexID + ", " + Pokemon.Species_Skill_Data.Acrobatics_Mod + ", " + (int)Pokemon.Species_Skill_Data.Acrobatics_Rank + ", " + Pokemon.Species_Skill_Data.Athletics_Mod + ", " + (int)Pokemon.Species_Skill_Data.Athletics_Rank + ", " + Pokemon.Species_Skill_Data.Combat_Mod + ", " + (int)Pokemon.Species_Skill_Data.Combat_Rank + ", " + Pokemon.Species_Skill_Data.Intimidate_Mod + ", " + (int)Pokemon.Species_Skill_Data.Intimidate_Rank + ", " + Pokemon.Species_Skill_Data.Stealth_Mod + ", " + (int)Pokemon.Species_Skill_Data.Stealth_Rank + ", " + Pokemon.Species_Skill_Data.Survival_Mod + ", " + (int)Pokemon.Species_Skill_Data.Survival_Rank + ");", Manager.Connection).ExecuteNonQuery();
        }

        public void Pokemon_Remove(Pokemon.PokemonData Pokemon)
        {
            new System.Data.SQLite.SQLiteCommand("DELETE FROM Pokedex_Pokemon_SD WHERE `Pokedex_Pokemon_ID` = " + Pokemon.Species_DexID + ";" +
                "DELETE FROM Pokedex_Pokemon_CD WHERE `Pokedex_Pokemon_ID` = " + Pokemon.Species_DexID + ";" +
                "DELETE FROM Pokedex_Pokemon WHERE `Pokedex_Pokemon_ID` = " + Pokemon.Species_DexID + ";", Manager.Connection).ExecuteNonQuery();
        }
        #endregion

        #region Abilities

        #endregion

        #region Items

        #endregion

        /// <summary>
        /// All the Moves in the save file
        /// </summary>
        public List<Moves.MoveData> Moves;
        /// <summary>
        /// All the Abilitys in the save file
        /// </summary>
        public List<Abilitys.AbilityData> Abilitys;
        /// <summary>
        /// All the Items in the save file
        /// </summary>
        public List<Items.ItemData> Items;
    }
}
