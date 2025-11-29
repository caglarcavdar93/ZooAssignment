using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZooAssignment.BusinessLayer.DTOs;
using ZooAssignment.DataAccessLayer.Context;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.BusinessLayer.Services
{
    public class ZooService : IZooService
    {
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
            var costs = new List<AnimalFeedingCostDto>();

            foreach (var animal in animals)
            {
                // For omnivores, we don't need a price lookup
                if (animal.Type.MeatToFoodRatio > 0)
                {
                    costs.Add(CalculateFeedingCost(animal, null));
                }
                else
                {
                    var price = prices.FirstOrDefault(p => p.FoodType.ToLower() == animal.Type.FoodType.ToLower());
                    if (price != null)
                    {
                        costs.Add(CalculateFeedingCost(animal, price));
                    }
                }
            }

            return costs;
        }

        public async Task<AnimalFeedingCostDto> GetAnimalFeedingCostByIdAsync(int animalId)
        {
            var animal = await _context.Animals.Include(a => a.Type).FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal == null)
                throw new KeyNotFoundException($"Animal with id {animalId} not found");

            // For omnivores, we don't need a single price lookup; prices will be determined in CalculateFeedingCost
            if (animal.Type.MeatToFoodRatio > 0)
            {
                return CalculateFeedingCost(animal, null);
            }

            var price = await _context.FoodPrices
                .FirstOrDefaultAsync(p => p.FoodType.ToLower() == animal.Type.FoodType.ToLower());

            if (price == null)
                throw new KeyNotFoundException($"Price for food type '{animal.Type.FoodType}' not found");

            return CalculateFeedingCost(animal, price);
        }

        public async Task<decimal> GetTotalMonthlyCostAsync()
        {
            var costs = await GetAllAnimalFeedingCostsAsync();
            return costs.Sum(c => c.MonthlyCost);
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

        private AnimalFeedingCostDto CalculateFeedingCost(Animal animal, FoodPrice price)
        {
            var dailyFoodAmount = animal.Weight * (decimal)animal.Type.FoodToWeightRatio;
            var dailyCost = decimal.Zero;
            var foodType = animal.Type.FoodType;

            // Check if omnivore (MeatToFoodRatio > 0)
            if (animal.Type.MeatToFoodRatio > 0)
            {
                // Omnivore: split between meat and fruit
                var meatRatio = (decimal)animal.Type.MeatToFoodRatio;
                var fruitRatio = 1 - meatRatio;

                var meatAmount = dailyFoodAmount * meatRatio;
                var fruitAmount = dailyFoodAmount * fruitRatio;

                // Get prices for both meat and fruit
                var meatPrice = _context.FoodPrices.FirstOrDefault(p => p.FoodType.ToLower() == "meat");
                var fruitPrice = _context.FoodPrices.FirstOrDefault(p => p.FoodType.ToLower() == "fruit");

                if (meatPrice != null)
                    dailyCost += meatAmount * meatPrice.Price;

                if (fruitPrice != null)
                    dailyCost += fruitAmount * fruitPrice.Price;

                foodType = "Meat and Fruit";
            }
            else
            {
                // Carnivore or Herbivore: use single food type
                dailyCost = dailyFoodAmount * price.Price;
            }

            var monthlyCost = dailyCost * 30;

            return new AnimalFeedingCostDto
            {
                AnimalId = animal.Id,
                AnimalName = animal.Name,
                Species = animal.Type.TypeName,
                Weight = animal.Weight,
                FoodType = foodType,
                DailyFoodAmount = dailyFoodAmount,
                FoodPricePerKg = price?.Price ?? 0m,
                DailyCost = dailyCost,
                MonthlyCost = monthlyCost
            };
        }
    }
}
