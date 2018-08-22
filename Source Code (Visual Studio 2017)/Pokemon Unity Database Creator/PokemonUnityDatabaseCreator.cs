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

namespace Pokemon_Unity_Database_Creator
{
    public partial class PokemonUnityDatabaseCreator : Form
    {
        //Creating Item Lists
        public List<TextBox> pokemonTextItems = new List<TextBox>();
        public List<NumericUpDown> pokemonNumericItems = new List<NumericUpDown>();
        public List<ComboBox> pokemonComboItems = new List<ComboBox>();
        public List<RichTextBox> pokemonRichTextItems = new List<RichTextBox>();
        public List<DataGridView> pokemonMoveItems = new List<DataGridView>();

        //Creating Pokemon List
        public List<PokemonData> pokemons = new List<PokemonData>();

        //Current Sprite Image
        public Image GIF;
        public GifImage gif;
        public string GIFName = "Unknown.png";

        public int currentPokemonIndex = 0;
        public PokemonData currentPokemon = new PokemonData();

        public PokemonUnityDatabaseCreator()
        {
            InitializeComponent();
            FillLists();
            pokemons.Add(currentPokemon);
            ((DataGridViewTextBoxColumn)levelMovesGrid.Columns["Level"]).MaxInputLength = 3;
        }

