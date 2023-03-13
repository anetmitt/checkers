using Domain;

namespace DAL;

public interface IGameOptionsRepository : IBaseRepository
{
    //crud - create, read, update, delete (how?)
    
    //read
    List<string> GetGameOptionsList();
    CheckersOptions GetGameOptions(string id);
    
    //create and update
    void SaveGameOptions(string id, CheckersOptions options);
    
    //delete
    void DeleteGameOptions(string id);
}