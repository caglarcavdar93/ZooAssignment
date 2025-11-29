using Microsoft.Extensions.DependencyInjection;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.DataAccessLayer.FileReaders.Factory
{
    public class FileReaderFactory : IFileReaderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FileReaderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFileReader<T> GetFileReader<T>(FileType fileType)
        {
            var reader = fileType switch
            {
                FileType.Csv => _serviceProvider.GetRequiredService<AnimalTypesCsvReader>() as object,
                FileType.Xml => _serviceProvider.GetRequiredService<ZooXmlReader>() as object,
                FileType.Txt => _serviceProvider.GetRequiredService<FoodPriceTxtReader>() as object,
                _ => throw new ArgumentException("Unknown file type")
            };

            if (reader is IFileReader<T> typedReader)
                return typedReader;

            throw new InvalidOperationException($"Reader does not implement IFileReader<{typeof(T).Name}>");
        }
    }
}
