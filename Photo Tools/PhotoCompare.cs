using System;
using System.Collections;
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
       // ImageHandler PhotoImageHandler = new ImageHandler();

        public List<PhotoData> OriginalPhotos = new List<PhotoData>();
        public List<PhotoData> SecondaryPhotos = new List<PhotoData>();



        public void Compare(string? OriginalType = "RAW")
        {
            string secondaryPhotosSQL = string.Empty;
            int counter = 0;
            switch (OriginalType)
            {
                case ("RAW"):
                    { 
                        OriginalPhotos = PhotoDBHandler.GetListofPhotosFromDB($"SELECT * from vw_ALL_ORIGINALS");
                        secondaryPhotosSQL = $"Select * from PhotoList where [FILE_NAME] not in(Select [DESIGNATED_ORIGINAL] from vw_DESIGNATED_ORIGINALS) AND [PHOTO_STATUS] is null";
                        Console.WriteLine($"PhotoCompare.Compare() - Original Photos Retrieved:{OriginalPhotos.Count}");
                        break;
                     }
                case ("jpg"): //Edited jpg
                    {
                        OriginalPhotos = PhotoDBHandler.GetListofPhotosFromDB($"SELECT * from vw_ALL_jpg_COPIES");
                        secondaryPhotosSQL = "";
                        break;
                    }
                case ("png"):
                    {
                        OriginalPhotos = PhotoDBHandler.GetListofPhotosFromDB($"SELECT * from vw_ALL_png_COPIES");
                        break;
                    }
            }
            foreach (PhotoData OriginalPhoto in OriginalPhotos)
            {
                Console.WriteLine($"PhotoCompare.Compare() - Checking {OriginalPhoto.FileName}:    ");

               // SecondaryPhotos = PhotoDBHandler.GetListofPhotosFromDB($"Select * from PhotoList where [FILE_PATH] not in(Select [DESIGNATED_ORIGINAL] from vw_DESIGNATED_ORIGINALS) AND [PHOTO_STATUS] is null");  //this pulls the rest of the images that have not been previously designated
                SecondaryPhotos = PhotoDBHandler.GetListofPhotosFromDB(secondaryPhotosSQL);
                //Console.WriteLine($"- {SecondaryPhotos.Count} Secondary Photos Retreieved");

                foreach (PhotoData SecondPhoto in SecondaryPhotos)
                    {
                        Debug.Print($"Checking photo set {OriginalPhoto.FileName} / {SecondPhoto.FileName}");
                        Console.WriteLine($"Checking photo set {OriginalPhoto.FileName} / {SecondPhoto.FileName}");

                    //enable the next two lines and add a breakpoint if a check on a particular file is needed - change the file name string appropriately
                    //if (SecondPhoto.FileName == "IMG_0404 BW EDIT_1.tiff") 
                   // { Console.WriteLine("Errant Photo"); }


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

                                    //Skip MOV screenshots
                                    if(OriginalPhoto.Extension.ToLower() == ".mov" && SecondPhoto.Extension.ToLower() == ".png"){ break; } //Skip it because it is a png screenshot of an mov and width/height of the .mov can't be evaluated
                                    
                                
                                //If the height and width do not match the original, it is a version
                                //Sometimes a png file mixes up the height and width values so it has to be checked against both
                                try
                                {
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageHeight.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageWidth.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageWidth.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageHeight.Substring(0, 4)) { counter++; }
                                    if (SecondPhoto.Extension == OriginalPhoto.Extension) { counter+=2; }
                                    if (SecondPhoto.isMonochrome != OriginalPhoto.isMonochrome) { counter--; }
                                    if (SecondPhoto.DateLastModified != OriginalPhoto.DateLastModified) { counter--; }
                                    //if (SecondPhoto.RGBHash == OriginalPhoto.RGBHash) { counter++; }
                                }
                                catch (Exception) //Generally thrown if the resulution is less that 1000x1000 pixels
                                {
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageHeight.Substring(0, 3)) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageWidth.Substring(0, 3)) { counter++; }
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageWidth.Substring(0, 3)) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageHeight.Substring(0, 3)) { counter++; }
                                    if (SecondPhoto.isMonochrome == OriginalPhoto.isMonochrome) { counter++; }
                                    if (SecondPhoto.DateLastModified == OriginalPhoto.DateLastModified) { counter++; }
                                    //if (SecondPhoto.RGBHash == OriginalPhoto.RGBHash) { counter++; }

                                }
                                break;
                                }
                                default: //All other file types
                                {
                                
                                    if ((SecondPhoto.DateCaptured != OriginalPhoto.DateCaptured)) 
                                    { counter = 1;
                                    break; 
                                    } //The photos are unrelated
                                    
                                    //If all 5 of these properties match, the SecondPhoto is a duplicate
                                    //If the software is different, it means the photo has been edited and is a version
                                    counter = 0;
                                   
                                    if (SecondPhoto.DateCaptured == OriginalPhoto.DateCaptured) { counter++; }
                                    if (SecondPhoto.Extension == OriginalPhoto.Extension) { counter++; }
                                    if (SecondPhoto.ImageHeight == OriginalPhoto.ImageHeight) { counter++; }
                                    if (SecondPhoto.ImageWidth == OriginalPhoto.ImageWidth) { counter++; }
                                //if (SecondPhoto.RGBHash == OriginalPhoto.RGBHash) { counter++; }

                                //Criteria causing subtractions from the counter
                                    if (SecondPhoto.DateLastModified != OriginalPhoto.DateLastModified) { counter--; }

                                //Criteria causing hard resets of the Duplicate score
                                if (SecondPhoto.isMonochrome != OriginalPhoto.isMonochrome) { counter = 1; }//If one is in Mono, it is a version
                                    if (SecondPhoto.Software != OriginalPhoto.Software) { counter = 1; } //Different software with all else the same is always a version
                                    if (SecondPhoto.FilePath.ToUpper().Contains("VERSION")) {  counter = 1; } //The fact that it is a version is distinctly called out
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
