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
    public class SQLite_Handler : IDisposable
    {
        public void Dispose() { }

        public SqliteConnection DBConnection { get; set; }

        public SQLite_Handler(string dbLocation = null, string SQLscript = null)
        {
            var cfg = AppConfig.Load();
            dbLocation ??= cfg.DbLocation;
            SQLscript ??= cfg.SqlScript;

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
            SqliteConnection dbConn = new SqliteConnection("Data Source=" + DBLocation);
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
            var sql = @"INSERT INTO PhotoList(
                ID, FILE_PATH, FILE_NAME, FILE_EXT, EXIF_DATE_CAPTURED, EXIF_CAMERA_MAKE, EXIF_CAMERA_MODEL,
                EXIF_FOCAL_LENGTH, EXIF_F_STOP, EXIF_EXPOSURE_TIME, EXIF_SOFTWARE, FILE_SIZE, EXIF_HEIGHT, EXIF_WIDTH,
                DATE_LAST_MODIFIED, PHOTO_IS_MONOCHROME, PHOTO_IS_RAW, FILE_PREFIX)
                VALUES (
                @ID, @FILE_PATH, @FILE_NAME, @FILE_EXT, @EXIF_DATE_CAPTURED, @EXIF_CAMERA_MAKE, @EXIF_CAMERA_MODEL,
                @EXIF_FOCAL_LENGTH, @EXIF_F_STOP, @EXIF_EXPOSURE_TIME, @EXIF_SOFTWARE, @FILE_SIZE, @EXIF_HEIGHT, @EXIF_WIDTH,
                @DATE_LAST_MODIFIED, @PHOTO_IS_MONOCHROME, @PHOTO_IS_RAW, @FILE_PREFIX)";

            var parameters = new Dictionary<string, object?>
            {
                ["@ID"] = photoData.ID,
                ["@FILE_PATH"] = photoData.FilePath,
                ["@FILE_NAME"] = photoData.FileName,
                ["@FILE_EXT"] = photoData.Extension,
                ["@EXIF_DATE_CAPTURED"] = photoData.DateCaptured,
                ["@EXIF_CAMERA_MAKE"] = photoData.CameraMake,
                ["@EXIF_CAMERA_MODEL"] = photoData.CameraModel,
                ["@EXIF_FOCAL_LENGTH"] = photoData.FocalLength,
                ["@EXIF_F_STOP"] = photoData.fStop,
                ["@EXIF_EXPOSURE_TIME"] = photoData.ShutterSpeed,
                ["@EXIF_SOFTWARE"] = photoData.Software,
                ["@FILE_SIZE"] = photoData.FileSize,
                ["@EXIF_HEIGHT"] = photoData.ImageHeight,
                ["@EXIF_WIDTH"] = photoData.ImageWidth,
                ["@DATE_LAST_MODIFIED"] = photoData.DateLastModified,
                ["@PHOTO_IS_MONOCHROME"] = photoData.isMonochrome,
                ["@PHOTO_IS_RAW"] = photoData.isRaw,
                ["@FILE_PREFIX"] = photoData.FilePrefix
            };

            RunSQLCommand(sql, parameters);
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
            Debug.Print(SQLcommand);
            List<PhotoData> photoList = new List<PhotoData>();

            using (SqliteCommand command = new SqliteCommand(SQLcommand, DBConnection))


            {
                Debug.Print(command.CommandText);
                DBConnection.Open();
                using (var photoDbReader = command.ExecuteReader())
                {
                    int recordcounter = 0;
                    while (photoDbReader.Read())
                    {
                        PhotoData subjectPhoto = new PhotoData();

                        subjectPhoto.recordnumber = recordcounter;
                        subjectPhoto.ID = photoDbReader.GetString(photoDbReader.GetOrdinal("ID"));
                        subjectPhoto.Extension = photoDbReader.GetString(photoDbReader.GetOrdinal("FILE_EXT"));
                        subjectPhoto.FileName = photoDbReader.GetString(photoDbReader.GetOrdinal("FILE_PATH"));
                        subjectPhoto.FilePath = photoDbReader.GetString(photoDbReader.GetOrdinal("FILE_NAME"));
                        subjectPhoto.DateCaptured = photoDbReader.GetString(photoDbReader.GetOrdinal("EXIF_DATE_CAPTURED"));
                        subjectPhoto.DateLastModified = photoDbReader.GetString(photoDbReader.GetOrdinal("DATE_LAST_MODIFIED"));
                        subjectPhoto.Software = photoDbReader.GetString(photoDbReader.GetOrdinal("EXIF_SOFTWARE"));
                        subjectPhoto.ImageWidth = photoDbReader.GetString(photoDbReader.GetOrdinal("EXIF_WIDTH"));
                        subjectPhoto.ImageHeight = photoDbReader.GetString(photoDbReader.GetOrdinal("EXIF_HEIGHT"));
                        subjectPhoto.CameraMake = photoDbReader.GetString(photoDbReader.GetOrdinal("EXIF_CAMERA_MAKE"));
                        subjectPhoto.CameraModel = photoDbReader.GetString(photoDbReader.GetOrdinal("EXIF_CAMERA_MODEL"));
                        subjectPhoto.FilePrefix = photoDbReader.GetString(photoDbReader.GetOrdinal("FILE_PREFIX"));
                        subjectPhoto.isMonochrome = Convert.ToBoolean(photoDbReader.GetString(photoDbReader.GetOrdinal("PHOTO_IS_MONOCHROME")));
                        subjectPhoto.isRaw = photoDbReader.GetBoolean(photoDbReader.GetOrdinal("PHOTO_IS_RAW"));
                        //subjectPhoto.RGBHash = photoDbReader.GetString(photoDbReader.GetOrdinal("PHOTO_RGB_HASH"));

                        photoList.Add(subjectPhoto);
                        recordcounter++;
                        subjectPhoto = null;
                        GC.Collect();
                    }


                }
            }
            GC.Collect();
            return photoList;
        }





        public void ExtractDuplicates(List<PhotoData> list, string FolderLocation)
        {
            //Take the list of duplicates and move them to a duplicates folder structure

        }

        // Keep the old single-argument RunSQLCommand for callers that didn't change
        public void RunSQLCommand(string CommandToRun)
        {
            RunSQLCommand(CommandToRun, null);
        }

        // New overload that accepts parameters
        public void RunSQLCommand(string CommandToRun, Dictionary<string, object>? parameters = null)
        {
            using (DBConnection)
            {
                using var cmd = new SqliteCommand(CommandToRun, DBConnection);
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                    }
                }
                try
                {
                    DBConnection.Open();
                    cmd.ExecuteNonQuery();
                    DBConnection.Close();
                }
                catch (Exception e)
                {
                    Debug.Print($"Database Operatrion Failed for {CommandToRun}" + Environment.NewLine + $"Reason: {e.Message}/{e.InnerException}");
                    throw;
                }
            }
        }

        public void UpdateLowHangingFruit()  //Pre processing corrections
        {
            Console.WriteLine($"Applying Pre Processing Corrections");
            try
            {
                RunSQLCommand("UPDATE PhotoList SET [FILE_PREFIX] = substring([FILE_PATH],0,9)");
                RunSQLCommand("UPDATE PhotoList SET [PHOTO_STATUS] = 'ORIGINAL',[DUPLICATE_SCORE] = 0 WHERE [PHOTO_IS_RAW] = 'True'");
                RunSQLCommand("UPDATE PhotoList SET [PHOTO_STATUS] = 'JPG ORIGINAL',[DUPLICATE_SCORE] = 0 WHERE [FILE_EXT] = '.JPG'");
                RunSQLCommand("UPDATE PhotoList SET [DUPLICATE_SCORE] = 0, [PHOTO_STATUS] = 'ORIGINAL' WHERE [FILE_EXT] in ('.JPG') and [PHOTO_STATUS] is null  and [FILE_PATH] in (Select [DESIGNA[...]");
                RunSQLCommand("UPDATE PhotoList SET [PHOTO_STATUS] = 'DUPLICATE', [DUPLICATE_SCORE] = 4 Where [FILE_EXT] in ('.jpg','.JPG') and [PHOTO_IS_RAW] = 'False' AND ([PHOTO_STATUS] is nul[...");
                RunSQLCommand("UPDATE PhotoList SET[PHOTO_STATUS] = 'VERSION', [DUPLICATE_SCORE] = 1 Where INSTR([FILE_PATH],'Version')>0");
                RunSQLCommand("UPDATE [PhotoList] SET[PHOTO_STATUS] = 'POSSIBLY CORRUPT' Where [FILE_EXT] = '.tiff' AND [EXIF_CAMERA_MAKE]= '' AND [EXIF_WIDTH] = '' AND [EXIF_HEIGHT] = '");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: e.ToString()");
                throw;
            }

        }
    }
}
