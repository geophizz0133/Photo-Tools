using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photo_Tools
{
    internal class DuplicateHandler
    {
        public DuplicateHandler() { }
        
         SQLite_Handler PhotoDB_Handler = new SQLite_Handler(); //This has to be changed to a config item

        public void ExtractDuplicates(string fileExt)
        {
            int counter = 0;

            //Get a list of duplicate files and move those files to a different folder
            List<PhotoData> Photos1 = new List<PhotoData>();
            List<PhotoData> Photos2 = new List<PhotoData>();

            Photos1 = PhotoDB_Handler.GetListofPhotosFromDB("SELECT * from PhotoList WHERE [FILE_EXT] in ('." + fileExt.ToLower() + "', '." + fileExt.ToUpper() + "')");
            foreach (PhotoData photo in Photos1) 
            {
                string SQLQuery = string.Empty;

                switch(fileExt)
                {
                    case ("dng"):
                        {   //Update the first photo as ORIGINAL
                            photo.PhotoStatus = "ORIGINAL";
                            photo.DuplicateScore = 0;
                            PhotoDB_Handler.RunSQLCommand("UPDATE PhotoList SET [PHOTO_STATUS] = 'ORIGINAL', [DUPLICATE_SCORE] = 0 Where [ID] in ('"+ photo.ID +"')");
                            PhotoDB_Handler.RunSQLCommand("UPDATE PhotoList SET [PHOTO_STATUS] = 'DUPLICATE', [DUPLICATE_SCORE] = 6 WHERE ID not in ('" + photo.ID.ToString() + "') AND [DUPLICATE_SCORE] = 0 AND [FILE_EXT] in ('." + fileExt.ToUpper() + "') AND [DATE_LAST_MODIFIED] in ('" + photo.DateLastModified + "')");
                            SQLQuery = "SELECT * from PhotoList WHERE ID not in ('" + photo.ID.ToString() + "') AND [FILE_EXT] in ('." + fileExt.ToUpper() + "') AND [DUPLICATE_SCORE] > 0 AND [EXIF_DATE_CAPTURED] in ('" + photo.DateCaptured + "')";
                            break;

                        }
                    case ("jpg"):
                    default:
                        {
                            SQLQuery = "SELECT * from PhotoList WHERE ID not in ('" + photo.ID.ToString() + "') AND [FILE_EXT] in ('." + fileExt.ToLower() + "') AND [EXIF_DATE_CAPTURED] in ('" + photo.DateCaptured + "')";
                            break;
                        }
                    case ("tif"): 
                        {

                            break;
                        }

                }

                //Photos2 = PhotoDB_Handler.GetListofPhotosFromDB("SELECT * from PhotoList WHERE ID not in (" + photo.ID.ToString() + " AND [FILE_EXT] in ('"  [EXIF_DATE_TAKEN] in (" + photo.DateCaptured + ")");
                Photos2 = PhotoDB_Handler.GetListofPhotosFromDB(SQLQuery);
                if (Photos2.Count > 0)
                {
                    foreach (PhotoData photoCopy in Photos2)
                    {
                        photoCopy.PhotoStatus = "VERSION";

                        counter = 0;
                        if (photo.DateLastModified == photoCopy.DateLastModified) { counter += 2; } else { counter = 0; }
                        if (photo.PhotoStatus == photoCopy.PhotoStatus) { counter++; }
                        if (photo.isMonochrome == photoCopy.isMonochrome) { counter++; }
                        if (photo.ImageHeight == photoCopy.ImageHeight) { counter++; }
                        if (photo.ImageWidth == photoCopy.ImageWidth) { counter++; }
                        if (photo.Software == photoCopy.Software) { counter++; }

                            switch (counter)
                        {
                            case (0):
                                photoCopy.PhotoStatus = "ORIGINAL";
                                break;
                            case (1):
                            case (2):
                            case (3):
                                photoCopy.PhotoStatus = "POSSIBLE DUPLICATE";
                                break;
                            case (> 3):
                                photoCopy.PhotoStatus = "DUPLICATE";
                                break;

                            default:
                                photoCopy.PhotoStatus = "INDETERMINATE";
                                break;
                        }

                        
                        
                            string updateSQLCommand = $"UPDATE PhotoList SET [PHOTO_STATUS] = ('" + photoCopy.PhotoStatus + "'), [DUPLICATE_SCORE] = " + counter.ToString() + " WHERE [ID] = '" + photoCopy.ID + "'";
                            PhotoDB_Handler.RunSQLCommand(updateSQLCommand);
                        

                    }
                }
                else 
                {
                    //Set the first photo to <ext>_ORIGINAL because there are no other versions or duplicates
                    photo.PhotoStatus = "ORIGINAL";
                    string updateSQLCommand = $"UPDATE PhotoList SET [PHOTO_STATUS] = ('" + fileExt + "_ORIGINAL'), [DUPLICATE_SCORE] = 0  WHERE [ID] = " + photo.ID + "'";
                }

            } 
        }

        public void FindAndMarkDuplicates(string fileType) 
        {
            //Get a list of duplicate files then write the keyword "Duplicate" to the ITPC Keywords
            
        }
    }
}
