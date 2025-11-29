using ZooAssignment.DataAccessLayer.Models;
using ZooAssignment.DataAccessLayer.FileReaders;

namespace ZooAssignment.DataAccessLayer.FileReaders.Factory
{
    public interface IFileReaderFactory
    {
        IFileReader<T> GetFileReader<T>(FileType fileType);
    }

    public enum FileType
    {
        Csv,
        Xml,
        Txt
    }
}
