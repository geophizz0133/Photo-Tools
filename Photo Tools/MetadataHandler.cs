using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Data.Sqlite;

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
            public string DateCaptured;
            public string CameraMake;
            public string FoclLength;
            public string fStop;
            public string ShutterSpeed;

        }

        public PhotoData ReadPhoto(string PhotoFileLocation)
        {
            PhotoData photoData = new PhotoData();
            //ImageMetadataReader photoReader = new ImageMetadataReader(PhotoFileLocation);
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
            var directory2 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory2 != null)
            {
                // query the tag's value
                if (directory2.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
                    //return dateTime;
                    Console.WriteLine($"Date/Time Original = {dateTime.ToString()}");



            }

            // obtain a specific directory
            var directory3 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            if (directory3 != null)
            {
                // create a descriptor
                var descriptor = new ExifSubIfdDescriptor(directory3);

                // get tag description
                String program = descriptor.GetExposureProgramDescription();
                Console.WriteLine($"Exposure Program = {program}");

                program = descriptor.Get35MMFilmEquivFocalLengthDescription();
                Console.WriteLine($"Focal Length = {program}");

                program = descriptor.GetExposureTimeDescription();
                Console.WriteLine($"Shutter Speed = {program}");
            }

            return photoData;
        }
    }
}
