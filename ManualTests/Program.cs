using Photo_Tools;
using System.Collections.Generic;

Console.WriteLine("Starting Tests");

try
{
//Establish database connection or create the DB if it doesn't exist
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler($"D:/Scratch/PhotoTools.db", $"D:/Scratch/CREATE TABLE PhotoList .txt");
	Console.WriteLine("Connecting with specific location and SQL Code - Works");

    //read data from photo files
    MetadataHandler metadataHandler = new MetadataHandler();
    
    List<PhotoData> photoData = new List<PhotoData>();

    DirectoryInfo FileFolder = new DirectoryInfo(@"Q:/Photo Library/2011/2011-04");
    Console.WriteLine("Starting folder scan:");
    foreach (var filename in FileFolder.EnumerateFiles())
    {
        try
        {
            string filePath = filename.FullName.ToString();
            photoData.Add(metadataHandler.ReadPhoto(filePath));
            
        }
        catch (Exception)
        {
            Console.WriteLine($"File type undetermined, skipping: {filename.FullName.ToString()}");
        }
    }

    foreach(PhotoData p in photoData)
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
        Console.WriteLine($"Date/Time Captured: {p.DateCaptured}");
        Console.WriteLine($"F-Stop: {p.fStop}");
        Console.WriteLine($"Shutter Speed: {p.ShutterSpeed}");
        Console.WriteLine($"Focal Length: {p.FoclLength}");
        Console.WriteLine($"Software: {p.Software}");
    }

    Console.WriteLine($"Press any key to quit");Console.Read();
}
catch (Exception)
{

    Console.WriteLine("Connecting with specific location and SQL Code - FAILED");
    throw;
}

