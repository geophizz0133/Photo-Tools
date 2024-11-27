using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Photo_Tools
{
    public class PhotoCompare
    {
        public PhotoCompare() { }

        SQLite_Handler PhotoDB = new SQLite_Handler();
        public List<PhotoData> OriginalPhotos = new List<PhotoData>();
        public List<PhotoData> SecondaryPhotos = new List<PhotoData>();



        public void Compare()
        {
            int counter = 0;

            OriginalPhotos = PhotoDB.GetListofPhotosFromDB($"SELECT * FROM PhotoList WHERE [PHOTO_STATUS] = 'ORIGINAL'");
            foreach (PhotoData OriginalPhoto in OriginalPhotos)
            {
                SecondaryPhotos = PhotoDB.GetListofPhotosFromDB($"SELECT * from PhotoList WHERE [FILE_PREFIX]='{OriginalPhoto.FilePrefix}'");
                foreach (PhotoData SecondPhoto in SecondaryPhotos)
                {


                    switch (SecondPhoto.Extension.ToLower())
                    {
                        case (".png:"): //PNG files have little metadata so only the image height and width can be compared
                            {
                                //This makes the math work
                                counter = 1;

                                //If the height and width do not match the original, it is a version
                                //Sometimes a png file mixes up the height and width values so it has to be checked against both
                                if (SecondPhoto.ImageHeight == OriginalPhoto.ImageHeight.Substring(0, 4)) { counter++; }
                                if (SecondPhoto.ImageWidth == OriginalPhoto.ImageWidth.Substring(0, 4)) { counter++; }
                                if (SecondPhoto.ImageHeight == OriginalPhoto.ImageWidth.Substring(0, 4)) { counter++; }
                                if (SecondPhoto.ImageWidth == OriginalPhoto.ImageHeight.Substring(0, 4)) { counter++; }
                                break;
                            }
                        default:
                            {   //If all three of these properties match, the SecondPhoto is a duplicate
                                //If the software is different, it means the photo has been edited and is a version
                                counter = 0;
                                if (SecondPhoto.DateCaptured == OriginalPhoto.DateCaptured) { counter++; }
                                if (SecondPhoto.ImageHeight == OriginalPhoto.ImageHeight) { counter++; }
                                if (SecondPhoto.ImageWidth == OriginalPhoto.ImageHeight) { counter++; }
                                if (SecondPhoto.Software != OriginalPhoto.Software) { counter = 0; }
                                break;
                            }
                    }



                    switch (counter)
                    {
                        case (0):
                        case (1):
                        case (2):
                            { SecondPhoto.PhotoStatus = "VERSION"; break; }
                        case (> 2):
                            { SecondPhoto.PhotoStatus = "DUPLICATE"; break; }
                    }

                    PhotoDB.RunSQLCommand($"UPDATE PhotoList SET [PHOTO_STATUS] = {SecondPhoto.PhotoStatus} WHERE [ID] = {SecondPhoto.ID}");
                }
            }

        }
    }
}
