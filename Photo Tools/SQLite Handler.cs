using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;


namespace Photo_Tools
{
    public class SQLite_Handler
    {
        public SqliteConnection DBConnection { get; set; }

        public SQLite_Handler(string dbLocation, string SQLscript)
        {
            DBConnection = GetSqliteConnection(dbLocation);
            if (!DatabaseExists(dbLocation))
            {
                CreateDB(dbLocation,SQLscript);
            }
        }

        public SqliteConnection GetSqliteConnection(string DBLocation)
        {
            SqliteConnection dbConn = new SqliteConnection("Data Source =" + DBLocation);
            {
                return dbConn;
            }
        }

        public void CreateDB(string DatabaseLocation, string sqlScriptLocation)
        {
            using (DBConnection)
            //using (SqliteConnection setupConn = new SqliteConnection("Data Source =" + DatabaseLocation))
            {
                SqliteCommand RunScript = new SqliteCommand();
                RunScript.Connection = DBConnection;
                RunScript.CommandText = File.ReadAllText(sqlScriptLocation);
                DBConnection.Open();
                RunScript.ExecuteNonQuery();
                DBConnection.Close();
            }
        }

        public bool DatabaseExists(string DatabaseLocation) 
        {
            using (DBConnection)
            {
                try
                {
                    DBConnection.Open();
                    SqliteCommand setupCommand = DBConnection.CreateCommand();
                    setupCommand.CommandText = ($"SELECT * FROM sqlite_master WHERE type = 'table' AND name = 'PhotoList'"); //This will return false if the table exists but is empty
                    SqliteDataReader dbExistsReader = setupCommand.ExecuteReader();
                    dbExistsReader.Read();
                    bool dbExists = dbExistsReader.HasRows;
                    dbExistsReader.Close();
                    return dbExists;
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }

 


    }
}
