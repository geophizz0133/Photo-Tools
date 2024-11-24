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
using System.Xml;
using System.Diagnostics;


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

        public void InsertSinglePhoto(PhotoData photoData) 
        { 
            //Insert a single PhotoData object into the database
            using (DBConnection) 
            { 
            SqliteCommand InsertCommand = new SqliteCommand();
                try
                {
                    InsertCommand.Connection = DBConnection;
                    InsertCommand.CommandText = ($"INSERT INTO PhotoList(ID, FILE_PATH, FILE_NAME, FILE_EXT, EXIF_DATE_CAPTURED, EXIF_CAMERA_MAKE, EXIF_FOCAL_LENGTH, EXIF_F_STOP, EXIF_SHUTTER_SPEED,EXIF_SOFTWARE,FILE_SIZE, EXIF_WIDTH, EXIF_HEIGHT, EXIF_FULL_IMAGE_SIZE,DATE_LAST_MODIFIED) VALUES( \"{photoData.ID.ToString()}\",\"{photoData.FileName}\",\"{photoData.FilePath}\",\"{photoData.Extension}\",\"{photoData.DateCaptured}\",\"{photoData.CameraMake}\",\"{photoData.FocalLength}\",\"{photoData.fStop}\",\"{photoData.ShutterSpeed}\",\"{photoData.Software}\",\"{photoData.FileSize}\",\"{photoData.ImageHeight}\",\"{photoData.ImageWidth}\",\"{photoData.FullImageSize}\",\"{photoData.DateLastModified}\")");
                    Debug.Print(InsertCommand.CommandText);

                    try
                    {
                        DBConnection.Open();
                        InsertCommand.ExecuteNonQuery();
                        DBConnection.Close();
                    }
                    catch (Exception)
                    {

                        Debug.Print($"Database Insert Failed for {InsertCommand.CommandText}");
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void InsertPhotoList(List<PhotoData> photoData) 
        { 
            //Insert a list of photodata objcts
        }
        
        
        public List<PhotoData> GetDuplicateList() 
        {   //run SQLite Script to get dulicates
            //return a list of duplicates in the form of a PhotoData List
            return new List<PhotoData>(); 
        }

        public void ExtractDuplicates(List<PhotoData> list, string FolderLocation)
        {
            //Take the list of duplicates and move them to a duplicates folder structure

        }

    }
}
