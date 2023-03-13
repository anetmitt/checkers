namespace DAL.Db;

public abstract class BaseRepository : IBaseRepository
{
    protected readonly AppDbContext Ctx;

    protected BaseRepository(AppDbContext dbContext)
    {
        Ctx = dbContext;
    }

    public string BaseType { get; } = "DB";

    public void SaveChanges()
    {
        Ctx.SaveChanges();
    }
}