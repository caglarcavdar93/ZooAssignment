using Microsoft.EntityFrameworkCore;
using ZooAssignment.DataAccessLayer.Context;
using ZooAssignment.DataAccessLayer.FileReaders.Factory;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.DataAccessLayer.Migrations
{
    public class MigrationService : IMigrationService
    {
        private readonly ZooContext _context;
        private readonly IFileReaderFactory _fileReaderFactory;

        public MigrationService(ZooContext context, IFileReaderFactory fileReaderFactory)
        {
            _context = context;
            _fileReaderFactory = fileReaderFactory;
        }

        public async Task MigrateAsync(string animalCsvPath, string pricesTxtPath, string zooXmlPath)
        {
            var csvReader = _fileReaderFactory.GetFileReader<AnimalType>(FileType.Csv);
            var animalTypes = csvReader.Read(animalCsvPath);

            var xmlReader = _fileReaderFactory.GetFileReader<Animal>(FileType.Xml);
            var animalsFromXml = xmlReader.Read(zooXmlPath);

            foreach (var animal in animalsFromXml)
            {
                var type = animalTypes.FirstOrDefault(x => x.TypeName == animal.Type.TypeName);
                if (type != null)
                {
                    animal.Type = type;
                }
            }

            var priceReader = _fileReaderFactory.GetFileReader<FoodPrice>(FileType.Txt);
            var prices = priceReader.Read(pricesTxtPath);

            _context.AnimalTypes.AddRange(animalTypes);
            _context.Animals.AddRange(animalsFromXml);
            _context.FoodPrices.AddRange(prices);

            await _context.SaveChangesAsync();
        }
    }
}
