using ZooAssignment.BusinessLayer.DTOs;

namespace ZooAssignment.BusinessLayer.Services
{
    public interface IZooService
    {
        Task<List<AnimalFeedingCostDto>> GetAllAnimalFeedingCostsAsync();
        Task<AnimalFeedingCostDto> GetAnimalFeedingCostByIdAsync(int animalId);
        Task<decimal> GetTotalDailyCostAsync();
        Task<List<AnimalDto>> GetAllAnimalsAsync();
        Task<List<FoodPriceDto>> GetAllPricesAsync();
    }
}
