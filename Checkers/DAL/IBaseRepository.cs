namespace DAL;

public interface IBaseRepository
{
     public string BaseType { get; }

     void SaveChanges();
}