using Domain;

namespace ConsoleUI;

public class UI
{
    public static string?[][]? Copy { get; set; }
    public static void DrawGameBoard(EGamePiece?[][] gameBoard, string?[][] boardCopy)
    {
        Copy = boardCopy;
        
        var rows = gameBoard.GetLength(0);
        
        var cols = gameBoard[0].GetLength(0);

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var printString = (j == 0) ? "   *-----" : "*-----";
                Console.Write(printString);

            }

            Console.Write("*");

            GetRowMiddlePart(cols, i, gameBoard);
        }

        for (var j = 0; j < cols; j++)
        {
            var printString = (j == 0) ? "   *-----" : "*-----";
            Console.Write(printString);
        }

        Console.WriteLine("*");
        
        GetBoardLetters(cols);
    }

    private static void GetBoardLetters(int boardWidth)
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();;
        var content = "   ";

        for (var i = 0; i < boardWidth; i++)
        {
            var letter = letters[i];
            content += "   " + letter + "  ";
        }
        Console.WriteLine(content);

    }

    private static void GetRowMiddlePart(int cols, int i, EGamePiece?[][] gameBoard)
    {
        Console.WriteLine();
        for (var j = 0; j < cols; j++)
        {
            Console.ResetColor();
            if (j == 0)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                var numPart = (i + 1 < 10) ? i + 1 + "  |" : i + 1 + " |";
                Console.Write(numPart);
            }
            else
            {
                Console.Write("|");
            }

            if (i % 2 == 0)
            {
                Console.BackgroundColor = (j % 2 == 0) ? ConsoleColor.White : ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = (j % 2 != 0) ? ConsoleColor.White : ConsoleColor.Black;
            }

            if (Copy != null)
            {
                if (Copy[i][j] == "move")
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                }
                else if (Copy[i][j] == "take")
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
            }

            if (gameBoard[i][j] != null)
            {   Console.Write(" ");
                var gamePiece = "";
                
                if (gameBoard[i][j] == EGamePiece.White) { gamePiece = " W"; }
                else if (gameBoard[i][j] == EGamePiece.WhiteKing) { gamePiece = "WW"; }
                else if (gameBoard[i][j] == EGamePiece.Black) { gamePiece = " B"; }
                else if (gameBoard[i][j] == EGamePiece.BlackKing) { gamePiece = "BB"; }
                
                Console.Write(gamePiece);
                Console.Write("  ");
            }
            else
            {
                Console.Write("     ");
            }
        }
        Console.ResetColor();
        Console.WriteLine("|");
    }
}