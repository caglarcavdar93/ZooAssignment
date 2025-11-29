namespace ZooAssignment.DataAccessLayer.Migrations
{
    public interface IMigrationService
    {
        Task MigrateAsync(string animalCsvPath, string pricesTxtPath, string zooXmlPath);
    }
}
