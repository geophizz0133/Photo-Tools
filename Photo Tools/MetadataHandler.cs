using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Data.Sqlite;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.IO;

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
            photoData.FilePath = PhotoFileLocation;
            photoData.FileName = fileInfo.Name;
            photoData.Extension = fileInfo.Extension;

            var directories = ImageMetadataReader.ReadMetadata(PhotoFileLocation);

            //Cycle thru all directories
            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {

                    switch(tag.Name) 
                    {
                        case "Make":
                            photoData.CameraMake = tag.Description;
                            break;
                        case "F-Number":
                            photoData.fStop = tag.Description;
                            break;
                        case "Model":
                            photoData.CameraModel = tag.Description;
                            break;
                        case "Focal Length":
                            photoData.FoclLength = tag.Description;
                            break;
                        case "Date/Time Digitized":
                        case "Date/Time Original":  //this needs more handling
                            photoData.DateCaptured = tag.Description;
                            break;
                        case "Exposure Time":
                            photoData.ShutterSpeed = tag.Description;
                            break;
                        case "Software":
                            photoData.Software = tag.Description;
                            break;
                        default:
                            break;
                    
                    }

                }
            }

            return photoData;
        }
    }
}
