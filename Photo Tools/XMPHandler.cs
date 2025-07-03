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
    public class XMPHandler
    
    {

            /// <summary>
            /// Writes XMP data to a sidecar file.
            /// </summary>
            /// <param name="filePath">The path to the XMP sidecar file.</param>
            /// <param name="property">The XMP property to write.</param>
            /// <param name="value">The value of the XMP property.</param>
            public void WriteXmpData(string filePath, string property, string value)
            {
                var xmpMeta = new XmpMeta();
                xmpMeta.SetProperty(XmpConstants.NsDC, property, value);

                using (var writer = new StreamWriter(filePath))
                {
                    XmpMetaFactory.Serialize(xmpMeta, writer.BaseStream, new SerializeOptions());
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
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var xmpMeta = XmpMetaFactory.Parse(stream);
                        return xmpMeta.GetPropertyString(XmpConstants.NsDC, property);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading XMP data: {ex.Message}");
                    throw;
                }
            }

            public void UpdateXmpData(string filePath, string property, string newValue)
            {
                UpdateXmpData(filePath, property, newValue);
            }

            /// <summary>
            /// Updates XMP data in a sidecar file.
            /// </summary>
            /// <param name="filePath">The path to the XMP sidecar file.</param>
            /// <param name="property">The XMP property to update.</param>
            /// <param name="newValue">The new value of the XMP property.</param>
            public void UpdateXmpData(string filePath, string property, string newValue, StreamWriter writer)
            {
                try
                {
                    XmpMeta xmpMeta;
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        xmpMeta = (XmpMeta)XmpMetaFactory.Parse(stream);
                    }

                    xmpMeta.SetProperty(XmpConstants.NsDC, property, newValue);

                    using (var rdfWriter = new StreamWriter(filePath))
                    {
                        //Stream rdfStream = rdfWriter.Write(rdfWriter.ToString());   
                        XmpMetaFactory.Serialize(xmpMeta, rdfWriter.BaseStream, new SerializeOptions());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating XMP data: {ex.Message}");
                }
            }
        }
    }


