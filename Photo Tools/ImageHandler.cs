using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photo_Tools
{
    internal class ImageHandler
    {
        public ImageHandler() { }
        public ImageHandler(string path) { }

        public bool IsMonochrome(string path) { return false; }
        public decimal GetRGBHash(string path) { return 0; }

        public void SaveImageAs(string path, string newformat) { }


    }
}
