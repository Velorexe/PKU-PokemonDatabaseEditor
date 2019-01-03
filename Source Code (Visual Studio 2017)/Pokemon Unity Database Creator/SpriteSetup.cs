using System;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Drawing;

namespace Pokemon_Unity_Database_Creator
{
    public partial class SpriteSetup : Form
    {
        private string resourcesFolder;

        //Current Sprite Image
        public Image GIF;
        public GifImage gif;
        public string GIFName = "Unknown.png";

        public SpriteSetup(PokemonData pokemon)
        {
            InitializeComponent();

            PokemonInfoPokemonID.Text = pokemon.PokedexID;
            PokemonInfoNameText.Text = pokemon.Name;

            FrontMaleSpriteType.DataSource = Enum.GetNames(typeof(SpriteTypes));
            FrontFemaleSpriteType.DataSource = Enum.GetNames(typeof(SpriteTypes));
            BackMaleSpriteType.DataSource = Enum.GetNames(typeof(SpriteTypes));
            BackFemaleSpriteType.DataSource = Enum.GetNames(typeof(SpriteTypes));
        }

        private void FemaleSpriteCheck_CheckedChanged(object sender, EventArgs e)
        {
            frontFemaleGroup.Enabled = FemaleSpriteCheck.Checked;
            backFemaleGroup.Enabled = FemaleSpriteCheck.Checked;
        }

        private void ResourcesFolderButton_Click(object sender, EventArgs e)
        {
            //Since the normal OpenFolderDialog lacks usability
            //I used the CommonOpenFileDialog for more user options
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\",
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (Directory.Exists(dialog.FileName + @"\PokemonBackSprites") && Directory.Exists(dialog.FileName + @"\PokemonSprites"))
                {
                    resourcesFolder = dialog.FileName;
                    ResourcesFolderText.Text = resourcesFolder;
                }
                else
                {
                    MessageBox.Show("The selected Resources Folder doesn't contain the folder \"PokemonBackSprites\" and \" PokemonSprites\".");
                }
            }
        }

        private enum SpriteTypes
        {
            GIF,
            Frames,
            Spritesheet
        }

        private void FrontMaleLocateSprite_Click(object sender, EventArgs e)
        {
            LocateSprite(FrontMaleSpriteType);
        }

        private void FrontFemaleLocateSprite_Click(object sender, EventArgs e)
        {
            LocateSprite(FrontFemaleSpriteType);
        }

        private void BackMaleLocateSprite_Click(object sender, EventArgs e)
        {
            LocateSprite(BackMaleSpriteType);
        }

        private void BackFemaleLocateSprite_Click(object sender, EventArgs e)
        {
            LocateSprite(BackFemaleSpriteType);
        }

        private void LocateSprite(ComboBox spriteType)
        {
            if (spriteType.SelectedItem.ToString() == SpriteTypes.GIF.ToString())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FileName = "Sprite",
                    Filter = "GIF Files|*.gif"
                };

                DialogResult result = openFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    GetPokemonGIFIcon(openFileDialog.FileName);
                }
            }
            else if(spriteType.SelectedItem.ToString() == SpriteTypes.Spritesheet.ToString())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FileName = "Sprite",
                    Filter = "GIF Files (*.gif)|*.gif|PNG Files (*.png)|*.png|JPG Files (*.jpg*)|*.jpg|JPEG Files (*.jpeg)|*.jpeg"
                };

                DialogResult result = openFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    
                }
            }
            else if(spriteType.SelectedItem.ToString() == SpriteTypes.Frames.ToString())
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog
                {
                    InitialDirectory = @"C:\",
                    IsFolderPicker = true
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    
                }
            }
        }

        private void GetPokemonGIFIcon(string gifPath)
        {
            pokemonIcon.Tag = gifPath;
            GIFName = Path.GetFileName(gifPath);
            GIF = Image.FromFile(gifPath);
            gif = new GifImage(gifPath)
            {
                ReverseAtEnd = false
            };
            pokemonIcon.Image = gif.GetNextFrame();
        }
    }
}
