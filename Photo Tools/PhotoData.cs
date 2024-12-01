using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Photo_Tools
{
    public class PhotoData
    {
        public int recordnumber = 0;
        public string ID = String.Empty;
        public string FilePath = String.Empty;
        public string FileName = String.Empty;
        public string Extension = String.Empty;
        public string DateCaptured = String.Empty;
        public string CameraMake = String.Empty;
        public string CameraModel = String.Empty;
        public string FocalLength = String.Empty;
        public string fStop = String.Empty;
        public string ShutterSpeed = String.Empty;
        public string Software = String.Empty;
        public bool ReducedResolution = false;
        public string FileSize = String.Empty;
        public string ImageHeight = String.Empty;
        public string ImageWidth = String.Empty;
        public string FullImageSize = String.Empty;
        public string DateLastModified = String.Empty;
        public string PhotoStatus = String.Empty;
        public string FilePrefix = string.Empty;
        public bool DataChanged = false;
        public int DuplicateScore = 0;
        public bool isMonochrome = false;
        public string RGBHash = String.Empty;
        
    }
}
