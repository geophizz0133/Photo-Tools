using Photo_Tools;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

Console.WriteLine("Starting Tests");


try
{
//Establish database connection or create the DB if it doesn't exist
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler($"D:/Scratch/PhotoTools.db", $"D:/Scratch/CREATE TABLE PhotoList .txt");
	Console.WriteLine("Connecting with specific location and SQL Code - Works");

    //Clear the PhotoList table
    Console.WriteLine("Clearing the database");
    dbTester.RunSQLCommand("delete from PhotoList");

    //read data from photo files
    MetadataHandler metadataHandler = new MetadataHandler();
    
    List<PhotoData> photoData = new List<PhotoData>();

    Console.WriteLine("Cataloging Photos");


    DirectoryInfo FileFolder = new DirectoryInfo(@"Q:/Photo Library/2014/2014-12");
    Console.WriteLine("Starting folder scan:");
    foreach (var filename in FileFolder.EnumerateFiles())
    {
        Console.Write(".");
        try
        {
            string filePath = filename.FullName.ToString();
            Debug.Print($"Reading {filePath}"); 
            PhotoData photo = metadataHandler.ReadPhoto(filePath);
            dbTester.InsertSinglePhoto(photo);
            photoData.Add(photo);
            //photoData.Add(metadataHandler.ReadPhoto(filePath));
            
        }
        catch (Exception)
        {
           // Console.WriteLine($"File type undetermined, skipping: {filename.FullName.ToString()}");
        }
            
    }
    Console.Clear();
  

    //Update the file prefix field in the DB (It is faster than doing it in c#)
    Console.WriteLine("Updating Low Hanging Fruit");
    dbTester.UpdateLowHangingFruit();

    Console.WriteLine("Retrieving Originals");
    List<PhotoData> Originals = dbTester.RunSQLGetPhotoCommand("SELECT * from PhotoList where [PHOTO_STATUS]='ORIGINAL'");
    foreach (PhotoData photo in Originals)
    {
        Console.WriteLine();
        Console.WriteLine(photo.ID);
        Console.WriteLine(photo.FileName);
        Console.WriteLine(photo.DateCaptured);
        Console.WriteLine(photo.CameraMake);
        Console.WriteLine(photo.CameraModel);
        Console.WriteLine(photo.Software);
    }

    Console.WriteLine($"Press any key to quit");Console.Read();
}
catch (Exception)
{

    Console.WriteLine("Connecting with specific location and SQL Code - FAILED");
    throw;
}

