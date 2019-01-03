using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Pokemon_Unity_Database_Creator
{
    class PokemonSprite
    {
        public SpriteTypes SpriteType { get; set; }

        public int CurrentFrameIndex = 0;
        public Image[] Frames { get; set; } = new Image[] { };

        public void SetSpriteSheet(Bitmap spriteSheet, int width, int height)
        {
            if (width == 0 || height == 0)
            {
                MessageBox.Show("The width or height hasn't been set for this Spritesheet.");
            }
            else
            {
                Frames = new Image[spriteSheet.Width / width * spriteSheet.Height / height];

                int index = 0;
                int opaqueFrames = 0;
                for (int i = 0; i < spriteSheet.Height / width; i++)
                {
                    for (int j = 0; j < spriteSheet.Width / height; j++)
                    {
                        Frames[index] = new Bitmap(width, height);
                        Frames[index] = spriteSheet.Clone(new Rectangle(j * width, i * height, width, height), spriteSheet.PixelFormat);

                        if (IsOpaque(Frames[index])) opaqueFrames++;
                        index++;
                    }
                }

                Frames = Frames.Take(Frames.Length - opaqueFrames).ToArray();
            }
        }

        private bool IsOpaque(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            Bitmap bitmap = image as Bitmap;

            if (bitmap == null)
            {
                bitmap = new Bitmap(image);
            }

            var width = bitmap.Width;
            var height = bitmap.Height;

            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    var pixels = (uint*)bitmapData.Scan0;
                    var stride = bitmapData.Stride / sizeof(uint);

                    for (var y = 0; y < height; y++)
                    {
                        var yOffset = y * stride;

                        for (var x = 0; x < width; x++)
                        {
                            if (pixels[x + yOffset] < 0xFF000000)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return true;
        }
    }
}
