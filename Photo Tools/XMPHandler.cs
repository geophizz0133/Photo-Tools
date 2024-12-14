using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmpCore.Impl;
using XmpCore.Options;
using XmpCore;

namespace Photo_Tools
{
    internal class XMPHandler
    {
        using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using XmpCore;

namespace ImageProcessing
    {
        public class XmpHandler
        {
            /// <summary>
            /// Writes XMP data to a sidecar file.
            /// </summary>
            /// <param name="filePath">The path to the XMP sidecar file.</param>
            /// <param name="property">The XMP property to write.</param>
            /// <param name="value">The value of the XMP property.</param>
            public void WriteXmpData(string filePath, string property, string value)
            {
                XmpMeta xmpMeta = new XmpMeta();
                xmpMeta.SetProperty(XmpConstants.NsDc, property, value);

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(xmpMeta.SerializeToString(new SerializeOptions()));
                }
            }

            /// <summary>
            /// Reads XMP data from a sidecar file.
            /// </summary>
            /// <param name="filePath">The path to the XMP sidecar file.</param>
            /// <param name="property">The XMP property to read.</param>
            /// <returns>The value of the XMP property.</returns>
            public string ReadXmpData(string filePath, string property)
            {
                try
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var parser = XmpMetaFactory.Parse(stream);
                        return parser.GetPropertyString(XmpConstants.NsDc, property);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading XMP data: {ex.Message}");
                    return null;
                }
            }

            /// <summary>
            /// Updates XMP data in a sidecar file.
            /// </summary>
            /// <param name="filePath">The path to the XMP sidecar file.</param>
            /// <param name="property">The XMP property to update.</param>
            /// <param name="newValue">The new value of the XMP property.</param>
            public void UpdateXmpData(string filePath, string property, string newValue)
            {
                try
                {
                    XmpMeta xmpMeta;
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        xmpMeta = (XmpMeta)XmpMetaFactory.Parse(stream);
                    }

                    xmpMeta.SetProperty(XmpConstants.NsDc, property, newValue);

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(xmpMeta.SerializeToString(new SerializeOptions()));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating XMP data: {ex.Message}");
                }
            }
        }
    }

}
}
