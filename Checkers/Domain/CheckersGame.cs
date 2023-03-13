using System.ComponentModel.DataAnnotations;

namespace Domain;

public class CheckersGame
{
    // Primary key
    public int Id { get; set; }
    
    public string? GameName { get; set; }
    
    public DateTime GameStarted { get; set; } = DateTime.Now;
    public DateTime? GameOver { get; set; }
    
    public string? GameWinner { get; set; }
    
    [MaxLength(128)]
    public string PlayerOneName { get; set; } = default!;
    public EPlayerType PlayerOneType { get; set; }
    
    [MaxLength(128)]
    public string PlayerTwoName { get; set; } = "AI";
    public EPlayerType PlayerTwoType { get; set; }
    
    public int CheckersOptionsId { get; set; }
    public CheckersOptions? CheckersOptions { get; set; }
    
    public ICollection<CheckersGameState>? CheckersGameStates { get; set; }
}