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
        /// This assumes that if R<>G and G<>B it is color
        /// <param name="photoPath">The path to the image file.</param>
        /// <returns>True if the image is monochrome, otherwise false.</returns>
        public bool IsMonochrome(string photoPath)
        {
            bool isMonochrome = false;
            try
            {
                //If the file is a RAW or Video file, it is always color and does not need to be checked
                int charIndex = 0;
                charIndex = charIndex + photoPath.ToUpper().IndexOf("CR2");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("CR3");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("ARW");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("MOV");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("MP4");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("RW2");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("XMP");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("DB");

                if (charIndex > 0) { return false; }

                using (var image = Image.Load<Rgba32>(photoPath))
                {
                    int monoColorPixels = 0;
                    for (int y = 100; y < 500; y++)
                    {
                        for (int x = 100; x < 500; x++)
                        {
                            Rgba32 pixel = image[x, y];
                            if (pixel.R != pixel.G && pixel.G != pixel.B)
                            {
                                monoColorPixels++; // Found a color pixel

                                if (monoColorPixels == 100)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    if (monoColorPixels > 100) //If there are more than 100 color pixels it is not monochrome
                    {
                        isMonochrome = false;
                    }
                    else
                    {
                        isMonochrome = true; // All pixels are monochrome
                    }
                }
                return isMonochrome;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IsMonochrome - An error occurred: {ex.Message}");
                return isMonochrome;
            }
        }
        public bool IsRAW(string photoPath) 
            //Video files are counted as RAW to exlude them from processing because they have funky metadata that screws things up
        {
            
            int charIndex = 0;
            charIndex = charIndex + photoPath.ToUpper().IndexOf("CR2");
            charIndex = charIndex + photoPath.ToUpper().IndexOf("CR3");
            charIndex = charIndex + photoPath.ToUpper().IndexOf("ARW"); //Some DSLR captured film photos retain ARW in their names even though they are saved as tiff so they get marked as originals - I'm ok with that
            charIndex = charIndex + photoPath.ToUpper().IndexOf("DNG");
            charIndex = charIndex + photoPath.ToUpper().IndexOf("HEIF");
            charIndex = charIndex + photoPath.ToUpper().IndexOf("MOV");
            charIndex = charIndex + photoPath.ToUpper().IndexOf("MP4");
            charIndex = charIndex + photoPath.ToUpper().IndexOf("RW2");
            charIndex = charIndex + photoPath.ToUpper().IndexOf("XMP");

            if (charIndex > 0) { return true; }
            return false;
        }
        public void Dispose() { }
    }
}
