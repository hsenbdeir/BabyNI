using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

public class DataParserService
{
    private readonly string outputDirectory = @"C:\Users\User\Desktop\IMS\Output";
    private readonly string archiveDirectory = @"C:\Users\User\Desktop\IMS\Parsed";
    private readonly HashSet<string> headersToSkip = new HashSet<string> { "Position", "IdLogNum", "MeanRxLevel1m", "NodeName" };
    private readonly Dictionary<string, string> fieldsToCheck = new Dictionary<string, string>
        {
             { "FailureDescription", "-" },
            { "FarEndTID","----" },
            {"Object","Unreachable Bulk FC" },
        };

    public void ProcessFile(string inputFilePath)
    {
        var (header, records) = ParseCsvFile(inputFilePath, headersToSkip, fieldsToCheck);
        if (records.Count > 0)
        {
            WriteToCsvFile(outputDirectory, header, records);
            ArchiveFile(inputFilePath);
        }
        else
        {
            Console.WriteLine("No Valid Records Found");
        }
    }

    public (List<string> header, List<Dictionary<string, string>> records) ParseCsvFile(string filePath, HashSet<string> headersToSkip, Dictionary<string, string> fieldsToCheck)
    {
        var header = new List<string>();
        var records = new List<Dictionary<string, string>>();

        try
        {
            using (var reader = new StreamReader(filePath))
            {
                var isFirstRow = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (isFirstRow)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            if (headersToSkip.Contains(values[i].Trim()))
                            {
                                continue; // Skip this header
                            }
                            header.Add(values[i].Trim());
                        }
                        isFirstRow = false;
                    }
                    else
                    {
                        var record = new Dictionary<string, string>();
                        bool shouldSkipRecord = false;

                        for (var i = 0; i < values.Length; i++)
                        {
                            if (i < header.Count)
                            {
                                if (headersToSkip.Contains(header[i]))
                                {
                                    continue; // Skip this value
                                }
                                record[header[i]] = values[i].Trim();
                            }
                        }

                        // Add the new record values to the record.
                        foreach (var (fieldToCheck, valueToFilter) in fieldsToCheck)
                        {
                            if (record.TryGetValue(fieldToCheck, out var fieldValue) && fieldValue == valueToFilter)
                            {
                                shouldSkipRecord = true;
                                break; // Skip this record
                            }
                        }

                        if (shouldSkipRecord)
                        {
                            continue;
                        }

                        records.Add(record);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing the file: {ex.Message}");
        }

        if (header.Count == 0)
        {
            Console.WriteLine("No Valid Records Found");
        }

        // Remove unneeded headers and columns here
        // ...

        return (header, records);
    }


    /* public (List<string> header, List<Dictionary<string, string>> records) ParseCsvFile(string filePath, HashSet<string> headersToSkip, Dictionary<string, string> fieldsToCheck)
     {
         var header = new List<string>();
         var records = new List<Dictionary<string, string>>();

         try
         {
             using (var reader = new StreamReader(filePath))
             {
                 var isFirstRow = true;

                 while (!reader.EndOfStream)
                 {
                     var line = reader.ReadLine();
                     var values = line.Split(',');

                     if (isFirstRow)
                     {
                         header = values
                             .Select(field => field.Trim())
                             .Where(field => !headersToSkip.Contains(field))
                             .ToList();
                         isFirstRow = false;
                     }
                     else
                     {
                         var record = new Dictionary<string, string>();
                         for (var i = 0; i < header.Count; i++)
                         {
                             if (i < values.Length)
                             {
                                 record[header[i]] = values[i].Trim();
                             }
                             else
                             {
                                 record[header[i]] = null; // Handle null values
                             }
                         }

                         bool shouldSkipRecord = false;



                         // Add the new record values to the record.
                         foreach (var (fieldToCheck, valueToFilter) in fieldsToCheck)
                         {
                             if (record.TryGetValue(fieldToCheck, out var fieldValue) && fieldValue == valueToFilter)
                             {
                                 shouldSkipRecord = true;
                                 break; // Skip this record
                             }
                         }

                         if (shouldSkipRecord)
                         {
                             continue;
                         }

                         records.Add(record);
                     }
                 }
             }

         }
         catch (Exception ex)
         {
             Console.WriteLine($"Error parsing the file: {ex.Message}");
         }

         return (header, records);
     } */

    public void WriteToCsvFile(string outputDirectory, List<string> header, List<Dictionary<string, string>> records)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string outputFileName = $"output_{timestamp}.csv";
        string filePath = Path.Combine(outputDirectory, outputFileName);

        foreach (var fieldName in header)
        {
            if (!fieldsToCheck.ContainsKey(fieldName))
            {
                fieldsToCheck.Add(fieldName, null);
            }
        }


        try
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                // Write the header
                foreach (var fieldName in header)
                {
                    csv.WriteField(fieldName);
                }
                csv.NextRecord();