        private void PokemonUnityDatabaseCreator_Load(object sender, EventArgs e)
        {

        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public void FillLists()
        {
            //Clearing Lists
            pokemonTextItems.Clear();
            pokemonNumericItems.Clear();
            pokemonComboItems.Clear();
            pokemonRichTextItems.Clear();
            pokemonMoveItems.Clear();

            //Filling Text List
            pokemonTextItems.Add(nameTextBox);
            pokemonTextItems.Add(speciesTextBox);

            //Filling Numeric List
            pokemonNumericItems.Add(pokedexID);
            pokemonNumericItems.Add(maleRatio);
            pokemonNumericItems.Add(catchRate);
            pokemonNumericItems.Add(hatchTime);
            pokemonNumericItems.Add(height);
            pokemonNumericItems.Add(weight);
            pokemonNumericItems.Add(luminance);
            //Base Stats
            pokemonNumericItems.Add(statsBaseHP);
            pokemonNumericItems.Add(statsBaseAttack);
            pokemonNumericItems.Add(statsBaseDefense);
            pokemonNumericItems.Add(statsBaseSpecialAttack);
            pokemonNumericItems.Add(statsBaseSpecialDefense);
            pokemonNumericItems.Add(statsBaseSpeed);
            //EV Stats
            pokemonNumericItems.Add(statsYieldHP);
            pokemonNumericItems.Add(statsYieldAttack);
            pokemonNumericItems.Add(statsYieldDefense);
            pokemonNumericItems.Add(statsYieldSpecialAttack);
            pokemonNumericItems.Add(statsYieldSpecialDefense);
            pokemonNumericItems.Add(statsYieldSpeed);
            pokemonNumericItems.Add(statsYieldExp);

            //Filling Combo Items
            pokemonComboItems.Add(pokemonType1);
            pokemonComboItems.Add(pokemonType2);
            pokemonComboItems.Add(pokemonHiddenAbility);
            pokemonComboItems.Add(pokemonEggGroup1);
            pokemonComboItems.Add(pokemonEggGroup2);
            pokemonComboItems.Add(levelingRate);
            pokemonComboItems.Add(pokedexColor);
            pokemonComboItems.Add(lightColor);
            pokemonComboItems.Add(spriteTypeBox);

            //Filling Rich Text Items
            pokemonRichTextItems.Add(pokedexEntry);

            //Filling Data Grid Items
            pokemonMoveItems.Add(levelMovesGrid);
            pokemonMoveItems.Add(tmAndHMGrid);
        }


        public PokemonData FillPokemonData(int Index)
        {
            PokemonData Pokemon = pokemons[Index];

            //Pokemon Info
            if (GIFName != "Unknown.png")
            {
                Pokemon.Sprite = pokemonIcon.Tag.ToString();
            }
            Pokemon.Name = nameTextBox.Text;
            Pokemon.PokedexID = pokedexID.Value.ToString();
            Pokemon.Type1 = pokemonType1.Text.ToString();
            Pokemon.Type2 = pokemonType2.Text.ToString();
            Pokemon.Ability1 = pokemonAbility1.Text.ToString();
            Pokemon.Ability2 = pokemonAbility2.Text.ToString();
            Pokemon.HiddenAbility = pokemonHiddenAbility.Text.ToString();

            //Egg Group
            Pokemon.EggGroup1 = pokemonEggGroup1.Text.ToString();
            Pokemon.EggGroup2 = pokemonEggGroup2.Text.ToString();

            //Ratio's And Value's
            Pokemon.MaleRatio = (float)maleRatio.Value;
            Pokemon.CatchRate = (float)catchRate.Value;
            Pokemon.HatchTime = Convert.ToInt32(hatchTime.Value);
            Pokemon.LevelingRate = levelingRate.Text.ToString();

            //Pokedex Info
            Pokemon.Height = (float)height.Value;
            Pokemon.Weight = (float)weight.Value;
            Pokemon.PokedexColor = pokedexColor.Text.ToString();
            Pokemon.Species = speciesTextBox.Text;
            Pokemon.BaseFriendship = baseFriendship.Value.ToString();
            Pokemon.PokedexEntry = pokedexEntry.Text;
            Pokemon.EvolutionID = evolutionID.Value.ToString();
            Pokemon.EvolutionLevel = evolutionLevel.Value.ToString();

            //Pokemon Unity Extra's
            Pokemon.Luminance = (float)luminance.Value;
            if (lightColor.Text == "Transparent")
            {
                Pokemon.LightColor = "Clear";
            }
            else
            {
                Pokemon.LightColor = lightColor.Text.ToString();
            }

            //Base Stats
            Pokemon.BaseHP = (int)statsBaseHP.Value;
            Pokemon.BaseAttack = (int)statsBaseAttack.Value;
            Pokemon.BaseDefense = (int)statsBaseDefense.Value;
            Pokemon.BaseSpecialAttack = (int)statsBaseSpecialAttack.Value;
            Pokemon.BaseSpecialDefense = (int)statsBaseSpecialDefense.Value;
            Pokemon.BaseSpeed = (int)statsBaseSpeed.Value;

            //EV Stats
            Pokemon.EvHP = (int)statsYieldHP.Value;
            Pokemon.EvAttack = (int)statsYieldAttack.Value;
            Pokemon.EvDefense = (int)statsYieldDefense.Value;
            Pokemon.EvSpecialAttack = (int)statsYieldSpecialAttack.Value;
            Pokemon.EvSpecialDefense = (int)statsYieldSpecialDefense.Value;
            Pokemon.EvSpeed = (int)statsYieldSpeed.Value;
            Pokemon.EvExp = (int)statsYieldExp.Value;

            //Level Moves
            Pokemon.LevelMoves.Clear();
            int i = 0;
            foreach (DataGridViewRow Row in levelMovesGrid.Rows)
            {
                if (levelMovesGrid.Rows[i].Cells["Move"].Value != null && levelMovesGrid.Rows[i].Cells["Level"].Value != null)
                {
                    if (levelMovesGrid.Rows[i].Cells["Move"].Value == null && levelMovesGrid.Rows[i].Cells["Level"].Value != null)
                    {
                        LevelMove tempLevelMove = new LevelMove();
                        tempLevelMove.Move = "???";
                        tempLevelMove.Level = (int)levelMovesGrid.Rows[i].Cells["Level"].Value;
                        Pokemon.LevelMoves.Add(tempLevelMove);
                    }
                    else if (levelMovesGrid.Rows[i].Cells["Move"].Value != null && levelMovesGrid.Rows[i].Cells["Level"].Value == null)
                    {
                        LevelMove tempLevelMove = new LevelMove();
                        tempLevelMove.Move = levelMovesGrid.Rows[i].Cells["Move"].Value.ToString();
                        tempLevelMove.Level = 999;
                        Pokemon.LevelMoves.Add(tempLevelMove);
                    }
                    else
                    {
                        LevelMove tempLevelMove = new LevelMove();
                        tempLevelMove.Move = levelMovesGrid.Rows[i].Cells["Move"].Value.ToString();
                        tempLevelMove.Level = Convert.ToInt32(levelMovesGrid.Rows[i].Cells["Level"].Value);
                        Pokemon.LevelMoves.Add(tempLevelMove);
                    }
                    i++;
                }
            }

            Pokemon.hmAndTM.Clear();
            int j = 0;
            foreach(DataGridViewRow Row in tmAndHMGrid.Rows)
            {
                if(tmAndHMGrid.Rows[j].Cells[0].Value != null)
                {
                    Pokemon.hmAndTM.Add(tmAndHMGrid.Rows[j].Cells[0].Value.ToString());
                }
                j++;
            }
            return Pokemon;
        }

        private void addPokemon_Click(object sender, EventArgs e)
        {
            PokemonData TempPokemon = new PokemonData();
            TempPokemon = FillPokemonData(currentPokemonIndex);

            bool Duplicate = false;
            int PokeIndex = 0;
            string PokeName = "";

            foreach (PokemonData pokemon in pokemons)
            {
                if (pokemon.Name == TempPokemon.Name || pokemon.PokedexID == TempPokemon.PokedexID)
                {
                    Duplicate = true;
                    PokeIndex = Convert.ToInt32(pokemon.PokedexID);
                    PokeName = pokemon.Name;
                }
            }

            if (Duplicate == false)
            {
                pokemons.Add(FillPokemonData(currentPokemonIndex));
                List<PokemonData> OrderList = pokemons.OrderBy(o => o.PokedexID).ToList();
                pokemons.Clear();
                pokemons = OrderList;

                mainBindingSource.Clear();
                mainBindingSource.DataSource = pokemons;
                databaseNavigationName.DataSource = mainBindingSource.DataSource;
                databaseNavigationID.DataSource = mainBindingSource.DataSource;
                databaseNavigationName.DisplayMember = "Name";
                databaseNavigationName.ValueMember = "Name";
                databaseNavigationID.DisplayMember = "PokedexID";
                databaseNavigationID.ValueMember = "PokedexID";
            }
            else
            {
                MessageBox.Show($"The entry you filled in is a duplicate. {PokeName} is already present with the ID {PokeIndex}");
            }
        }

        private void lightColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lightColor.Text != "")
            {
                if (lightColor.Text.ToString() != "Clear")
                {
                    lightColorExample.BackColor = Color.FromName(lightColor.Text.ToString());
                }
                else
                {
                    lightColorExample.BackColor = SystemColors.Control;
                }
            }
        }

