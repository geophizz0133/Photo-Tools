using Photo_Tools;

namespace PhotoTools_Tests
{

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            EstablishConnection();
            Console.Read();
        }

        [Test]
        //DatabaseExists handles specified database and sql script - Used when user knows a new DB is needed
        public void EstablishConnection()
        {   string dbLocation = ($"D:/Scratch/TestDB.db");
            string sqlCommandScript = "Create Table LOG(TIME datetime, APPLICATION Varchar(20), USER Varchar(30), EVENT varchar (500))";
            Photo_Tools.SQLite_Handler ToolTest = new Photo_Tools.SQLite_Handler(dbLocation,sqlCommandScript);
            ToolTest.DatabaseExists(dbLocation);
            
            if(ToolTest.DatabaseExists(dbLocation) is true)
            {
                Assert.Pass();
                Console.WriteLine("Test Passed");
            }
            else 
            { 
                Assert.Fail();
                Console.WriteLine($"Test Failed");
            }
        }
    }
}