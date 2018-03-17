using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_FIle_Migration
{
    class Program
    {
        static AssaultBird2454.VPTU.SaveManager.SaveManager Manager;

        static void Main(string[] args)
        {
            Console.Write("Save File Path: ");
            Manager = new AssaultBird2454.VPTU.SaveManager.SaveManager(Console.ReadLine());
            Manager.Load_SaveData();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Loaded VPTUDBS @ " + Manager.SaveFileDir);

                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("Enter Opperation");
                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("1: Save Migration Services");
                //Console.WriteLine("2: Save Upgrade Services");// -> (Not Implemented)
                Console.WriteLine();
                Console.WriteLine("0: Quit");
                Console.WriteLine();
                Console.Write("Enter Opperation Code: ");

                try
                {
                    int code = Convert.ToInt32(Console.ReadLine());

                    if (code == 0)
                    {
                        break;
                    }
                    else if (code == 1)
                    {
                        SaveMigration(Manager);
                    }
                }
                catch { }
            }
        }

        #region Save Migration
        static void SaveMigration(AssaultBird2454.VPTU.SaveManager.SaveManager Manager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Loaded VPTUDBS @ " + Manager.SaveFileDir);

                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("Enter Migration Opperation");
                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("1: Migrate Pokedex Data (Pokemon)");
                Console.WriteLine("2: Migrate Pokedex Data (Moves)");
                Console.WriteLine("3: Migrate Typing Data");
                Console.WriteLine("4: Migrate Resources");
                Console.WriteLine();
                Console.WriteLine("0: Go Back");
                Console.WriteLine();
                Console.Write("Enter Opperation Code: ");

                try
                {
                    int code = Convert.ToInt32(Console.ReadLine());

                    if (code == 0)
                    {
                        break;
                    }
                    else if (code == 1)
                    {
                        Console.Clear();
                        Convert_PokedexPokemon(Manager);

                        Console.WriteLine("Done... Press any key to continue");
                        Console.ReadKey();
                    }
                    else if (code == 3)
                    {

                    }
                    else if (code == 4)
                    {

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.ReadKey();
                }
            }
        }

        #region Pokedex Migration
        static void Convert_PokedexPokemon(AssaultBird2454.VPTU.SaveManager.SaveManager Manager)
        {
            List<AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData> Pokemon = new List<AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData>();

            Console.Write("Path to \"Pokedex/Pokemon.json: \"");
            FileStream FS = new FileStream(Console.ReadLine(), FileMode.Open);
            StreamReader Reader = new StreamReader(FS);

            Pokemon = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData>>(Reader.ReadToEnd());

            foreach (AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData data in Pokemon)
            {
                Console.WriteLine("Migrating Pokemon: " + data.Species_DexID + " (" + data.Species_Name + ")");
                Manager.SaveData.PokedexData.Pokemon_Add(data);
            }
        }

        // This method has code that needs to be moved into the savemanager
        static void Convert_Battle_Typing(AssaultBird2454.VPTU.SaveManager.SaveManager Manager)
        {
            AssaultBird2454.VPTU.BattleManager.Typing.Manager TypingManager;

            Console.Write("Path to \"Battle/TypingData.json\"");
            FileStream FS = new FileStream(Console.ReadLine(), FileMode.Open);
            StreamReader Reader = new StreamReader(FS);

            TypingManager = Newtonsoft.Json.JsonConvert.DeserializeObject<AssaultBird2454.VPTU.BattleManager.Typing.Manager>(Reader.ReadToEnd());

            foreach (AssaultBird2454.VPTU.BattleManager.Typing.Typing_Data Data in TypingManager.Types)
            {
                new System.Data.SQLite.SQLiteCommand("INSERT INTO Battle_Typing (`Battle_Typing_Name`) VALUES (\"" + Data.Type_Name + "\");", Manager.Connection).ExecuteNonQuery();

                foreach (string SuperEffective in Data.Effect_SuperEffective)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Battle_Typing_Effectivity (`Battle_Typing_Attacking`, `Battle_Typing_Defending`, `Battle_Typing_Mod`) VALUES (\"" + Data.Type_Name + "\", \"" + SuperEffective + "\", 1.5);");
                }
                foreach (string NotEffective in Data.Effect_NotVery)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Battle_Typing_Effectivity (`Battle_Typing_Attacking`, `Battle_Typing_Defending`, `Battle_Typing_Mod`) VALUES (\"" + Data.Type_Name + "\", \"" + NotEffective + "\", 0.5);");
                }
                foreach (string Immune in Data.Effect_SuperEffective)
                {
                    new System.Data.SQLite.SQLiteCommand("INSERT INTO Battle_Typing_Effectivity (`Battle_Typing_Attacking`, `Battle_Typing_Defending`, `Battle_Typing_Mod`) VALUES (\"" + Data.Type_Name + "\", \"" + Immune + "\", 0);");
                }
            }
        }

        static void Convert_Resources(AssaultBird2454.VPTU.SaveManager.SaveManager Manager)
        {
            List<AssaultBird2454.VPTU.SaveManager.Resource_Data.Resources> Resources = new List<AssaultBird2454.VPTU.SaveManager.Resource_Data.Resources>();
            Console.Write("Path to \"Resources/Data.json: \"");
            FileStream FS = new FileStream(Console.ReadLine(), FileMode.Open);
            StreamReader Reader = new StreamReader(FS);

            string PathToResources = "";
            Console.Write("Path to \"Resources/Images/: \"");
            PathToResources = Console.ReadLine();

            Resources = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AssaultBird2454.VPTU.SaveManager.Resource_Data.Resources>>(Reader.ReadToEnd());
        }
        #endregion
        #endregion
    }
}