        private void pokedexColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pokedexColor.Text != "")
            {
                pokedexColorExample.BackColor = Color.FromName(pokedexColor.Text.ToString());
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "PokemonDatabase.xml";
            save.Filter = "XML Files|*.xml";
            if (save.ShowDialog() == DialogResult.OK)
            {
                FillPokemonData(currentPokemonIndex);
                XmlSerializer serialiser = new XmlSerializer(typeof(List<PokemonData>));
                TextWriter Filestream = new StreamWriter(save.FileName);
                serialiser.Serialize(Filestream, pokemons);
                Filestream.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "PokemonDatabase.xml";
            openFileDialog.Filter = "XML Files|*.xml";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                List<PokemonData> myObjects = new List<PokemonData>();
                XmlSerializer serializer = new XmlSerializer(typeof(List<PokemonData>));
                XmlReader reader = XmlReader.Create(openFileDialog.FileName);
                myObjects = (List<PokemonData>)serializer.Deserialize(reader);
                pokemons.Clear();
                pokemons = myObjects;
                BindingSource();
                RefillPokemonData(pokemons[0]);
            }
            openFileDialog.Filter = "";
        }

        private void databaseNavigationID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            foreach (PokemonData Pokemon in pokemons)
            {
                if (databaseNavigationID.SelectedValue != null)
                {
                    if (databaseNavigationID.SelectedValue.ToString() == Pokemon.PokedexID.ToString())
                    {
                        currentPokemonIndex = i;
                        RefillPokemonData(Pokemon);
                        break;
                    }
                }
                i++;
            }
        }

        public void BindingSource()
        {
            mainBindingSource.DataSource = null;
            mainBindingSource.Clear();
            mainBindingSource.DataSource = pokemons;
            databaseNavigationID.DataSource = null;
            databaseNavigationName.DataSource = null;
            databaseNavigationName.DataSource = mainBindingSource.DataSource;
            databaseNavigationID.DataSource = mainBindingSource.DataSource;
            databaseNavigationName.DisplayMember = "Name";
            databaseNavigationName.ValueMember = "Name";
            databaseNavigationID.DisplayMember = "PokedexID";
            databaseNavigationID.ValueMember = "PokedexID";
        }

        public void RefillPokemonData(PokemonData Pokemon)
        {
            //Pokemon Info
            if (Pokemon.Sprite != null)
            {
                getPokemonGIFIcon(Pokemon.Sprite);
            }
            nameTextBox.Text = Pokemon.Name;
            pokedexID.Text = Pokemon.PokedexID.ToString();
            pokemonType1.Text = Pokemon.Type1;
            pokemonType2.Text = Pokemon.Type2;
            pokemonAbility1.Text = Pokemon.Ability1;
            pokemonAbility2.Text = Pokemon.Ability2;
            pokemonHiddenAbility.Text = Pokemon.HiddenAbility;

            //Egg Groups
            pokemonEggGroup1.Text = Pokemon.EggGroup1;
            pokemonEggGroup2.Text = Pokemon.EggGroup2;

            //Ratio's and Value's
            maleRatio.Value = (decimal)Pokemon.MaleRatio;
            catchRate.Value = (decimal)Pokemon.CatchRate;
            hatchTime.Value = (decimal)Pokemon.HatchTime;
            levelingRate.Text = Pokemon.LevelingRate;

            //Pokedex Info
            height.Value = (decimal)Pokemon.Height;
            weight.Value = (decimal)Pokemon.Weight;
            pokedexColor.Text = Pokemon.PokedexColor;
            speciesTextBox.Text = Pokemon.Species;
            baseFriendship.Value = Convert.ToInt32(Pokemon.BaseFriendship);
            pokedexEntry.Text = Pokemon.PokedexEntry;
            evolutionID.Value = Convert.ToInt32(Pokemon.EvolutionID);
            evolutionLevel.Value = Convert.ToInt32(Pokemon.EvolutionLevel);

            //Pokemon Unity Extra's
            luminance.Value = (decimal)Pokemon.Luminance;
            lightColor.Text = Pokemon.LightColor;

            //Base Stats
            statsBaseHP.Value = Pokemon.BaseHP;
            statsBaseAttack.Value = Pokemon.BaseAttack;
            statsBaseDefense.Value = Pokemon.BaseDefense;
            statsBaseSpecialAttack.Value = Pokemon.BaseSpecialAttack;
            statsBaseSpecialDefense.Value = Pokemon.BaseSpecialDefense;
            statsBaseSpeed.Value = Pokemon.BaseSpeed;

            //Ev Stats
            statsYieldHP.Value = Pokemon.EvHP;
            statsYieldAttack.Value = Pokemon.EvAttack;
            statsYieldDefense.Value = Pokemon.EvDefense;
            statsYieldSpecialAttack.Value = Pokemon.EvSpecialAttack;
            statsYieldSpecialDefense.Value = Pokemon.EvSpecialDefense;
            statsYieldSpeed.Value = Pokemon.EvSpeed;
            statsYieldExp.Value = Pokemon.EvExp;

            if (pokedexColor.Text != "" && lightColor.Text != "")
            {
                pokedexColorExample.BackColor = Color.FromName(pokedexColor.Text);
                if (lightColor.Text.ToString() != "Clear")
                {
                    lightColorExample.BackColor = Color.FromName(lightColor.Text);
                }
                else
                {
                    lightColorExample.BackColor = SystemColors.Control;
                }
            }

            List<LevelMove> OrderList = Pokemon.LevelMoves.OrderBy(o => o.Level).ToList();
            Pokemon.LevelMoves.Clear();
            Pokemon.LevelMoves = OrderList;

            levelMovesGrid.Rows.Clear();

            int i = 0;
            foreach (LevelMove Move in Pokemon.LevelMoves)
            {
                levelMovesGrid.Rows.Add();
                levelMovesGrid.Rows[i].Cells["Move"].Value = Move.Move;
                levelMovesGrid.Rows[i].Cells["Level"].Value = Move.Level;
                i++;
            }

            tmAndHMGrid.Rows.Clear();

            int j = 0;
            foreach(string Move in Pokemon.hmAndTM)
            {
                tmAndHMGrid.Rows.Add();
                tmAndHMGrid.Rows[j].Cells[0].Value = Move;
                j++;
            }
        }

        private void spriteButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                getPokemonGIFIcon(openFileDialog.FileName);
            }
        }

        public TimeSpan GetGifDuration(Image image, int fps = 60)
        {
            var minimumFrameDelay = (1000.0 / fps);

            var frameDimension = new FrameDimension(image.FrameDimensionsList[0]);

            var frameCount = image.GetFrameCount(frameDimension);
            var totalDuration = 0;

            for (var f = 0; f < frameCount; f++)
            {
                var delayPropertyBytes = image.GetPropertyItem(20736).Value;
                var frameDelay = BitConverter.ToInt32(delayPropertyBytes, f * 4) * 10;
                totalDuration += (frameDelay < minimumFrameDelay ? (int)minimumFrameDelay : frameDelay);
            }

            return TimeSpan.FromMilliseconds(totalDuration);
        }

        public void getPokemonGIFIcon(string gifPath)
        {
            pokemonIcon.Tag = gifPath;
            GIFName = Path.GetFileName(gifPath);
            GIF = Image.FromFile(gifPath);
            gif = new GifImage(gifPath);
            gif.ReverseAtEnd = false;
            pokemonIcon.Image = gif.GetNextFrame();
        }

        private void levelMovesGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (levelMovesGrid.CurrentCell.ColumnIndex == 1)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void saveCurrentPokemon_Click(object sender, EventArgs e)
        {
            FillPokemonData(currentPokemonIndex);
            BindingSource();
        }

        private void newPokemon_Click(object sender, EventArgs e)
        {
            FillPokemonData(currentPokemonIndex);
            PokemonData TempPokemon = new PokemonData();
            TempPokemon.PokedexID = "???";
            TempPokemon.Name = "???";
            pokemons.Add(TempPokemon);
            currentPokemonIndex = pokemons.Count() - 1;
            BindingSource();
            databaseNavigationID.SelectedValue = "???";
            ResetPokemon();
        }

        public void ResetPokemon()
        {
            foreach(TextBox text in pokemonTextItems)
            {
                text.Text = "";
            }
            foreach(NumericUpDown numeric in pokemonNumericItems)
            {
                numeric.Value = 0;
            }
            foreach(ComboBox combo in pokemonComboItems)
            {
                combo.Text = "";
            }
            foreach(RichTextBox richText in pokemonRichTextItems)
            {
                richText.Text = "";
            }
            foreach(DataGridView dataGrid in pokemonMoveItems)
            {
                dataGrid.Rows.Clear();
            }
            gif = null;
            pokemonIcon.Image = Pokemon_Unity_Database_Creator.Properties.Resources.Unknown;
        }

        private void tmAndHMGrid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.C:
                            CopyToClipboard();
                            break;

                        case Keys.V:
                            PasteClipboard();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Copy/paste operation failed. " + ex.Message, "Copy/Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = tmAndHMGrid.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void PasteClipboard()
        {
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');

                int iRow = tmAndHMGrid.CurrentCell.RowIndex;
                int iCol = tmAndHMGrid.CurrentCell.ColumnIndex;
                DataGridViewCell oCell;
                if (iRow + lines.Length > tmAndHMGrid.Rows.Count - 1)
                {
                    bool bFlag = false;
                    foreach (string sEmpty in lines)
                    {
                        if (sEmpty == "")
                        {
                            bFlag = true;
                        }
                    }

                    int iNewRows = iRow + lines.Length - tmAndHMGrid.Rows.Count;
                    if (iNewRows > 0)
                    {
                        if (bFlag)
                            tmAndHMGrid.Rows.Add(iNewRows);
                        else
                            tmAndHMGrid.Rows.Add(iNewRows + 1);
                    }
                    else
                        tmAndHMGrid.Rows.Add(iNewRows + 1);
                }
                foreach (string line in lines)
                {
                    if (iRow < tmAndHMGrid.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        for (int i = 0; i < sCells.GetLength(0); ++i)
                        {
                            if (iCol + i < this.tmAndHMGrid.ColumnCount)
                            {
                                oCell = tmAndHMGrid[iCol + i, iRow];
                                oCell.Value = Convert.ChangeType(sCells[i].Replace("\r", ""), oCell.ValueType);
                            }
                            else
                            {
                                break;
                            }
                        }
                        iRow++;
                    }
                    else
                    {
                        break;
                    }
                }
                Clipboard.Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("The data you pasted is in the wrong format for the cell");
                return;
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DatabaseExporter(pokemons);
        }
    }
}
