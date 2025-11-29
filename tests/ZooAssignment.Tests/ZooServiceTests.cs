using Microsoft.EntityFrameworkCore;
using ZooAssignment.BusinessLayer.Services;
using ZooAssignment.DataAccessLayer.Context;
using ZooAssignment.DataAccessLayer.Models;
using AutoMapper;
using ZooAssignment.BusinessLayer.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ZooAssignment.Tests.BusinessLayer
{
    public class ZooServiceTests
    {
        private IMapper CreateMapper()
        {
            var services = new ServiceCollection();
            services.AddLogging(config => config.AddConsole());
            services.AddAutoMapper(mc => mc.AddProfile<ZooMappingProfile>());
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IMapper>();
        }

        [Fact]
        public async Task CalculateDailyFood_Carnivore_CalculatesCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var lion = new AnimalType
            {
                Id = 1,
                TypeName = "Lion",
                FoodToWeightRatio = 10m,
                FoodType = "Meat",
                MeatToFoodRatio = 0m
            };

            var animal = new Animal
            {
                Id = 1,
                Name = "Simba",
                AnimalTypeId = 1,
                Weight = 100m,
                Type = lion
            };

            var meatPrice = new FoodPrice
            {
                Id = 1,
                FoodType = "Meat",
                Price = 50m
            };

            context.AnimalTypes.Add(lion);
            context.Animals.Add(animal);
            context.FoodPrices.Add(meatPrice);
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            var expectedDailyFood = 100m * 10m;
            Assert.Equal(expectedDailyFood, cost.DailyFoodAmount);
        }

        [Fact]
        public async Task CalculateDailyCost_Carnivore_UsesCorrectPrice()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var lion = new AnimalType
            {
                Id = 1,
                TypeName = "Lion",
                FoodToWeightRatio = 10m,
                FoodType = "Meat",
                MeatToFoodRatio = 0m
            };

            var animal = new Animal
            {
                Id = 1,
                Name = "Simba",
                AnimalTypeId = 1,
                Weight = 100m,
                Type = lion
            };

            var meatPrice = new FoodPrice
            {
                Id = 1,
                FoodType = "Meat",
                Price = 50m
            };

            context.AnimalTypes.Add(lion);
            context.Animals.Add(animal);
            context.FoodPrices.Add(meatPrice);
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            var expectedDailyFood = 100m * 10m;
            var expectedCost = expectedDailyFood * 50m;
            Assert.Equal(expectedCost, cost.DailyCost);
        }

        [Fact]
        public async Task CalculateMonthlyCost_Carnivore_Multiplies30Days()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var lion = new AnimalType
            {
                Id = 1,
                TypeName = "Lion",
                FoodToWeightRatio = 10m,
                FoodType = "Meat",
                MeatToFoodRatio = 0m
            };

            var animal = new Animal
            {
                Id = 1,
                Name = "Simba",
                AnimalTypeId = 1,
                Weight = 100m,
                Type = lion
            };

            var meatPrice = new FoodPrice
            {
                Id = 1,
                FoodType = "Meat",
                Price = 50m
            };

            context.AnimalTypes.Add(lion);
            context.Animals.Add(animal);
            context.FoodPrices.Add(meatPrice);
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            var expectedMonthlyCost = cost.DailyCost * 30;
            Assert.Equal(expectedMonthlyCost, cost.MonthlyCost);
        }

        [Fact]
        public async Task CalculateDailyFood_Omnivore_CalculatesCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var bear = new AnimalType
            {
                Id = 1,
                TypeName = "Bear",
                FoodToWeightRatio = 12m,
                FoodType = "Both",
                MeatToFoodRatio = 0.6m
            };

            var animal = new Animal
            {
                Id = 1,
                Name = "Bruno",
                AnimalTypeId = 1,
                Weight = 200m,
                Type = bear
            };

            context.AnimalTypes.Add(bear);
            context.Animals.Add(animal);
            context.FoodPrices.Add(new FoodPrice { Id = 1, FoodType = "Meat", Price = 50m });
            context.FoodPrices.Add(new FoodPrice { Id = 2, FoodType = "Fruit", Price = 30m });
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            var expectedDailyFood = 200m * 12m;
            Assert.Equal(expectedDailyFood, cost.DailyFoodAmount);
        }

        [Fact]
        public async Task CalculateDailyCost_Omnivore_SplitsBetweenMeatAndFruit()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var bear = new AnimalType
            {
                Id = 1,
                TypeName = "Bear",
                FoodToWeightRatio = 12m,
                FoodType = "Both",
                MeatToFoodRatio = 0.6m
            };

            var animal = new Animal
            {
                Id = 1,
                Name = "Bruno",
                AnimalTypeId = 1,
                Weight = 200m,
                Type = bear
            };

            context.AnimalTypes.Add(bear);
            context.Animals.Add(animal);
            context.FoodPrices.Add(new FoodPrice { Id = 1, FoodType = "Meat", Price = 50m });
            context.FoodPrices.Add(new FoodPrice { Id = 2, FoodType = "Fruit", Price = 30m });
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            var dailyFood = 200m * 12m;
            var meatAmount = dailyFood * 0.6m;
            var fruitAmount = dailyFood * 0.4m;
            var expectedCost = (meatAmount * 50m) + (fruitAmount * 30m);
            
            Assert.Equal(expectedCost, cost.DailyCost);
        }

        [Fact]
        public async Task GetAllAnimalFeedingCostsAsync_ReturnsAllCosts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var lion = new AnimalType { Id = 1, TypeName = "Lion", FoodToWeightRatio = 10m, FoodType = "Meat", MeatToFoodRatio = 0m };
            var zebra = new AnimalType { Id = 2, TypeName = "Zebra", FoodToWeightRatio = 8m, FoodType = "Fruit", MeatToFoodRatio = 0m };
            var bear = new AnimalType { Id = 3, TypeName = "Bear", FoodToWeightRatio = 12m, FoodType = "Both", MeatToFoodRatio = 0.6m };

            context.AnimalTypes.AddRange(lion, zebra, bear);
            context.Animals.AddRange(
                new Animal { Id = 1, Name = "Simba", AnimalTypeId = 1, Weight = 100m, Type = lion },
                new Animal { Id = 2, Name = "Stripes", AnimalTypeId = 2, Weight = 150m, Type = zebra },
                new Animal { Id = 3, Name = "Bruno", AnimalTypeId = 3, Weight = 200m, Type = bear }
            );
            context.FoodPrices.AddRange(
                new FoodPrice { Id = 1, FoodType = "Meat", Price = 50m },
                new FoodPrice { Id = 2, FoodType = "Fruit", Price = 30m }
            );
            context.SaveChanges();

            // Act
            var costs = await service.GetAllAnimalFeedingCostsAsync();

            // Assert
            Assert.Equal(3, costs.Count);
            Assert.NotNull(costs.FirstOrDefault(c => c.AnimalId == 1));
            Assert.NotNull(costs.FirstOrDefault(c => c.AnimalId == 2));
            Assert.NotNull(costs.FirstOrDefault(c => c.AnimalId == 3));
        }

        [Fact]
        public async Task GetTotalMonthlyCostAsync_SumsAllCosts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var lion = new AnimalType { Id = 1, TypeName = "Lion", FoodToWeightRatio = 10m, FoodType = "Meat", MeatToFoodRatio = 0m };
            context.AnimalTypes.Add(lion);
            context.Animals.Add(new Animal { Id = 1, Name = "Simba", AnimalTypeId = 1, Weight = 100m, Type = lion });
            context.FoodPrices.Add(new FoodPrice { Id = 1, FoodType = "Meat", Price = 50m });
            context.SaveChanges();

            // Act
            var total = await service.GetTotalDailyCostAsync();
            var individualCosts = await service.GetAllAnimalFeedingCostsAsync();
            var expectedTotal = individualCosts.Sum(c => c.DailyCost);

            // Assert
            Assert.Equal(expectedTotal, total);
        }

        [Fact]
        public async Task GetAnimalFeedingCostByIdAsync_Omnivore_SetsFoodTypeToBoth()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var bear = new AnimalType { Id = 1, TypeName = "Bear", FoodToWeightRatio = 12m, FoodType = "Both", MeatToFoodRatio = 0.6m };
            context.AnimalTypes.Add(bear);
            context.Animals.Add(new Animal { Id = 1, Name = "Bruno", AnimalTypeId = 1, Weight = 200m, Type = bear });
            context.FoodPrices.Add(new FoodPrice { Id = 1, FoodType = "Meat", Price = 50m });
            context.FoodPrices.Add(new FoodPrice { Id = 2, FoodType = "Fruit", Price = 30m });
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            Assert.Equal("Meat and Fruit", cost.FoodType);
        }

        [Fact]
        public async Task CalculateDailyCost_WithZeroWeight_ReturnsZero()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var animalType = new AnimalType { Id = 1, TypeName = "TestAnimal", FoodToWeightRatio = 10m, FoodType = "Meat", MeatToFoodRatio = 0m };
            context.AnimalTypes.Add(animalType);
            context.Animals.Add(new Animal { Id = 1, Name = "ZeroWeight", AnimalTypeId = 1, Weight = 0m, Type = animalType });
            context.FoodPrices.Add(new FoodPrice { Id = 1, FoodType = "Meat", Price = 50m });
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            Assert.Equal(0m, cost.DailyCost);
            Assert.Equal(0m, cost.MonthlyCost);
        }

        [Fact]
        public async Task OmnivoreWith100PercentMeat_UsesOnlyMeatPrice()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ZooContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new ZooContext(options);
            
            var mapper = CreateMapper();
            var service = new ZooService(context, mapper);

            var animalType = new AnimalType { Id = 1, TypeName = "PureMeatEater", FoodToWeightRatio = 10m, FoodType = "Both", MeatToFoodRatio = 1m };
            context.AnimalTypes.Add(animalType);
            context.Animals.Add(new Animal { Id = 1, Name = "TestAnimal", AnimalTypeId = 1, Weight = 100m, Type = animalType });
            context.FoodPrices.Add(new FoodPrice { Id = 1, FoodType = "Meat", Price = 50m });
            context.FoodPrices.Add(new FoodPrice { Id = 2, FoodType = "Fruit", Price = 30m });
            context.SaveChanges();

            // Act
            var cost = await service.GetAnimalFeedingCostByIdAsync(1);

            // Assert
            var dailyFood = 100m * 10m;
            var expectedCost = dailyFood * 50m;
            Assert.Equal(expectedCost, cost.DailyCost);
        }
    }
}

