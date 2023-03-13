namespace Domain;

public class CheckersState
{
    public EGamePiece?[][] GameBoard = default!;

    public int BlackPoints { get; set; } = 0;

    public int WhitePoints { get; set; } = 0;
    public bool NextMoveByBlack { get; set; } = true;
}