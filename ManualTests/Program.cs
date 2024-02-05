using Photo_Tools;

Console.WriteLine("Starting Tests");

try
{
	Photo_Tools.SQLite_Handler dbTester = new SQLite_Handler($"D:/Scratch/PhotoTools.db", $"D:/Scratch/CREATE TABLE PhotoList .txt");
	Console.WriteLine("Connecting with specific location and SQL Code - Works");
}
catch (Exception)
{

    Console.WriteLine("Connecting with specific location and SQL Code - FAILED");
    throw;
}

