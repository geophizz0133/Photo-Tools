using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Photo_Tools
{
    public class PhotoCompare
    {
        public PhotoCompare() { }

        SQLite_Handler PhotoDBHandler = new SQLite_Handler();
        ImageHandler PhotoImageHandler = new ImageHandler();

        public List<PhotoData> OriginalPhotos = new List<PhotoData>();
        public List<PhotoData> SecondaryPhotos = new List<PhotoData>();



        public void Compare()
        {
            int counter = 0;

            OriginalPhotos = PhotoDBHandler.GetListofPhotosFromDB($"SELECT * from PhotoList WHERE [ID] in (SELECT DISTINCT [ID] FROM vw_ALL_ORIGINALS)");
            Console.WriteLine($"PhotoCompare.Compare() - Original Photos Retrieved:{OriginalPhotos.Count}");
    
            foreach (PhotoData OriginalPhoto in OriginalPhotos)
            {
                Console.WriteLine($"PhotoCompare.Compare() - Checking {OriginalPhoto.FileName}");


                    SecondaryPhotos = PhotoDBHandler.GetListofPhotosFromDB($"SELECT * from PhotoList WHERE [FILE_PREFIX]='{OriginalPhoto.FilePrefix}' AND ([PHOTO_STATUS] is null or ([DUPLICATE_SCORE]<>0 or [DUPLICATE_SCORE] is null))");
                    Console.WriteLine($"PhotoCompare.Compare().GetListOfPhotosFromDB(SELECT * from PhotoList WHERE [FILE_PREFIX]='{OriginalPhoto.FilePrefix}' AND [PHOTO_STATUS] is null)");                   
                    Console.WriteLine($" PhotoCompare.Compare() - {SecondaryPhotos.Count} Secondary Photos Retreieved");
                
                    foreach (PhotoData SecondPhoto in SecondaryPhotos)
                    {
                        Debug.Print($"Checking photo set {OriginalPhoto.FileName} / {SecondPhoto.FileName}");

                        switch (SecondPhoto.Extension.ToLower())
                        {
                        case (".mov"):
                        case (".mp4"):
                            //Ignore video files
                            { break; }
                        case (".png"): //PNG files have little metadata so only the image height and width can be compared
                                {
                                    //This makes the math work
                                    counter = 3;
                                    if(OriginalPhoto.Extension.ToLower() == ".mov" && SecondPhoto.Extension.ToLower() == ".png"){ break; } //Skip it because width/height of the .mov can't be evaluated
                                    
                                    Debug.Print($"OriginalPhoto {OriginalPhoto.Extension.ToLower()} - {OriginalPhoto.Extension.ToLower() != ".mov"}");
                                    Debug.Print($"OriginalPhoto {OriginalPhoto.Extension.ToLower()} - {OriginalPhoto.Extension.ToLower() != ".mp4"}");
                                    Debug.Print($"SecondPhoto {SecondPhoto.Extension.ToLower()} - {SecondPhoto.Extension.ToLower() != ".mov"}");
                                    Debug.Print($"SecondPhoto {SecondPhoto.Extension.ToLower()} - {SecondPhoto.Extension.ToLower() != ".mp4"}");
                                    Debug.Print($"If any of the above is true, this code should not execute");

                                    
                                    Debug.Print($"Checking photo set {OriginalPhoto.FileName} / {SecondPhoto.FileName}");
                                    //If the height and width do not match the original, it is a version
                                    //Sometimes a png file mixes up the height and width values so it has to be checked against both
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageHeight.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageWidth.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageWidth.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageHeight.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.isMonochrome == OriginalPhoto.isMonochrome) { counter++; }
                                    if (SecondPhoto.RGBHash == OriginalPhoto.RGBHash) { counter++; }

                                    
                                    break;
                                }
                            default:
                                {   //If all 4 of these properties match, the SecondPhoto is a duplicate
                                    //If the software is different, it means the photo has been edited and is a version
                                    counter = 1;
                                    
                                    if (SecondPhoto.DateCaptured == OriginalPhoto.DateCaptured) { counter++; }
                                    if (SecondPhoto.Extension == OriginalPhoto.Extension) { counter++; }
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageHeight) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageHeight) { counter++; }
                                    if (SecondPhoto.isMonochrome == OriginalPhoto.isMonochrome) { counter++; }
                                    if (SecondPhoto.RGBHash == OriginalPhoto.RGBHash) { counter++; }
                                    if (SecondPhoto.Software != OriginalPhoto.Software) { counter = 1; } //Different software with all else the same is always a version
                                    break;
                                }
                        }
                        SecondPhoto.DuplicateScore = counter;
                        switch (counter)
                        {
                            case (0):
                                { SecondPhoto.PhotoStatus = "ORIGINAL"; break; } //Whoah man, this should never happen here
                            case (1):
                            case (2):
                            case (3):
                                { SecondPhoto.PhotoStatus = "VERSION"; break; }
                            case (> 3):
                                { SecondPhoto.PhotoStatus = "DUPLICATE"; break; }
                        }
                        Debug.Print($"PhotoCompare.Compare() - Updating {SecondPhoto.FileName} to {SecondPhoto.PhotoStatus}");
                        PhotoDBHandler.RunSQLCommand($"UPDATE PhotoList SET [PHOTO_STATUS] = '{SecondPhoto.PhotoStatus}',[DUPLICATE_SCORE] = {SecondPhoto.DuplicateScore} WHERE [ID] = '{SecondPhoto.ID}'");
                        
                    }
                PhotoDBHandler.Dispose();                

            }
            
        }
    }
}
