using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pokemon_Unity_Database_Creator
{
    public class DatabaseImporter
    {
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
            string[] Result;
            Result = Database.Split(new [] { "new PokemonData" }, StringSplitOptions.None);
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

            if(Result.Length == 1 && PokemonAmount == 0)
            {
                MessageBox.Show("The database you tried to import seems empty. Did you load the right database?");
            }

            Progress.Maximum = Result.Length;
            return Result;
        }

        List<PokemonData> DatabaseCompiler(string[] Database)
        {
            if(PokemonAmount == 0)
            {
                return new List<PokemonData>();
            }
            List<PokemonData> PokemonDatabase = new List<PokemonData>();

            foreach(string PokemonData in Database)
            {
                PokemonData tempPokemon = new PokemonData();
                string Pokemon = PokemonData.TrimStart('(');
                string[] pokemonArray = Pokemon.Split(',');

                int i = 0;
                foreach(string stat in pokemonArray)
                {
                    pokemonArray[i] = stat.Replace("\n", "");
                    i++;
                }

                tempPokemon.PokedexID = pokemonArray[0];

                tempPokemon.Name = QuoteRemove(pokemonArray[1]);

                tempPokemon.Type1 = ToTitleCase(TypeStripper(pokemonArray[2]));
                tempPokemon.Type2 = ToTitleCase(TypeStripper(pokemonArray[3]));

                tempPokemon.Ability1 = AbilityCheck(QuoteRemove(pokemonArray[4]));
                tempPokemon.Ability2 = AbilityCheck(QuoteRemove(pokemonArray[5]));
                tempPokemon.HiddenAbility = AbilityCheck(QuoteRemove(pokemonArray[6]));

                tempPokemon.MaleRatio = RemoveLetter(pokemonArray[7], "f");

                tempPokemon.CatchRate = RemoveLetter(pokemonArray[8], "f");

                tempPokemon.EggGroup1 = ToTitleCase(TypeStripper(pokemonArray[9]));
                tempPokemon.EggGroup2 = ToTitleCase(TypeStripper(pokemonArray[10]));

                tempPokemon.HatchTime = Convert.ToInt32(pokemonArray[11]);

                tempPokemon.Height = RemoveLetter(pokemonArray[12], "f");

                tempPokemon.Weight = RemoveLetter(pokemonArray[13], "f");

                tempPokemon.EvExp = Convert.ToInt32(pokemonArray[14]);

                tempPokemon.LevelingRate = LevelFixer(TypeStripper(pokemonArray[15]));

                tempPokemon.EvHP = Convert.ToInt32(pokemonArray[16]);
                tempPokemon.EvAttack = Convert.ToInt32(pokemonArray[17]);
                tempPokemon.EvDefense = Convert.ToInt32(pokemonArray[18]);
                tempPokemon.EvSpecialAttack = Convert.ToInt32(pokemonArray[19]);
                tempPokemon.EvSpecialDefense = Convert.ToInt32(pokemonArray[20]);
                tempPokemon.EvSpeed = Convert.ToInt32(pokemonArray[21]);

                tempPokemon.PokedexColor = ToTitleCase(TypeStripper(pokemonArray[22]).ToLower());

                tempPokemon.BaseFriendship = pokemonArray[23].Replace(" ", "");

                tempPokemon.Species = QuoteRemove(pokemonArray[24]);

                bool repeat = true;
                while (repeat == true)
                {
                    for (int q = 0; q < pokemonArray[26].Replace(" ", "").Length; q++)
                    {
                        if (!Char.IsDigit(pokemonArray[26].Replace(" ", "")[q]))
                        {
                            pokemonArray[25] = pokemonArray[25] + pokemonArray[26].Replace("\"", "");
                            pokemonArray = pokemonArray.Where(w => w != pokemonArray[26]).ToArray();
                            repeat = true;
                            break;
                        }
                        else if(q == pokemonArray[26].Replace(" ", "").Length - 1)
                        {
                            repeat = false;
                            break;
                        }
                    }
                }

                tempPokemon.PokedexEntry = pokemonArray[25].Replace("\"", "");
                if(tempPokemon.PokedexEntry[0] == ' ')
                {
                    tempPokemon.PokedexEntry = tempPokemon.PokedexEntry.Remove(0, 1);
                }

                tempPokemon.BaseHP = Convert.ToInt32(pokemonArray[26]);
                tempPokemon.BaseAttack = Convert.ToInt32(pokemonArray[27]);
                tempPokemon.BaseDefense = Convert.ToInt32(pokemonArray[28]);
                tempPokemon.BaseSpecialAttack = Convert.ToInt32(pokemonArray[29]);
                tempPokemon.BaseSpecialDefense = Convert.ToInt32(pokemonArray[30]);
                tempPokemon.BaseSpeed = Convert.ToInt32(pokemonArray[31]);

                tempPokemon.Luminance = RemoveLetter(pokemonArray[32], "f");
                tempPokemon.LightColor = ToTitleCase(pokemonArray[33].Replace("Color.", "")).Replace(" ", "");

                pokemonArray[34] = pokemonArray[34].Replace("new", "").Replace(" ", "").Replace("int", "").Replace("[]", "").Replace("{", "");
                int Index = 0;
                List<int> Levels = new List<int>();
                for (int a = 34; a < pokemonArray.Length - 1; a++)
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
                foreach(int Level in Levels)
                {
                    LevelMove Move = new LevelMove
                    {
                        Level = Level,
                        Move = Moves[p]
                    };
                    tempPokemon.LevelMoves.Add(Move);
                    p++;
                }

                if(pokemonArray[Index].Replace(" ", "") == "}")
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
            if(Target == "MEDIUMSLOW")
            {
                Target = "Medium Slow";
            }
            else if(Target == "MEDIUMFAST")
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
            return Result[0];
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
