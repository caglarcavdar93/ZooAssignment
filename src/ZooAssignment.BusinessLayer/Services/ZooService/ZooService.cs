using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZooAssignment.BusinessLayer.DTOs;
using ZooAssignment.DataAccessLayer.Context;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.BusinessLayer.Services
{
    public class ZooService : IZooService
    {
        private record CostCalculationParams(Animal animal, decimal fruitPrice, decimal meatPrice);
        private readonly ZooContext _context;
        private readonly IMapper _mapper;

        public ZooService(ZooContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AnimalFeedingCostDto>> GetAllAnimalFeedingCostsAsync()
        {
            var animals = await _context.Animals.Include(a => a.Type).ToListAsync();
            var prices = await _context.FoodPrices.ToListAsync();
            var meatPrice = prices.FirstOrDefault(p => p.FoodType.ToLower() == "meat")?.Price;
            var fruitPrice = prices.FirstOrDefault(p => p.FoodType.ToLower() == "fruit")?.Price;
            var costs = new List<AnimalFeedingCostDto>();

            foreach (var animal in animals)
            {
                // For omnivores, we don't need a price lookup
                var costParams = new CostCalculationParams(
                    animal,
                    fruitPrice ?? 0m,
                    meatPrice ?? 0m
                );
                costs.Add(CalculateFeedingCost(costParams));
            }

            return costs;
        }

        public async Task<AnimalFeedingCostDto> GetAnimalFeedingCostByIdAsync(int animalId)
        {
            var animal = await _context.Animals.Include(a => a.Type).FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal == null)
                throw new KeyNotFoundException($"Animal with id {animalId} not found");

            var prices = await _context.FoodPrices.ToListAsync();
            var meatPrice = prices.FirstOrDefault(p => p.FoodType.ToLower() == "meat")?.Price;
            var fruitPrice = prices.FirstOrDefault(p => p.FoodType.ToLower() == "fruit")?.Price;
            var costParams = new CostCalculationParams(
                    animal,
                    fruitPrice ?? 0m,
                    meatPrice ?? 0m
                );
            // For omnivores, we don't need a single price lookup; prices will be determined in CalculateFeedingCost
            if (animal.Type.MeatToFoodRatio > 0)
            {
                return CalculateFeedingCost(costParams);
            }

            return CalculateFeedingCost(costParams);
        }

        public async Task<decimal> GetTotalDailyCostAsync()
        {
            var costs = await GetAllAnimalFeedingCostsAsync();
            return costs.Sum(c => c.DailyCost);
        }

        public async Task<List<AnimalDto>> GetAllAnimalsAsync()
        {
            var animals = await _context.Animals.ToListAsync();
            return _mapper.Map<List<AnimalDto>>(animals);
        }

        public async Task<List<FoodPriceDto>> GetAllPricesAsync()
        {
            var prices = await _context.FoodPrices.ToListAsync();
            return _mapper.Map<List<FoodPriceDto>>(prices);
        }

        private AnimalFeedingCostDto CalculateFeedingCost(CostCalculationParams costParams)
        {
            var animal = costParams.animal;
            var dailyFoodAmount = animal.Weight * (decimal)animal.Type.FoodToWeightRatio;
            var dailyCost = decimal.Zero;
            var foodType = costParams.animal.Type.FoodType;

            // Check if omnivore (MeatToFoodRatio > 0)
            if (costParams.animal.Type.MeatToFoodRatio > 0)
            {
                // Omnivore: split between meat and fruit
                var meatRatio = (decimal)costParams.animal.Type.MeatToFoodRatio;
                var fruitRatio = 1 - meatRatio;

                var meatAmount = dailyFoodAmount * meatRatio;
                var fruitAmount = dailyFoodAmount * fruitRatio;

                if (costParams.meatPrice > 0)
                    dailyCost += meatAmount * costParams.meatPrice;

                if (costParams.fruitPrice > 0)
                    dailyCost += fruitAmount * costParams.fruitPrice;
            }
            else if (foodType.ToLower() == "meat")
            {
                // Carnivore: use meat price
                dailyCost = dailyFoodAmount * costParams.meatPrice;
            }
            else if (foodType.ToLower() == "fruit")
            {
                // Carnivore or Herbivore: use single food type
                dailyCost = dailyFoodAmount * costParams.fruitPrice;
            }

            return new AnimalFeedingCostDto
            {
                AnimalId = animal.Id,
                AnimalName = animal.Name,
                Species = animal.Type.TypeName,
                Weight = animal.Weight,
                FoodType = foodType,
                DailyFoodAmount = dailyFoodAmount,
                DailyCost = dailyCost,
                MonthlyCost = dailyCost * 30
            };
        }
    }
}
