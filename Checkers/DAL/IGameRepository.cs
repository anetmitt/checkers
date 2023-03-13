using Domain;

namespace DAL;

public interface IGameRepository : IBaseRepository
{
    //crud - create, read, update, delete (how?)
    
    //read
    List<CheckersGame> GetGamesList();
    
    CheckersGame GetGame(string id);
    
    CheckersGame GetGame(int id);
    
    //create and update
    CheckersGame SaveGame(string id, CheckersGame game);
    
    //delete
    void DeleteGame(string id);
}