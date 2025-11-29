using System;
using System.Collections.Generic;
using System.Text;

namespace ZooAssignment.DataAccessLayer.Models
{
    public class AnimalType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public decimal FoodToWeightRatio { get; set; }
        public string FoodType { get; set; }
        public decimal MeatToFoodRatio { get; set; }
        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
