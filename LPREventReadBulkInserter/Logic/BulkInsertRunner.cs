using LPREventReadBulkInserter.Data;
using LPREventReadBulkInserter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace LPREventReadBulkInserter.Logic
{
    public class BulkInsertRunner
    {
        public static void Run()
        {
            // Generate people for testing bulk insert
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("numberOfRecords"), out int numberToCreate))
            {
                Console.WriteLine($"Error: Unable to parse the numberOfRecords from config file. Exiting.");
                return;
            }
            List<Person> ofPeople = Utilities.GeneratePeople(numberToCreate);

            Stopwatch sw = new Stopwatch();

            // Perform individual inserts
            sw.Start();
            var totalInserts = ProcessData.DoMultipleInserts(ofPeople);
            sw.Stop();
            Console.WriteLine($"Individual inserts performed: {totalInserts}: Elapsed time = {sw.Elapsed} sec ({sw.Elapsed.TotalMilliseconds} ms)");

            // Perform Bulk Insert
            ofPeople = Utilities.GeneratePeople(numberToCreate);
            sw = new Stopwatch();
            sw.Start();
            var bulkResult = ProcessData.DoBulkInsert(ofPeople);
            sw.Stop();
            Console.WriteLine($"Bulk inserts performed: {bulkResult}: Elapsed time = {sw.Elapsed} sec ({sw.Elapsed.TotalMilliseconds} ms)");

            Console.ReadKey();
        }
    }
}
