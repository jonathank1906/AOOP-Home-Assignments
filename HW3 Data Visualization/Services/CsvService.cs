using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using HW3_Data_Visualization.Models;

namespace HW3_Data_Visualization.Services
{
    public class CsvService
    {
        public List<FoodWasteData> LoadData(string filePath)
        {
            try
            {
                // Combine the current directory path with the relative file path
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                using var reader = new StreamReader(fullPath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                return csv.GetRecords<FoodWasteData>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading CSV: {ex.Message}");
                return new List<FoodWasteData>();
            }
        }
    }
}
