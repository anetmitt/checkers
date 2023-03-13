using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Db;

public class GameRepositoryDb : BaseRepository, IGameRepository
{
    public GameRepositoryDb(AppDbContext dbContext) : base(dbContext)
    {
    }

    public List<CheckersGame> GetGamesList() =>
        Ctx.CheckersGames
            .Include(o => o.CheckersOptions)
            .OrderBy(o => o.GameStarted)
            .ToList();


    public CheckersGame GetGame(string id)
    {
        return Ctx.CheckersGames
            .Include(g => g.CheckersOptions)
            .Include(g => g.CheckersGameStates)
            .First(o => o.GameName == id);
    }
    
    public CheckersGame? GetGame(int id)
    {
        return Ctx.CheckersGames
            .Include(g => g.CheckersOptions)
            .Include(g => g.CheckersGameStates)
            .FirstOrDefault(g => g.Id == id);
    }

    public CheckersGame SaveGame(string id, CheckersGame game)
    {
        var gameFromDb = Ctx.CheckersGames.FirstOrDefault(o => o.GameName == id);

        if (gameFromDb == null)
        {
            Ctx.CheckersGames.Add(game);
            Ctx.SaveChanges();
            return game;
        }

        gameFromDb.GameName = game.GameName;
        gameFromDb.GameOver = game.GameOver;
        gameFromDb.GameStarted = game.GameStarted;
        gameFromDb.GameWinner = game.GameWinner;
        gameFromDb.CheckersGameStates = game.CheckersGameStates;
        gameFromDb.PlayerOneName = game.PlayerOneName;
        gameFromDb.PlayerOneType = game.PlayerOneType;
        gameFromDb.PlayerTwoName = game.PlayerTwoName;
        gameFromDb.PlayerTwoType = game.PlayerTwoType;
        
        Ctx.SaveChanges();

        return gameFromDb;
    }

    public void DeleteGame(string id)
    {
        var gameFromDb = GetGame(id);
        Ctx.CheckersGames.Remove(gameFromDb);
        Ctx.SaveChanges();
    }
    
}