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


        public struct PhotoData
        {
            public Guid ID;
            public string FilePath;
            public string FileName;
            public string Extension;
            public string DateCaptured;
            public string CameraMake;
            public string CameraModel;
            public string FoclLength;
            public string fStop;
            public string ShutterSpeed;
            public string Software;

        }

        public PhotoData ReadPhoto(string PhotoFileLocation)
        {
            PhotoData photoData = new PhotoData();
           
            FileInfo fileInfo = new FileInfo(PhotoFileLocation);
            photoData.FilePath = PhotoFileLocation;
            photoData.FileName = fileInfo.Name;
            photoData.Extension = fileInfo.Extension;

            var directories = ImageMetadataReader.ReadMetadata(PhotoFileLocation);

            string tempFStop = String.Empty;
            
            Console.WriteLine(Environment.NewLine);


            photoData.FilePath = PhotoFileLocation;
            // photoData.FileName = FileInfo(PhotoFileLocation).Name;

            //Cycle thru all directories
            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                   // Console.WriteLine($"{ tag.ToString()}");



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

   

            Console.WriteLine($"File Name: {photoData.FileName}");
            Console.WriteLine($"Path: {photoData.FilePath}");
            Console.WriteLine($"File Type: {photoData.Extension}");
            Console.WriteLine($"Camera Make: {photoData.CameraMake}");
            Console.WriteLine($"Date/Time Captured: {photoData.DateCaptured}");
            Console.WriteLine($"F-Stop: {photoData.fStop}");
            Console.WriteLine($"Shutter Speed: {photoData.ShutterSpeed}");
            Console.WriteLine($"Focal Length: {photoData.FoclLength}");
            Console.WriteLine($"Software: {photoData.Software}");

            return photoData;
        }
    }
}
