using Photo_Tools;
using System.Collections.Generic;
using System.Diagnostics;

Console.WriteLine("Starting Tests");

try
{
//Establish database connection or create the DB if it doesn't exist
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler($"D:/Scratch/PhotoTools.db", $"D:/Scratch/CREATE TABLE PhotoList .txt");
	Console.WriteLine("Connecting with specific location and SQL Code - Works");

    //read data from photo files
    MetadataHandler metadataHandler = new MetadataHandler();
    
    List<PhotoData> photoData = new List<PhotoData>();

    Console.WriteLine("Retrieving Originals");



    DirectoryInfo FileFolder = new DirectoryInfo(@"Q:/Photo Library/2014/2014-12");
    Console.WriteLine("Starting folder scan:");
    foreach (var filename in FileFolder.EnumerateFiles())
    {
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
            Console.WriteLine($"File type undetermined, skipping: {filename.FullName.ToString()}");
        }
    }

    /*  foreach(PhotoData p in photoData)
      {

          try
          {
              Console.WriteLine(Environment.NewLine);
              Console.WriteLine($"File Name: {p.FileName}");
              dbTester.InsertSinglePhoto(p);
          }
          catch (Exception)
          {

              throw;
          }

          Console.WriteLine($"Path: {p.FilePath}");
          Console.WriteLine($"File Type: {p.Extension}");
          Console.WriteLine($"Camera Make: {p.CameraMake}");
          Console.WriteLine($"Camera Model: {p.CameraModel}");
          Console.WriteLine($"Date/Time Captured: {p.DateCaptured}");
          Console.WriteLine($"F-Stop: {p.fStop}");
          Console.WriteLine($"Shutter Speed: {p.ShutterSpeed}");
          Console.WriteLine($"Focal Length: {p.FocalLength}");
          Console.WriteLine($"Software: {p.Software}");
          Console.WriteLine($"Reduced Resolution = {p.ReducedResolution.ToString()}");
          Console.WriteLine($"File Size: {p.FileSize.ToString()}");

      }
    */

    //Update the file prefix field in the DB (It is faster than doing it in c#)
    Console.WriteLine("Updating Low Hanging Fruit");
    dbTester.UpdateLowHangingFruit();

    Console.WriteLine("Retrieving Originals");
    List<PhotoData> Originals = dbTester.RunSQLGetPhotoCommand("SELECT * from PhotoList where [PHOTO_STATUS]='ORIGINAL'");
    foreach (var photo in Originals)
    {
        Console.WriteLine(photo.FileName);
        Console.WriteLine(photo.DateCaptured);
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

