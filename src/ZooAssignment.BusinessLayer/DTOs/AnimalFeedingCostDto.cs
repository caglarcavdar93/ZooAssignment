namespace ZooAssignment.BusinessLayer.DTOs
{
    public class AnimalFeedingCostDto
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; }
        public string Species { get; set; }
        public decimal Weight { get; set; }
        public string FoodType { get; set; }
        public decimal DailyFoodAmount { get; set; }
        public decimal FoodPricePerKg { get; set; }
        public decimal DailyCost { get; set; }
        public decimal MonthlyCost { get; set; }
    }
}
