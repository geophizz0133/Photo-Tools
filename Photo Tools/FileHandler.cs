using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photo_Tools
{
    internal class FileHandler
    {
        public void ExtractDuplicates(string folder) 
        { 
        //Get a list of duplicate files from vw_ORIGINALS_AND_DUPLICATES and move those files to a different folder
        
        // SQLite_Handler photoDBHandler = new SQLite_Handler();

            //photoDBHandler.GetListofPhotosFromDB();
        }

        public void MarkDuplicates() 
        {
            //Get a list of duplicate files from vw_ORIGINALS_AND_DUPLICATES then write the keyword "Duplicate" to the ITPC Keywords
        
        }
    }
}
