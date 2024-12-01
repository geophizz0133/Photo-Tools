using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Linq.Expressions;
using ImageMagick;
using ImageMagick.Colors;



namespace Photo_Tools
{
    internal class ImageHandler
    {
        public ImageHandler() { }
        public ImageHandler(string path) { }




        public bool IsMonochrome(string photoPath)
        {
            {
                int charIndex = 0;
                charIndex = charIndex + photoPath.ToUpper().IndexOf("CR2");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("CR3");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("ARW");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("MOV");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("MP4");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("RW2");
                charIndex = charIndex + photoPath.ToUpper().IndexOf("XMP");


                Debug.Print($"{photoPath}:{charIndex}");
                if (charIndex > 0) { return false; }

                string imagePath = photoPath;
                Console.WriteLine($"Checking Image {imagePath}");
                using (var image = new MagickImage(imagePath))
                // using var image = new MagickImage(@"Q:\\Photo Library\\2014\\2014-12\\DSC00224_2.jpg");
                {
                    bool isMonochrome = true; // Inspect each pixel to see if all channels are equal
                    for (int y = 0; y < image.Height && isMonochrome; y++)
                    {
                        for (int x = 0; x < image.Width && isMonochrome; x++)
                        {
                            var pixel = image.GetPixels().GetPixel(x, y);
                            var channels = pixel.ToColor();
                            if (channels.R != channels.G || channels.R != channels.B)
                            {
                                isMonochrome = false;
                            }
                        }
                    }

                    if (isMonochrome)
                    {
                        Console.WriteLine("The image is effectively monochrome (black and white).");
                    }
                    else
                    {
                        //Console.WriteLine("The image is color."); 
                    }

                    GC.Collect();
                    return isMonochrome;
                }
            }
        }
        

            

        public decimal RGBHash(string path) { return 0; }

        public void SaveImageAs(string path, string newformat) { }

        static string ConvertToUnixPath(string windowsPath) { return windowsPath.Replace('\\', '/'); }
        static string ConvertToEscapedPath(string path) { return path.Replace("\\", "\\\\"); }


    }
}
