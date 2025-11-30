using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.DataAccessLayer.FileReaders
{
    public class AnimalTypesCsvReader : IFileReader<AnimalType>
    {
        public List<AnimalType> Read(string filePath)
        {
            var animals = new List<AnimalType>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');
                if (parts.Length < 3)
                    continue;

                var meatToFoodRatio = 0m;
                if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3].Trim()))
                {
                    meatToFoodRatio = decimal.Parse(parts[3].Trim().TrimEnd('%'), System.Globalization.CultureInfo.InvariantCulture) / 100m;
                }

                var animal = new AnimalType
                {
                    TypeName = parts[0].Trim(),
                    FoodToWeightRatio = decimal.Parse(parts[1].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    FoodType = parts[2].Trim(),
                    MeatToFoodRatio = meatToFoodRatio
                };

                animals.Add(animal);
            }

            return animals;
        }
    }
}
