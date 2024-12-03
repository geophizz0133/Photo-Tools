using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Data.Sqlite;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Photo_Tools
{
    public class MetadataHandler : IDisposable
    {
        public PhotoData photoData = new PhotoData();
        public MetadataHandler()
        {
            
        }

        public void Dispose() { }

        public PhotoData ReadPhoto(string PhotoFileLocation)
        {
            PhotoData photoData = new PhotoData();

            Debug.Print(PhotoFileLocation);

            FileInfo fileInfo = new FileInfo(PhotoFileLocation);
            photoData.ID = new UniqueId().ToString();
            photoData.FilePath = PhotoFileLocation;
            photoData.FileName = fileInfo.Name;
            photoData.Extension = fileInfo.Extension;
            photoData.FileSize = fileInfo.Length.ToString();
            photoData.isMonochrome = DetermineMonochrome(PhotoFileLocation);
            photoData.RGBHash = String.Empty;


            var dataTree = ImageMetadataReader.ReadMetadata(PhotoFileLocation);
            


                //Cycle thru all dataTree
                foreach (var dataBranch in dataTree)
                {
                    foreach (var MetadataTag in dataBranch.Tags)
                    {
                        switch (MetadataTag.Description)
                        {
                            case "Reduced-Resolution Image":
                                photoData.ReducedResolution = true;
                                break;
                        }
                        switch (MetadataTag.Name)
                        {
                            case "Make":
                                photoData.CameraMake = MetadataTag.Description;
                                break;
                            case "Model":
                                photoData.CameraModel = MetadataTag.Description;
                                break;
                            case "F-Number":
                                photoData.fStop = MetadataTag.Description;
                                break;
                            case "Focal Length":
                                photoData.FocalLength = MetadataTag.Description;
                                break;
                            case "Date/Time":
                            case "Date/Time Digitized":
                            case "Date/Time Original":  //this needs more handling
                                photoData.DateCaptured = MetadataTag.Description;
                                break;
                            case "Exposure Time":
                                photoData.ShutterSpeed = MetadataTag.Description;
                                break;
                            case "Software":
                                photoData.Software = MetadataTag.Description;
                                break;
                            case "Image Height":
                            case "Pixel Y Dimension":
                                photoData.ImageHeight = MetadataTag.Description;
                                break;
                            case "Image Width":
                            case "Pixel X Dimension":
                                photoData.ImageWidth = MetadataTag.Description;
                                break;
                            case "Full Image Size":
                                photoData.FullImageSize = MetadataTag.Description;
                                break;
                            case "File Modified Date":
                                photoData.DateLastModified = MetadataTag.Description;
                                break;
                            default:
//                                if (photoData.Extension.ToLower() == ".png")
//                                {
//                                    Console.WriteLine($"RandoData:{photoData.FilePath} - {MetadataTag.Name}:{MetadataTag.Description}");
//                                }
                                break;

                        }

                    }
                }
            dataTree = null;
            this.Dispose();
            GC.Collect();
            GC.WaitForFullGCComplete(); 
            return photoData;
        }
        public bool DetermineMonochrome(string filePath)
        {
           // ImageHandler photoHandler = new ImageHandler(); //ImageMagick
            ImageHandler1 photoHandler = new ImageHandler1(); //ImageSharp
            return photoHandler.IsMonochrome(filePath);
            photoHandler.Dispose();
            GC.Collect();
        }
    }
}
