using DAL;
using Domain;

namespace DAL.FileSystem;

public class GameOptionsRepositoryFileSystem : BaseRepository, IGameOptionsRepository
{
    private const string FileExtension = "json";
    private readonly string _optionsDirectory = "." + Path.DirectorySeparatorChar + "options";
    public List<string> GetGameOptionsList()
    {
        var res = new List<string>();

        CheckOrCreateDirectory();
        
        foreach (var fileName in Directory.GetFileSystemEntries(_optionsDirectory, "*." + FileExtension))
        {
            res.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        return res;
    }

    public CheckersOptions GetGameOptions(string id)
    {
        var fileContent = File.ReadAllText(GetFileName(id));
        var options = System.Text.Json.JsonSerializer.Deserialize<CheckersOptions>(fileContent);

        if (options == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }
        return options;
    }

    public void SaveGameOptions(string id, CheckersOptions options)
    {
        CheckOrCreateDirectory();
        
        var fileContent = System.Text.Json.JsonSerializer.Serialize(options);
        File.WriteAllText(GetFileName(id), fileContent);
    }

    public void DeleteGameOptions(string id)
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