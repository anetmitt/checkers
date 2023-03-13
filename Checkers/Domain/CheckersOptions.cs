using System.ComponentModel.DataAnnotations;

namespace Domain;

public class CheckersOptions
{
     
     public int Id { get; set; }
     
     [MaxLength(20)]
     public string? Name { get; set; }
      
     public int Width { get; set; } = 8;
     public int Height { get; set; } = 8;

     public bool WhiteStarts { get; set; } = true;

     public bool LoadedOptions { get; set; } = false;
     
     public ICollection<CheckersGame>? CheckersGame { get; set; }

}