                // Write the records
                foreach (var record in records)
                {
                    foreach (var fieldName in header)
                    {
                        csv.WriteField(record[fieldName]);
                    }
                    csv.NextRecord();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to the output file: {ex.Message}");
        }
    }


    /*   public (List<string> header, List<Dictionary<string, string>> records) ParseCsvFile(string filePath, HashSet<string> headersToSkip, Dictionary<string, string> fieldsToCheck)
       {
           var header = new List<string>();
           var records = new List<Dictionary<string, string>>();

           try
           {
               using (var reader = new StreamReader(filePath))
               {
                   string line;

                   // Process the header line
                   if ((line = reader.ReadLine()) != null)
                   {
                       var headerValues = line.Split(',');

                       foreach (var headerValue in headerValues)
                       {
                           var headerField = headerValue.Trim();
                           if (!headersToSkip.Contains(headerField))
                           {
                               header.Add(headerField);
                           }
                       }
                   }

                   while ((line = reader.ReadLine()) != null)
                   {
                       var values = line.Split(',');

                       if (values.Length != header.Count)
                       {
                           Console.WriteLine($"Skipping row due to mismatch between header and values: {line}");
                           continue; // Skip this row
                       }

                       var record = new Dictionary<string, string>();

                       for (var i = 0; i < header.Count; i++)
                       {
                           record[header[i]] = values[i].Trim();
                       }

                       bool shouldSkipRecord = false;

                       // Add the new record values to the record.
                       foreach (var (fieldToCheck, valueToFilter) in fieldsToCheck)
                       {
                           if (record.TryGetValue(fieldToCheck, out var fieldValue) && fieldValue == valueToFilter)
                           {
                               shouldSkipRecord = true;
                               break; // Skip this record
                           }
                       }

                       if (!shouldSkipRecord)
                       {
                           records.Add(record);
                       }
                   }
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Error parsing the file: {ex.Message}");
           }

           return (header, records);
       }
   */
    /* public void WriteToCsvFile(string outputDirectory, List<string> header, List<Dictionary<string, string>> records)
     {
         string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
         string outputFileName = $"output_{timestamp}.csv";
         string filePath = Path.Combine(outputDirectory, outputFileName);

         foreach (var fieldName in header)
         {
             if (!fieldsToCheck.ContainsKey(fieldName))
             {
                 fieldsToCheck.Add(fieldName, null);
             }
         }


         try
         {
             using (var writer = new StreamWriter(filePath))
             using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
             {
                 // Write the header
                 foreach (var fieldName in header)
                 {
                     csv.WriteField(fieldName);
                 }
                 csv.NextRecord();

                 // Write the records
                 foreach (var record in records)
                 {
                     foreach (var fieldName in header)
                     {
                         csv.WriteField(record[fieldName]);
                     }
                     csv.NextRecord();
                 }
             }
         }
         catch (Exception ex)
         {
             Console.WriteLine($"Error writing to the output file: {ex.Message}");
         }
     }*/
    private void ArchiveFile(string filePath)
    {
        // Construct the destination path in the archive directory
        string fileName = Path.GetFileName(filePath);
        string archiveFilePath = Path.Combine(archiveDirectory, fileName);

        try
        {
            // Move the file to the archive directory
            File.Move(filePath, archiveFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error archiving the file: {ex.Message}");
        }
    }
    private static string GetLink(string input)
    {
        string[] values = input.Split('/');

        string slot;
        string port;

        if (values.Length >= 4)
        {
            slot = values[1];
            port = values[2];
        }
        else
        {
            slot = values[0];
            port = "1";
        }

        return $"{slot}/{port}";
    }
    private static string GetTid(string input)
    {
        int startIndex = input.IndexOf('_', input.IndexOf('_') + 1) + 1;
        int endIndex = input.LastIndexOf('_') - 1;

        if (startIndex >= 0 && endIndex >= 0 && startIndex <= endIndex)
        {
            return input.Substring(startIndex, endIndex - startIndex + 1);
        }
        else
        {
            // Handle the case where the input doesn't meet the expected format
            return "N/A"; // or any other appropriate default value
        }
    }

    private static string GetFarEndTid(string input)
    {
        int startIndex = input.LastIndexOf('_') + 1;

        return input.Substring(startIndex);
    }
    private static IList<string> GetSlots(string input)
    {
        string[] values = input.Split('/');

        IList<string> slots = new List<string>();

        if (values.Length >= 4)
        {
            slots.Add(values[1]);

            if (values.Length == 5)
            {
                slots.Add(values[2]);
            }
        }
        else
        {
            slots.Add(values[0]);
        }

        return slots;
    }
    private static string GetPort(string input)
    {
        string[] values = input.Split('/');

        string port;

        if (values.Length >= 4)
        {
            port = values[2];
        }
        else
        {
            port = "1";
        }

        return port;
    }
    private static Dictionary<string, string> GenerateNewRecordValues(Dictionary<string, string> record, IEnumerable<string> newHeaderFields)
    {
        var newRecordValues = new Dictionary<string, string>();

        foreach (var newHeaderField in newHeaderFields)
        {
            string newValue;
            if (!record.ContainsKey(newHeaderField))
            {
                record[newHeaderField] = null;
            }
            switch (newHeaderField)
            {
                case "LINK":
                    newValue = GetLink(record["Object"]);
                    break;
                case "TID":
                    newValue = GetTid(record["Object"]);
                    break;
                case "FARENDTID":
                    newValue = GetFarEndTid(record["Object"]);
                    break;
                case "SLOT":
                    newValue = GetSlots(record["Object"]).First();
                    break;
                case "PORT":
                    newValue = GetPort(record["Object"]);
                    break;
                default:
                    newValue = null;
                    break;
            }

            newRecordValues.Add(newHeaderField, newValue);
        }

        // Add the new header values to the record.
        foreach (var newHeaderField in newHeaderFields)
        {
            record[newHeaderField] = newRecordValues[newHeaderField];
        }

        return record;
    }
}
