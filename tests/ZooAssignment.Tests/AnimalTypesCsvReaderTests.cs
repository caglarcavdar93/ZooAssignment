using System.Text;
using ZooAssignment.DataAccessLayer.FileReaders;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.Tests.FileReaders
{
    public class AnimalTypesCsvReaderTests
    {
        private readonly AnimalTypesCsvReader _reader = new();
        private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), "ZooTests");

        public AnimalTypesCsvReaderTests()
        {
            if (!Directory.Exists(_testDirectory))
                Directory.CreateDirectory(_testDirectory);
        }

        private string CreateTestFile(string fileName, string content)
        {
            var filePath = Path.Combine(_testDirectory, fileName);
            File.WriteAllText(filePath, content, Encoding.UTF8);
            return filePath;
        }

        [Fact]
        public void Read_ValidCsvFile_ReturnsAnimalTypes()
        {
            // Arrange
            var csvContent = "Lion;10;Meat\nTiger;12;Meat\nZebra;8;Fruit";
            var filePath = CreateTestFile("valid_animals.csv", csvContent);

            // Act
            var result = _reader.Read(filePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Lion", result[0].TypeName);
            Assert.Equal(10m, result[0].FoodToWeightRatio);
            Assert.Equal("Meat", result[0].FoodType);
        }

        [Fact]
        public void Read_PercentageInMeatRatio_ParsesCorrectly()
        {
            // Arrange
            var csvContent = "Omnivore;10;Both;60%";
            var filePath = CreateTestFile("omnivore.csv", csvContent);

            // Act
            var result = _reader.Read(filePath);

            // Assert
            Assert.Single(result);
            Assert.Equal(0.6m, result[0].MeatToFoodRatio);
        }

        [Fact]
        public void Read_MissingMeatRatio_DefaultsToZero()
        {
            // Arrange
            var csvContent = "Carnivore;10;Meat";
            var filePath = CreateTestFile("carnivore.csv", csvContent);

            // Act
            var result = _reader.Read(filePath);

            // Assert
            Assert.Single(result);
            Assert.Equal(0m, result[0].MeatToFoodRatio);
        }

        [Fact]
        public void Read_ValuesWithWhitespace_TrimsCorrectly()
        {
            // Arrange
            var csvContent = "  Lion  ;  10  ;  Meat  ";
            var filePath = CreateTestFile("whitespace.csv", csvContent);

            // Act
            var result = _reader.Read(filePath);

            // Assert
            Assert.Single(result);
            Assert.Equal("Lion", result[0].TypeName);
            Assert.Equal("Meat", result[0].FoodType);
        }

        [Fact]
        public void Read_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDirectory, "nonexistent.csv");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _reader.Read(nonExistentPath));
        }

        [Fact]
        public void Read_EmptyLines_SkipsCorrectly()
        {
            // Arrange
            var csvContent = "Lion;10;Meat\n\n\nTiger;12;Meat\n";
            var filePath = CreateTestFile("empty_lines.csv", csvContent);

            // Act
            var result = _reader.Read(filePath);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Read_InvalidLineFormat_SkipsLine()
        {
            // Arrange
            var csvContent = "Lion;10;Meat\nInvalidLine\nTiger;12;Meat";
            var filePath = CreateTestFile("invalid_format.csv", csvContent);

            // Act
            var result = _reader.Read(filePath);

            // Assert
            Assert.Equal(2, result.Count); // Only valid lines
        }

        [Fact]
        public void Read_DecimalParsing_HandlesCorrectly()
        {
            // Arrange
            var csvContent = "Lion;10.5;Meat;50%";
            var filePath = CreateTestFile("decimal.csv", csvContent);

            // Act
            var result = _reader.Read(filePath);

            // Assert
            Assert.Single(result);
            Assert.Equal(10.5m, result[0].FoodToWeightRatio);
            Assert.Equal(0.5m, result[0].MeatToFoodRatio);
        }
    }
}

