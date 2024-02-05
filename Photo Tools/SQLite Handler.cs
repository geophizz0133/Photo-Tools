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
            return File.Exists(DatabaseLocation);
        }

 


    }
}
