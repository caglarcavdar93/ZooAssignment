using System.Xml.Linq;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.DataAccessLayer.FileReaders
{
    public class ZooXmlReader : IFileReader<Animal>
    {
        public List<Animal> Read(string filePath)
        {
            var animals = new List<Animal>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            if (root == null)
                return animals;

            // Get all species groups
            var speciesGroups = root.Elements();

            foreach (var group in speciesGroups)
            {
                // Get all animals of this species
                var animalElements = group.Elements();

                foreach (var animalElement in animalElements)
                {
                    var nameAttr = animalElement.Attribute("name");
                    var kgAttr = animalElement.Attribute("kg");
                    var speciesName = animalElement.Name.LocalName;
                    if (nameAttr != null && kgAttr != null)
                    {
                        var animal = new Animal
                        {
                            Name = nameAttr.Value,
                            Weight = decimal.Parse(kgAttr.Value),
                            Type = new AnimalType { TypeName = speciesName }
                        };

                        animals.Add(animal);
                    }
                }
            }

            return animals;
        }
    }
}
