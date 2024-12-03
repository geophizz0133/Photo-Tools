using System;
using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Photo_Tools
{
    public class ImageHandler1 :IDisposable
    {
        /// <summary>
        /// Determines if the given image is monochrome (black and white).
        /// </summary>
        /// <param name="photoPath">The path to the image file.</param>
        /// <returns>True if the image is monochrome, otherwise false.</returns>
        public bool IsMonochrome(string photoPath)
        {
            bool isMonochrome = false;
            try
            {
                //If the file is a RAW file, it is always color and does not need to be checked
                int charIndex = 0;
                charIndex = charIndex + photoPath.ToUpper().IndexOf("CR2");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("CR3");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("ARW");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("MOV");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("MP4");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("RW2");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("XMP");


                //Debug.Print($"{photoPath}:{charIndex}");
                if (charIndex > 0) { return false; }

                using (var image = Image.Load<Rgba32>(photoPath))
                {
                    int monoColorPixels = 0;
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            Rgba32 pixel = image[x, y];
                            if (pixel.R != pixel.G || pixel.R != pixel.B)
                            {
                                monoColorPixels++; // Found a color pixel     
                            }
                            if (monoColorPixels > 0)
                            {
                                isMonochrome = false;
                            }

                            }
                    }
                    isMonochrome = true; // All pixels are monochrome
                }
                return isMonochrome;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return isMonochrome;
            }
        }
        public void Dispose() { }
    }
}
