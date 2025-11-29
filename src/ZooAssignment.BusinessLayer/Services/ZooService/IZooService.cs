using ZooAssignment.BusinessLayer.DTOs;

namespace ZooAssignment.BusinessLayer.Services
{
    public interface IZooService
    {
        Task<List<AnimalFeedingCostDto>> GetAllAnimalFeedingCostsAsync();
        Task<AnimalFeedingCostDto> GetAnimalFeedingCostByIdAsync(int animalId);
        Task<decimal> GetTotalMonthlyCostAsync();
        Task<List<AnimalDto>> GetAllAnimalsAsync();
        Task<List<FoodPriceDto>> GetAllPricesAsync();
    }
}
