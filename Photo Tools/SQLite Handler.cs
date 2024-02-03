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

        public void CreateDB(string DatabaseLocation, string sqlScript)
        {
            using (SqliteConnection setupConn = new SqliteConnection("Data Source =" + DatabaseLocation))
            {
                SqliteCommand setupDBCommand = new SqliteCommand("Create Table MEMBER(TIME datetime, APPLICATION Varchar(20), USER Varchar(30), EVENT varchar (500))");
                setupConn.Open();
                setupDBCommand.Connection = setupConn;
                setupDBCommand.ExecuteNonQuery();
            }
        }

        public bool DatabaseExists(string DatabaseLocation) 
        {
            SqliteConnection setupConn = DBConnection;

            using (setupConn)
            {
                try
                {
                    setupConn.Open();
                    SqliteCommand setupCommand = setupConn.CreateCommand();
                    setupCommand.CommandText = ($"SELECT * FROM SQLITE_MASTER where Type = 'table' and name = 'List'"); //This will return false if the table exists but is empty
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
