using Domain;

namespace DAL.FileSystem;

public class GameRepositoryFileSystem : BaseRepository, IGameRepository
{   
    private const string FileExtension = "json";
    private readonly string _optionsDirectory = "." + Path.DirectorySeparatorChar + "games";
    
    public List<CheckersGame> GetGamesList()
    {
        var res = new List<CheckersGame>();

        CheckOrCreateDirectory();
        
        foreach (var fileName in Directory.GetFileSystemEntries(_optionsDirectory, "*." + FileExtension))
        {
            res.Add(GetGame(Path.GetFileNameWithoutExtension(fileName)));
        }

        return res;
    }

    public CheckersGame GetGame(string id)
    {
        var fileContent = File.ReadAllText(GetFileName(id));
        var game = System.Text.Json.JsonSerializer.Deserialize<CheckersGame>(fileContent);

        if (game == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }
        return game;
    }

    public CheckersGame GetGame(int id)
    {
        return new CheckersGame();
    }

    public CheckersGame SaveGame(string id, CheckersGame game)
    {
        CheckOrCreateDirectory();
        
        var fileContent = System.Text.Json.JsonSerializer.Serialize(game);
        File.WriteAllText(GetFileName(id), fileContent);

        return game;
    }

    public void DeleteGame(string id)
    {
        File.Delete(GetFileName(id));
    }
    
    private string GetFileName(string id)
    {
        return _optionsDirectory + Path.DirectorySeparatorChar + id + "." + FileExtension;
    }

    private void CheckOrCreateDirectory()
    {
        if (!Directory.Exists(_optionsDirectory))
        {
            Directory.CreateDirectory(_optionsDirectory);
        }
    }

    public string BaseType { get; } = "FS";
}