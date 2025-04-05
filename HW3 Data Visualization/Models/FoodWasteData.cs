using CsvHelper.Configuration;

namespace HW3_Data_Visualization.Models
{
    public class FoodWasteData
    {
        public string Country { get; set; }
        public int Year { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Food Category")]
        public string FoodCategory { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Total Waste (Tons)")]
        public double TotalWaste { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Economic Loss (Million $)")]
        public double EconomicLoss { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Avg Waste per Capita (Kg)")]
        public double AvgWastePerCapita { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Population (Million)")]
        public double Population { get; set; }

        [CsvHelper.Configuration.Attributes.Name("Household Waste (%)")]
        public double HouseholdWastePercentage { get; set; }
    }
}
