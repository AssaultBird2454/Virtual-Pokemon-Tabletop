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
    }
}
