using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Data.Sqlite;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Photo_Tools
{
    public class MetadataHandler
    {
        public PhotoData photoData = new PhotoData();
        public MetadataHandler()
        {
            
        }


        public PhotoData ReadPhoto(string PhotoFileLocation)
        {
            PhotoData photoData = new PhotoData();
           
            FileInfo fileInfo = new FileInfo(PhotoFileLocation);
            photoData.ID = new UniqueId().ToString();
            photoData.FilePath = PhotoFileLocation;
            photoData.FileName = fileInfo.Name;
            photoData.Extension = fileInfo.Extension;
            photoData.FileSize = fileInfo.Length.ToString();

           
                var directories = ImageMetadataReader.ReadMetadata(PhotoFileLocation);

                //Cycle thru all directories
                foreach (var directory in directories)
                {
                    foreach (var MetadataTag in directory.Tags)
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
                                if (photoData.Extension.ToLower() == ".png")
                                {
                                    Console.WriteLine($"RandoData:{photoData.FilePath} - {MetadataTag.Name}:{MetadataTag.Description}");
                                }
                                break;

                        }

                    }
                }
           return photoData;
        }
    }
}
