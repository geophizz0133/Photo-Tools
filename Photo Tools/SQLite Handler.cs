using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections;


namespace Photo_Tools
{
    public class SQLite_Handler
    {
        public SqliteConnection DBConnection { get; set; }

        public SQLite_Handler(string dbLocation = $"D:/Scratch/PhotoTools.db", string SQLscript= $"D:/Scratch/CREATE TABLE PhotoList .txt")
        {
            try
            {
                DBConnection = GetSqliteConnection(dbLocation);
                if (!DatabaseExists(dbLocation))
                {
                    CreateDB(dbLocation, SQLscript);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public SqliteConnection GetSqliteConnection(string DBLocation) //I don't think this is used
        {
            SqliteConnection dbConn = new SqliteConnection("Data Source =" + DBLocation);
            {
                return dbConn;
            }
        }

        public bool DatabaseExists(string DatabaseLocation)
        {
            try
            {
                return File.Exists(DatabaseLocation);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void CreateDB(string DatabaseLocation, string sqlScriptLocation)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
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
                    InsertCommand.CommandText = ($"INSERT INTO PhotoList(ID, FILE_PATH, FILE_NAME, FILE_EXT, EXIF_DATE_CAPTURED, EXIF_CAMERA_MAKE, EXIF_CAMERA_MODEL, EXIF_FOCAL_LENGTH, EXIF_F_STOP, EXIF_SHUTTER_SPEED,EXIF_SOFTWARE,FILE_SIZE, EXIF_WIDTH, EXIF_HEIGHT, EXIF_FULL_IMAGE_SIZE,DATE_LAST_MODIFIED) VALUES( \"{photoData.ID.ToString()}\",\"{photoData.FileName}\",\"{photoData.FilePath}\",\"{photoData.Extension}\",\"{photoData.DateCaptured}\",\"{photoData.CameraMake}\",\"{photoData.CameraModel}\",\"{photoData.FocalLength}\",\"{photoData.fStop}\",\"{photoData.ShutterSpeed}\",\"{photoData.Software}\",\"{photoData.FileSize}\",\"{photoData.ImageHeight}\",\"{photoData.ImageWidth}\",\"{photoData.FullImageSize}\",\"{photoData.DateLastModified}\")");
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


        public List<PhotoData> GetAllRelatedPhotos(string FilePrefix)
        {   //run SQLite Script to get dulicates
            //return a list of duplicates in the form of a PhotoData List

            try
            {
                List<PhotoData> duplicateList = GetListofPhotosFromDB($"Select * from PhotoList Where [FILE_PREFIX] = '{FilePrefix}' AND [PHOTO_STATUS]<>'ORIGINAL'");
                return duplicateList; ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhotoData> GetListofPhotosFromDB(string SQLcommand)
        {
            List<PhotoData> photoList = new List<PhotoData>();
            
            using (SqliteCommand command = new SqliteCommand(SQLcommand, DBConnection))

            {
                DBConnection.Open();
                using (var photoReader = command.ExecuteReader())

                {
                    int recordcounter = 0;
                        while (photoReader.Read())
                        {
                            PhotoData subjectPhoto = new PhotoData();

                            subjectPhoto.recordnumber = recordcounter;
                            subjectPhoto.ID = photoReader.GetString(photoReader.GetOrdinal("ID"));
                            subjectPhoto.Extension = photoReader.GetString(photoReader.GetOrdinal("FILE_EXT"));
                            subjectPhoto.FileName = photoReader.GetString(photoReader.GetOrdinal("FILE_PATH"));
                            subjectPhoto.DateCaptured = photoReader.GetString(photoReader.GetOrdinal("EXIF_DATE_CAPTURED"));
                            subjectPhoto.Software = photoReader.GetString(photoReader.GetOrdinal("EXIF_SOFTWARE"));
                            subjectPhoto.ImageWidth = photoReader.GetString(photoReader.GetOrdinal("EXIF_WIDTH"));
                            subjectPhoto.ImageHeight = photoReader.GetString(photoReader.GetOrdinal("EXIF_HEIGHT"));
                            subjectPhoto.CameraMake = photoReader.GetString(photoReader.GetOrdinal("EXIF_CAMERA_MAKE"));
                            subjectPhoto.CameraModel = photoReader.GetString(photoReader.GetOrdinal("EXIF_CAMERA_MODEL"));
                            subjectPhoto.FilePrefix = photoReader.GetString(photoReader.GetOrdinal("FILE_PREFIX"));

                            photoList.Add(subjectPhoto);
                            recordcounter++;
                        }


                }
            }
            return photoList;
        }





        public void ExtractDuplicates(List<PhotoData> list, string FolderLocation)
        {
            //Take the list of duplicates and move them to a duplicates folder structure

        }

        public void RunSQLCommand(string CommandToRun)
        {
            using (DBConnection)
            {
                SqliteCommand InsertCommand = new SqliteCommand();

                    InsertCommand.Connection = DBConnection;
                    InsertCommand.CommandText = (CommandToRun);
                    Debug.Print(InsertCommand.CommandText);

                    try
                    {
                        DBConnection.Open();
                       
                        InsertCommand.ExecuteNonQuery();
                        DBConnection.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.Print($"Database Operatrion Failed for {InsertCommand.CommandText}" + Environment.NewLine + $"Reason: {e.Message}/{e.InnerException}");
                          throw;
                    }
            }
        }

        public void UpdateLowHangingFruit() 
        { 
            Console.WriteLine($"Updating the low hanging fruit");
            try
            {
                RunSQLCommand("UPDATE PhotoList SET [FILE_PREFIX] = substring([FILE_PATH],0,9)");
                RunSQLCommand("UPDATE PhotoList SET [PHOTO_STATUS] = 'ORIGINAL', [DUPLICATE_SCORE] = 0 where [FILE_PATH] in (SELECT DISTINCT [ORIGINAL_PHOTO] FROM vw_ALL_ORIGINALS)");
                RunSQLCommand("UPDATE PhotoList SET [PHOTO_STATUS] = 'DUPLICATE', [DUPLICATE_SCORE] = 4 Where [FILE_EXT] in('.CR2','.ARW','RW2','CR3') AND ([PHOTO_STATUS] is null or [DUPLICATE_SCORE] is null>0)");
                RunSQLCommand("UPDATE PhotoList SET[PHOTO_STATUS] = 'VERSION', [DUPLICATE_SCORE] = 1 Where INSTR([FILE_PATH],'Version')>0");
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
           
        }
    }
}
