namespace DAL.FileSystem;

public abstract class BaseRepository : IBaseRepository
{
    public string BaseType { get; } = "FS";
    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}