using Photo_Tools;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

Console.Clear();
Console.WriteLine("Starting Tests");


/*
// Path to your image file and sidecar file
string imagePath = "path/to/your/photo.jpg";
string sidecarPath = "path/to/your/photo.xmp";

// Create an instance of XmpHandler
XmpHandler xmpHandler = new XmpHandler();

// Write XMP data
xmpHandler.WriteXmpData(sidecarPath, "dc:title", "My Photo Title");

// Read XMP data
string title = xmpHandler.ReadXmpData(sidecarPath, "dc:title");
Console.WriteLine($"Title: {title}");

// Update XMP data
xmpHandler.UpdateXmpData(sidecarPath, "dc:title", "Updated Photo Title");

// Read XMP data again to confirm update
title = xmpHandler.ReadXmpData(sidecarPath, "dc:title");
Console.WriteLine($"Updated Title: {title}");
*/
/*
XMPHandler xmpHandler = new XMPHandler();
string filePath = (@"D://Scratch//PhotoTools Samples//PhotoSampleData//DSC00003-19643.xmp");
FileInfo fileInfo = new FileInfo(filePath);
string XMPData = xmpHandler.ReadXmpData(filePath, "tiff:Make");
Console.WriteLine(XMPData);
Console.Write($"Stop Here"); Console.Read();

*/

try
{
//Establish database connection or create the DB if it doesn't exist
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler($"D:/Scratch/PhotoTools.db", $"D:/Scratch/Create_PhotoTools DB.sql");
	Console.WriteLine("Connecting with specific location and SQL Code - Works");

  
    //Clear the PhotoList table
    Console.WriteLine("Clearing the database");
    dbTester.RunSQLCommand("delete from PhotoList");
  
    List<PhotoData> photoData = new List<PhotoData>();

    Console.WriteLine("Cataloging Photos");


    DirectoryInfo FileFolder = new DirectoryInfo(@"D://Scratch//PhotoTools Samples//PhotoSampleData");
    //DirectoryInfo FileFolder = new DirectoryInfo(@"D://Scratch//PhotoTools Samples//BigSample");

    Console.WriteLine("Starting folder scan:");
    int fileLocation = 0;
    int fileCount = FileFolder.EnumerateFileSystemInfos().Count();

    foreach (var filename in FileFolder.EnumerateFiles())
    {
        
        try
        {
            fileLocation++;
            if (filename.Extension.ToUpper() != ".XMP" || filename.Extension.ToUpper() !=".DB")
            {
                Console.SetCursorPosition(0, 5);
                Console.Write($"Reading {fileLocation}/{fileCount}");
                Console.SetCursorPosition(0, 6);
                string filePath = filename.FullName.ToString();
                Debug.Print($"Reading {filePath}");
                MetadataHandler metadataHandler = new MetadataHandler();
                PhotoData photo = metadataHandler.ReadPhoto(filePath);
                metadataHandler.Dispose();
                dbTester.InsertSinglePhoto(photo);
            }
        }
        catch (Exception)
        {
            //Console.WriteLine($"File type undetermined, skipping: {filename.FullName.ToString()}");
            fileLocation++;
        }
        dbTester.Dispose();

        GC.Collect();
    }

  
  

    //Update several fields in the DB (It is faster than doing it in c#)
    Console.WriteLine("Applying Pre Processing Corrections");
    dbTester.UpdateLowHangingFruit();

    Console.WriteLine("Retrieving Originals");
    
    //Find the Versions and Duplicates
    Console.WriteLine("Starting RAW Photo Comparison");
    PhotoCompare RunCompare = new PhotoCompare();
    RunCompare.Compare();
    Console.WriteLine("RAW Comparison Done");

    Console.WriteLine("Starting tiff Photo Comparison");
    RunCompare.Compare("tif");
    Console.WriteLine("tiff Comparison Done");

  //  Console.WriteLine("Starting dng Photo Comparison");
  //  RunCompare.Compare("dng");
  //  Console.WriteLine("dng Comparison Done");


    Console.WriteLine($"DONE - Press any key to quit");Console.Read();
    
    dbTester.Dispose();
}
catch (Exception)
{

    Console.WriteLine("Connecting with specific location and SQL Code - FAILED");
    throw;
}


