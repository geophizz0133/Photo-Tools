using Photo_Tools;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

Console.WriteLine("Starting Tests");
/*
//Create and/or update a sidecar file (I don't know if this writes a properly formatted XMP file)
{
    string filePath = $"D:/Scratch/photo.xmp";
    string metadata = "<metadata><title>Test Title</title><description>Test Description Update</description></metadata>";
    MetadataHandler xmpHandler = new MetadataHandler();

    if (File.Exists(filePath))
    {
        
        Console.WriteLine("File exists. Updating the file...");
        xmpHandler.UpdateSidecarFile(filePath, metadata);
    }
    else
    {


        Console.WriteLine("File does not exist. Creating a new file...");
        xmpHandler.CreateSidecarFile(filePath, metadata);
    }


}
*/
try
{
//Establish database connection or create the DB if it doesn't exist
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler($"D:/Scratch/PhotoTools.db", $"D:/Scratch/CREATE TABLE PhotoList .txt");
	Console.WriteLine("Connecting with specific location and SQL Code - Works");

  

    
    //Clear the PhotoList table
    Console.WriteLine("Clearing the database");
    dbTester.RunSQLCommand("delete from PhotoList");

    //read data from photo files
   // MetadataHandler xmpHandler = new MetadataHandler();
    
    List<PhotoData> photoData = new List<PhotoData>();

    Console.WriteLine("Cataloging Photos");


    DirectoryInfo FileFolder = new DirectoryInfo(@"Q:/Photo Library/2014/2014-12");
    Console.WriteLine("Starting folder scan:");
    int fileLocation = 0;
    int fileCount = FileFolder.EnumerateFileSystemInfos().Count();

    foreach (var filename in FileFolder.EnumerateFiles())
    {
        
        try
        {
            fileLocation++;
            if (filename.Extension.ToUpper() != ".XMP")
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
                //photoData.Add(photo);

                //photoData.Add(xmpHandler.ReadPhoto(filePath));
            }
        }
        catch (Exception)
        {
            // Console.WriteLine($"File type undetermined, skipping: {filename.FullName.ToString()}");
            fileLocation++;
        }
        dbTester.Dispose();

        GC.Collect();
    }

  
  

    //Update the file prefix field in the DB (It is faster than doing it in c#)
    Console.WriteLine("Updating Low Hanging Fruit");
    dbTester.UpdateLowHangingFruit();

    Console.WriteLine("Retrieving Originals");
    
    //Find the Versions and Duplicates
    Console.WriteLine("Starting Photo Comparison");
    PhotoCompare RunCompare = new PhotoCompare();
    RunCompare.Compare();
    Console.WriteLine("Comparison Done");
    Console.WriteLine($"Press any key to quit");Console.Read();
    
    dbTester.Dispose();
}
catch (Exception)
{

    Console.WriteLine("Connecting with specific location and SQL Code - FAILED");
    throw;
}


