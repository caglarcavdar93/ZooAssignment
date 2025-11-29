namespace ZooAssignment.DataAccessLayer.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AnimalTypeId { get; set; }
        public AnimalType Type { get; set; }
        public decimal Weight { get; set; }
    }
}
