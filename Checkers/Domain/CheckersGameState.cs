namespace Domain;

public class CheckersGameState
{
    // Primary key
    public int Id { get; set; }
    
    public DateTime CreatedTime { get; set; } = DateTime.Now;

    public string SerializedGameState { get; set; } = default!;
    
    // FK
    public int CheckersGameId { get; set; }
    public CheckersGame? CheckersGame { get; set; }
}