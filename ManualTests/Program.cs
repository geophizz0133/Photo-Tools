using Photo_Tools;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

Console.Clear();
Console.WriteLine("Starting Tests");

var cfg = Photo_Tools.AppConfig.Load();

try
{
//Establish database connection or create the DB if it doesn't exist
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler(cfg.DbLocation, cfg.SqlScript);
	Console.WriteLine("Connecting with specific location and SQL Code - Works");

  
    //Clear the PhotoList table
    Console.WriteLine("Clearing the database");
    dbTester.RunSQLCommand("delete from PhotoList");
  
    List<PhotoData> photoData = new List<PhotoData>();

    Console.WriteLine("Cataloging Photos");


    DirectoryInfo FileFolder = new DirectoryInfo(cfg.SampleDataPath);
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

    Console.WriteLine("Starting jpg Photo Comparison");
    RunCompare.Compare("jpg");
    Console.WriteLine("jpg Comparison Done");

    Console.WriteLine("Starting DNG Photo Comparison");
    RunCompare.Compare("dng");
    Console.WriteLine("DNG Comparison Done");


    Console.WriteLine($"DONE - Press any key to quit");Console.Read();
    
    dbTester.Dispose();
}
catch (Exception)
{

    Console.WriteLine("Connecting with specific location and SQL Code - FAILED");
    throw;
}
