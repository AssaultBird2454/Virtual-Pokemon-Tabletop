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
            Manager = new AssaultBird2454.VPTU.SaveManager.SaveManager(Console.ReadLine());

            while (true)
            {
                Console.WriteLine("Loaded VPTUDBS @ " + Manager.SaveFileDir);

                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("Enter Opperation");
                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("1: Save Migration Services");
                Console.WriteLine("2: Save Upgrade Services");// -> (Not Implemented)
                Console.WriteLine("3: Data Migration");
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
                    else if (code == 3)
                    {
                        DataMigration_Migrate(Manager);
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
                Console.WriteLine("Loaded VPTUDBS @ " + Manager.SaveFileDir);

                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("Enter Migration Opperation");
                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("1: Migrate Pokedex Data (Pokemon)");
                Console.WriteLine("2: Migrate Pokedex Data (Moves)");
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
                        Convert_PokedexPokemon(Manager);
                    }
                }
                catch { }
            }
        }

        #region Pokedex Migration
        static void Convert_PokedexPokemon(AssaultBird2454.VPTU.SaveManager.SaveManager Manager)
        {
            List<AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData> Pokemon = new List<AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData>();

            Console.Write("Path to \"Pokedex/Pokemon.json\"");
            FileStream FS = new FileStream(Console.ReadLine(), FileMode.Open);
            StreamReader Reader = new StreamReader(FS);

            Pokemon = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData>>(Reader.ReadToEnd());

            foreach (AssaultBird2454.VPTU.Pokedex.Pokemon.PokemonData data in Pokemon)
            {
                //Manager.SaveData.PokedexData.
            }
        }
        #endregion
        #endregion

        #region Data Migration
        static void DataMigration_Migrate(AssaultBird2454.VPTU.SaveManager.SaveManager Manager)
        {
            while (true)
            {
                Console.WriteLine("Loaded VPTUDBS @ " + Manager.SaveFileDir);

                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("Enter Migration Opperation");
                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                Console.WriteLine("1: Migrate JSON Data (cwstra/rpdiscordbot)");
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
                        DM_cwstra_rpdiscordbot_Moves(Manager);
                    }
                }
                catch { }
            }
        }

        #region DM - cwstra/rpdiscordbot
        static void DM_cwstra_rpdiscordbot_Moves(AssaultBird2454.VPTU.SaveManager.SaveManager Manager)
        {
            string[] Data = "\nType: Fighting\nFreq: Scene\nClass: Status\nRange: Self, Trigger, Interrupt, Shield\nEffect: If the user is hit by a Move, the user may use Detect. The user is instead not hit by the Move. You do not take any damage nor are you affected by anty of the Move's effects\nContest Type: Cool\nContest Effect: Inversed Appeal".Split('\n');

            foreach(string d in Data)
            {
                Console.WriteLine(d);
            }

            //AssaultBird2454.VPTU.Pokedex.Moves.MoveData MD = new AssaultBird2454.VPTU.Pokedex.Moves.MoveData();
        }
        #endregion
        #endregion
    }
}
