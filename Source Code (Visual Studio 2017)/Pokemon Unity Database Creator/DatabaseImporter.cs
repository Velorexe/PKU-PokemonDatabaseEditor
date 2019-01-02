using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pokemon_Unity_Database_Creator
{
    public class DatabaseImporter
    {
        /*
         * This script needs to be rewritten using Regions
         * Seperating the most important part (Pokedex, ID, Abilities, etc.) into seperate methods
         * The return value should be the same, but it could be simplified by returning it when it's initiated
         * public class DatabaseImporter()
         * That get's called from within the PokemonUnityDatabaseCreator Import event
         */

        public int PokemonAmount = 0;
        public ProgressBar Progress { get; set; }
        public List<PokemonData> ImportDatabase()
        {
            string PokemonDatabase = "";

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                FileName = "PokemonDatabase.cs",
                Filter = "C# Files|*.cs"
            };
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                PokemonDatabase = File.ReadAllText(openFileDialog.FileName);
                return DatabaseCompiler(DatabaseSplitter(PokemonDatabase));
            }
            else
            {
                return new List<PokemonData>();
            }
        }

        string[] DatabaseSplitter(string Database)
        {
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            Database = Regex.Replace(Database,
                blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me =>
                {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                        return me.Value.StartsWith("//") ? Environment.NewLine : "";
                        return me.Value;
                },
            RegexOptions.Singleline);
            string[] Result;
            Result = Database.Split(new[] { "new PokemonData" }, StringSplitOptions.None);
            for (int i = 0; i < Result.Length; i++)
            {
                string CheckChar = Result[i];
                if (Result[i].StartsWith("(") && Char.IsDigit(CheckChar[1]))
                {
                    Result = Result.Skip(i).ToArray();
                    PokemonAmount++;
                    break;
                }
            }

            if (Result.Length == 1 && PokemonAmount == 0)
            {
                MessageBox.Show("The database you tried to import seems empty. Did you load the right database?");
            }

            Progress.Maximum = Result.Length;
            return Result;
        }

        List<PokemonData> DatabaseCompiler(string[] Database)
        {
            if (PokemonAmount == 0)
            {
                return new List<PokemonData>();
            }
            List<PokemonData> PokemonDatabase = new List<PokemonData>();

            foreach (string PokemonData in Database)
            {
                PokemonData tempPokemon = new PokemonData();

                string Pokemon = PokemonData.TrimStart('(');
                Pokemon = Pokemon.Replace("\r", "");
                Pokemon = Pokemon.Replace("\n", "");

                string[] pokemonArray = Pokemon.Split(',');

                int i = 0;
                foreach (string stat in pokemonArray)
                {
                    pokemonArray[i] = stat.Replace("\n", "");
                    i++;
                }

                tempPokemon.PokedexID = pokemonArray[0];

                tempPokemon.Name = QuoteRemove(pokemonArray[1]);

                tempPokemon.Species = QuoteRemove(pokemonArray[2]);

                tempPokemon.Type1 = ToTitleCase(TypeStripper(pokemonArray[3]));
                tempPokemon.Type2 = ToTitleCase(TypeStripper(pokemonArray[4]));

                tempPokemon.Ability1 = AbilityCheck(QuoteRemove(pokemonArray[5]));
                tempPokemon.Ability2 = AbilityCheck(QuoteRemove(pokemonArray[6]));
                tempPokemon.HiddenAbility = AbilityCheck(QuoteRemove(pokemonArray[7]));

                tempPokemon.MaleRatio = RemoveLetter(pokemonArray[8], "f");

                tempPokemon.CatchRate = RemoveLetter(pokemonArray[9], "f");

                tempPokemon.EggGroup1 = ToTitleCase(TypeStripper(pokemonArray[10]));
                tempPokemon.EggGroup2 = ToTitleCase(TypeStripper(pokemonArray[11]));

                tempPokemon.HatchTime = Convert.ToInt32(pokemonArray[12]);

                tempPokemon.Height = RemoveLetter(pokemonArray[13], "f");

                tempPokemon.Weight = RemoveLetter(pokemonArray[14], "f");

                tempPokemon.EvExp = Convert.ToInt32(pokemonArray[15]);

                tempPokemon.LevelingRate = LevelFixer(TypeStripper(pokemonArray[16]));

                tempPokemon.EvHP = Convert.ToInt32(pokemonArray[17]);
                tempPokemon.EvAttack = Convert.ToInt32(pokemonArray[18]);
                tempPokemon.EvDefense = Convert.ToInt32(pokemonArray[19]);
                tempPokemon.EvSpecialAttack = Convert.ToInt32(pokemonArray[20]);
                tempPokemon.EvSpecialDefense = Convert.ToInt32(pokemonArray[21]);
                tempPokemon.EvSpeed = Convert.ToInt32(pokemonArray[22]);

                tempPokemon.PokedexColor = ToTitleCase(TypeStripper(pokemonArray[23]).ToLower());

                tempPokemon.BaseFriendship = pokemonArray[24].Replace(" ", "");


                bool repeat = true;
                while (repeat == true)
                {
                    for (int q = 0; q < pokemonArray[27].Replace(" ", "").Length; q++)
                    {
                        if (!Char.IsDigit(pokemonArray[28].Replace(" ", "")[q]))
                        {
                            pokemonArray[26] = pokemonArray[26] + pokemonArray[27].Replace("\"", "");
                            pokemonArray = pokemonArray.Where(w => w != pokemonArray[27]).ToArray();
                            repeat = true;
                            break;
                        }
                        else if (q == pokemonArray[27].Replace(" ", "").Length - 1)
                        {
                            repeat = false;
                            break;
                        }
                    }
                }

                tempPokemon.PokedexEntry = pokemonArray[26].Replace("\"", "");
                if (tempPokemon.PokedexEntry[0] == ' ')
                {
                    tempPokemon.PokedexEntry = tempPokemon.PokedexEntry.Remove(0, 1);
                }

                tempPokemon.BaseHP = Convert.ToInt32(pokemonArray[27]);
                tempPokemon.BaseAttack = Convert.ToInt32(pokemonArray[28]);
                tempPokemon.BaseDefense = Convert.ToInt32(pokemonArray[29]);
                tempPokemon.BaseSpecialAttack = Convert.ToInt32(pokemonArray[30]);
                tempPokemon.BaseSpecialDefense = Convert.ToInt32(pokemonArray[31]);
                tempPokemon.BaseSpeed = Convert.ToInt32(pokemonArray[32]);

                tempPokemon.Luminance = RemoveLetter(pokemonArray[33], "f");
                tempPokemon.LightColor = ToTitleCase(pokemonArray[34].Replace("Color.", "")).Replace(" ", "");

                pokemonArray[35] = pokemonArray[35].Replace("new", "").Replace(" ", "").Replace("int", "").Replace("[]", "").Replace("{", "");
                int Index = 0;
                List<int> Levels = new List<int>();
                for (int a = 35; a < pokemonArray.Length - 1; a++)
                {
                    try
                    {
                        Levels.Add(Convert.ToInt32(pokemonArray[a].Replace(" ", "").Replace("}", "")));
                    }
                    catch (System.FormatException)
                    {
                        Index = a;
                        break;
                    }
                }

                int SaveB = 0;
                pokemonArray[Index] = pokemonArray[Index].Replace("new", "").Replace(" ", "").Replace("string", "").Replace("[]", "").Replace("{", "");
                List<string> Moves = new List<string>();
                for (int b = Index; b < Index + Levels.Count; b++)
                {
                    Moves.Add(SpaceBeforeCapital(pokemonArray[b].Replace("\"", "").Replace("}", "").Replace(" ", "")));
                    SaveB = b;
                }
                Index = SaveB + 1;

                int p = 0;
                foreach (int Level in Levels)
                {
                    LevelMove Move = new LevelMove
                    {
                        Level = Level,
                        Move = Moves[p]
                    };
                    tempPokemon.LevelMoves.Add(Move);
                    p++;
                }

                if (pokemonArray[Index].Replace(" ", "") == "}")
                {
                    Index++;
                }

                List<string> HmMoves = new List<string>();
                pokemonArray[Index] = pokemonArray[Index].Replace("new", "").Replace("string", "").Replace("[]", "").Replace("{", "").Replace(" ", "");
                for (int o = Index; o < pokemonArray.Length - 1; o++)
                {
                    if (pokemonArray[o].Replace(" ", "").StartsWith("newint[]"))
                    {
                        break;
                    }
                    else
                    {
                        tempPokemon.hmAndTM.Add(SpaceBeforeCapital(pokemonArray[o].Replace("\"", "").Replace(" ", "").Replace("}", "")).Replace("_", " "));
                        Index++;
                    }
                }

                tempPokemon.EvolutionID = pokemonArray[Index].Replace("new", "").Replace(" ", "").Replace("int", "").Replace("[]", "").Replace("{", "").Replace("}", "");
                Index++;
                if (tempPokemon.EvolutionID != "")
                {
                    tempPokemon.EvolutionMethod = pokemonArray[Index].Replace("new", "").Replace(" ", "").Replace("{", "").Replace("string", "").Replace("[]", "").Replace("\"", "");
                    Index++;
                    tempPokemon.EvolutionLevel = pokemonArray[Index].Replace("}", "").Replace("\"", "").Replace(")", "");
                }
                else
                {
                    tempPokemon.EvolutionMethod = "";
                    tempPokemon.EvolutionLevel = "";
                }

                PokemonDatabase.Add(tempPokemon);
                Progress.Value++;
            }

            return PokemonDatabase;
        }

        string SpaceBeforeCapital(string word)
        {
            word = Regex.Replace(word, "([a-z])([A-Z])", "$1 $2");
            return word;
        }

        string LevelFixer(string Target)
        {
            if (Target == "MEDIUMSLOW")
            {
                Target = "Medium Slow";
            }
            else if (Target == "MEDIUMFAST")
            {
                Target = "Medium Fast";
            }
            return ToTitleCase(Target);
        }

        public float RemoveLetter(string word, string replace)
        {
            word = word.Replace(replace, "");
            return (float)Convert.ToDouble(word);
        }

        public string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public string TypeStripper(string Type)
        {
            string[] Result;
            Result = Type.Split('.');
            Result = Result.Skip(2).ToArray();

            if (Result == null)
                return Result[0];
            else
                return "NONE";
        }

        public string QuoteRemove(string str)
        {
            string Result = "";
            Result = str.Replace("\"", "").Replace(" ", "");
            return Result;
        }

        public string AbilityCheck(string Ability)
        {
            string Result = "";
            if (Ability == "null")
            {
                Result = "None";
            }
            else
            {
                Result = Ability;
            }
            return Result;
        }
    }
}
