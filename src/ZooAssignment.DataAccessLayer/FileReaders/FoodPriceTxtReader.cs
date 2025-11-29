using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.DataAccessLayer.FileReaders
{
    public class FoodPriceTxtReader : IFileReader<FoodPrice>
    {
        public List<FoodPrice> Read(string filePath)
        {
            var prices = new List<FoodPrice>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                var price = new FoodPrice
                {
                    FoodType = parts[0].Trim(),
                    Price = decimal.Parse(parts[1].Trim())
                };

                prices.Add(price);
            }

            return prices;
        }
    }
}
