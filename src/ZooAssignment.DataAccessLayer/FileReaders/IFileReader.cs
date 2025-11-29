namespace ZooAssignment.DataAccessLayer.FileReaders
{
    public interface IFileReader<T>
    {
        List<T> Read(string filePath);
    }
}
