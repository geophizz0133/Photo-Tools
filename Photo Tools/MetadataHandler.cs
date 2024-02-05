using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Data.Sqlite;
using System.ComponentModel.Design;

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
            public DateTime DateCaptured;
            public string CameraMake;
            public string FoclLength;
            public string fStop;
            public string ShutterSpeed;

        }

        public PhotoData ReadPhoto(string PhotoFileLocation)
        {
            PhotoData photoData = new PhotoData();

            var directories = ImageMetadataReader.ReadMetadata(PhotoFileLocation);

           //Cycle thru all directories
            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    Console.WriteLine(tag.ToString());
                    //photoData.CameraMake = ExifSubIfdDirectory.TagMake.ToString();

                }
            }

            //Get a specific directory
            // obtain the Exif SubIFD directory
            Console.WriteLine(Environment.NewLine + "Starting single value read");
            var directoryIFD = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directoryIFD != null)
            {
                // query the tag's value
                if (directoryIFD.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
                {
                    photoData.DateCaptured = dateTime;
                    Console.WriteLine($"Date/Time Original = {photoData.DateCaptured.ToString()}");
                }
               

            

            } 

            // obtain a specific directory
            var directoryIFD2 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            if (directoryIFD2 != null) //Descriptors for not directly readable fields?
            {
                // create a descriptor
                var descriptor = new ExifSubIfdDescriptor(directoryIFD2);

                // get tag description
                photoData.fStop = descriptor.GetFNumberDescription();
                Console.WriteLine($"F-Stop = {photoData.fStop}");

                photoData.FoclLength = descriptor.Get35MMFilmEquivFocalLengthDescription();
                Console.WriteLine($"Focal Length = {photoData.FoclLength}");

                photoData.ShutterSpeed = descriptor.GetExposureTimeDescription();
                Console.WriteLine($"Shutter Speed = {photoData.ShutterSpeed}");

                var directoryIFD0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                photoData.CameraMake = directoryIFD0.GetDescription(ExifIfd0Directory.TagMake);
                Console.WriteLine($"Camera Make = {photoData.CameraMake.ToString()}");
            }

            return photoData;
        }
    }
}
