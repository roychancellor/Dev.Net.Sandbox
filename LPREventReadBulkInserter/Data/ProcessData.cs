using LPREventReadBulkInserter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LPREventReadBulkInserter.Data
{
    public class ProcessData
    {
        public static int DoMultipleInserts(List<Person> people)
        {
            int totalInserts = 0;

            var connString = GetConnectionString();

            using (var conn = new SqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar, 50);
                cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 50);
                cmd.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime);
                cmd.Parameters.Add("@FavoriteMovie", System.Data.SqlDbType.NVarChar, 50);
                cmd.Parameters.Add("@FavoriteNumber", System.Data.SqlDbType.Int);
                cmd.CommandText = @"INSERT INTO [dbo].[People]
					   ([Id]
					   ,[Name]
					   ,[BirthDate]
					   ,[FavoriteMovie]
					   ,[FavoriteNumber])
				 VALUES
					   (@Id
					   ,@Name
					   ,@BirthDate
					   ,@FavoriteMovie
					   ,@FavoriteNumber)";

                try
                {
                    conn.Open();
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: Unable to open a connection to the database. Exiting.");
                    return 0;
                }

                foreach (var person in people)
                {
                    /*string favMovie = person.FavoriteMovie;
                    if (string.IsNullOrEmpty(person.FavoriteMovie))
                    {
                        favMovie = DBNull.Value.ToString();
                    }
                    object favNum = person.FavoriteNumber;
                    if (person.FavoriteNumber == null)
                    {
                        favNum = DBNull.Value;
                    }*/

                    cmd.Parameters["@Id"].Value = person.Id.ToString();
                    cmd.Parameters["@Name"].Value = person.Name;
                    cmd.Parameters["@BirthDate"].Value = GetDBValue(person.BirthDate); // ?? System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                    cmd.Parameters["@FavoriteMovie"].Value = GetDBValue(person.FavoriteMovie); //favMovie;
                    cmd.Parameters["@FavoriteNumber"].Value = GetDBValue(person.FavoriteNumber); //favNum;

                    int result = 0;
                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                        {
                            Console.WriteLine($"Error, the SQL Connection is closed. Exiting.");
                            return 0;
                        }
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: The ExecuteNonQuery call threw and exception. Exiting.\n{ex}");
                        return 0;
                    }

                    totalInserts += result;

                    if (result < 1)
                    {
                        Console.WriteLine($"Single Insert failed");
                    }
                }
            }
            return totalInserts;
        }

        public static int DoBulkInsert(List<Person> people)
        {
            DataTable dt = new DataTable
            {
                TableName = ConfigurationManager.AppSettings.Get("tableName")
            };

            dt.Columns.Add(nameof(Person.Id), typeof(string));
            dt.Columns.Add(nameof(Person.Name), typeof(string));
            dt.Columns.Add(nameof(Person.BirthDate), typeof(DateTime));
            dt.Columns.Add(nameof(Person.FavoriteMovie), typeof(string));
            dt.Columns.Add(nameof(Person.FavoriteNumber), typeof(int));

            foreach (var person in people)
            {
                var row = dt.NewRow();
                row[nameof(Person.Id)] = person.Id.ToString();
                row[nameof(Person.Name)] = person.Name;
                row[nameof(Person.BirthDate)] = GetDBValue(person.BirthDate);
                row[nameof(Person.FavoriteMovie)] = GetDBValue(person.FavoriteMovie);
                row[nameof(Person.FavoriteNumber)] = GetDBValue(person.FavoriteNumber);

                dt.Rows.Add(row);
            }

            using (var bulkInsert = new SqlBulkCopy(GetConnectionString()))
            {
                try
                {
                    bulkInsert.DestinationTableName = dt.TableName;
                    bulkInsert.WriteToServer(dt);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: WriteToServer threw. Exiting.\n{ex}");
                    return 0;
                }
            }
            return 1;
        }

        public static object GetDBValue(object o)
        {
            return o ?? DBNull.Value;
        }

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["People"].ConnectionString;
        }
    }
}
