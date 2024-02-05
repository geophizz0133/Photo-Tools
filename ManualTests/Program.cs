using Photo_Tools;

Console.WriteLine("Starting Tests");

try
{
//Establish database connection or create the DB if it doesn't exist
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler($"D:/Scratch/PhotoTools.db", $"D:/Scratch/CREATE TABLE PhotoList .txt");
	Console.WriteLine("Connecting with specific location and SQL Code - Works");

    //read data from photo files
    MetadataHandler metadataHandler = new MetadataHandler();
    metadataHandler.ReadPhoto($"D:/Scratch/2023-01-065 (DSC05176).JPG");



    Console.WriteLine($"Press any key to quit");Console.Read();
}
catch (Exception)
{

    Console.WriteLine("Connecting with specific location and SQL Code - FAILED");
    throw;
}

