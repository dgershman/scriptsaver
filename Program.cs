using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace ScriptSaver
{
    using System;

    /// <summary>
    /// Script Saver saves a bunch of scripts to text files.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Script a procedure to a text file
        /// </summary>
        /// <param name="args">The file containing the scripts.</param>
        public static void Main(string[] args)
        {
            var outputPath = ConfigurationManager.AppSettings["outputPath"];
            var connString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

            using (var connection = new SqlConnection(connString))
            {
                using (var sr = new StreamReader(args[0]))
                {
                    while (!sr.EndOfStream)
                    {
                        var scriptName = sr.ReadLine();
                        connection.Open();
                        var command = new SqlCommand("sp_helptext " + scriptName, connection);
                        var reader = command.ExecuteReader();
                        var sw = new StreamWriter(outputPath + @"\" + scriptName + ".sql");
                        while (reader.Read())
                        {                            
                            sw.WriteLine(reader.GetValue(0));
                        }

                        sw.Close();
                        connection.Close();                      
                    }

                    sr.Close();
                }                
            }
        }
    }
}
