using Domain;
using Newtonsoft.Json;

namespace GameBrain;


public class CheckersBrain
{
     private int boardWidth {get; set; }
     private int boardHeight {get; set; }

     public bool loadedOptions { get; set; }

     private bool ShouldTake = false;

     private readonly CheckersState _state;
     
     public CheckersBrain(CheckersOptions options, CheckersGameState? state)
     {
         if (state == null)
         {
              _state = new CheckersState();
              _state.NextMoveByBlack = !options.WhiteStarts;
              InitializeNewGame(options);
         }
         else
         {
             _state = JsonConvert.DeserializeObject<CheckersState>(state.SerializedGameState)!;
         }
     }

     private void InitializeNewGame(CheckersOptions options)
     {
         boardWidth = options.Width;
         boardHeight = options.Height;
         loadedOptions = options.LoadedOptions;
         
         if (boardWidth < 4 || boardHeight < 4 || boardHeight % 2 != 0 || boardWidth % 2 != 0)
         {
             throw new ArgumentException("Board size too small");
         }

         GetNewGameBoard();
     }

     public void GetNewGameBoard()
     {
         _state.GameBoard = new EGamePiece?[boardHeight][];

         for (var x = 0; x < boardHeight; x++)
         {
             _state.GameBoard[x] = new EGamePiece?[boardWidth];
             var boardMiddle = boardHeight / 2;

             if (x == boardMiddle || x == boardMiddle - 1)
             {
                 continue;
             }

             for (var y = 0; y < boardWidth; y++)
             {
                 if (x < boardMiddle - 1)
                 {
                     if (x % 2 == 0)
                     {
                         _state.GameBoard[x][y] = (y % 2 == 0) ? null : EGamePiece.Black;
                     }
                     else
                     {
                         _state.GameBoard[x][y] = (y % 2 != 0) ? null : EGamePiece.Black;
                     }
                 }
                 
                 else
                 {
                     if (x % 2 == 0)
                     {
                         _state.GameBoard[x][y] = (y % 2 == 0) ? null : EGamePiece.White;
                     }
                     else
                     {
                         _state.GameBoard[x][y] = (y % 2 != 0) ? null : EGamePiece.White;
                     }
                 }
             }
             
         }
     }

     public bool NextMoveByBlack() => _state.NextMoveByBlack;
     
     public void SetNextMove() => _state.NextMoveByBlack = !_state.NextMoveByBlack;

     public int GetWhitePoints() => _state.WhitePoints;
     
     public int GetBlackPoints() => _state.BlackPoints;

     public bool CheckIfWin()
     {
         var board = GetBoard();
         
         var buttonCount = (board.Length * board[0].Length - 2 * board.Length) / 4;

         return buttonCount == GetWhitePoints() || buttonCount == GetBlackPoints();
     }

     public string[][] GenerateCopyOfBoard()
     {
         var gameBoardCopy = new string[_state.GameBoard.Length][];
         
         for (var i = 0; i < _state.GameBoard.Length; i++)
         {
             gameBoardCopy[i] = new string[_state.GameBoard[0].Length];
         }

         return gameBoardCopy;
     }

     public bool CheckIfGivenPieceCanMove(int x, int y, string?[][] boardCopy)
     {
         var canTake = false;
         var canMove = false;

         boardCopy = GetPossibleMoves(x, y);
         
         Array.ForEach(boardCopy, each =>
         {
             if (each.Contains("needsToMove")) { canTake = true; }
             else if (each.Contains("move")) { canMove = true; }
         });
         
         if (CheckIfTheCorrectPieceHasBeenChosen(x, y))
         {
             if (canTake && boardCopy[x][y] == "needsToMove" || canMove)
             {
                 return true;
             } 
         }

         return false;
     }
     public string[][] GetPossibleMoves(int x, int y)
     {
         var piece = _state.GameBoard[x][y];
         var gameBoardCopy = GenerateCopyOfBoard();
         var take = CheckIfCanTakeAnyPiece();

         if (ShouldTake)
         {
             ShouldTake = false;
             return take;
         }
         
         if (piece is EGamePiece.BlackKing or EGamePiece.WhiteKing)
         {
             if (CheckIfKingCanMoveOrEat(x, y, gameBoardCopy, true))
             {
                 return gameBoardCopy;
             }

             CheckIfKingCanMoveOrEat(x, y, gameBoardCopy, false);
             return gameBoardCopy;
         }

         if (piece == EGamePiece.Black)
         {
             CheckIfMovePossible(x, y, 1, -1, gameBoardCopy);
             CheckIfMovePossible(x, y, 1, 1, gameBoardCopy);
         }

         if (piece == EGamePiece.White)
         {
             CheckIfMovePossible(x, y, -1, -1, gameBoardCopy);
             CheckIfMovePossible(x, y, -1, 1, gameBoardCopy);
         }

         return gameBoardCopy;
     }
     
     private void CheckIfMovePossible(int x, int y, int AddX, int AddY, string[][] boardCopy)
     {
         if (x + AddX < 0 || x + AddX >= _state.GameBoard.Length || y + AddY < 0 || y + AddY >= _state.GameBoard[0].Length)
         {
             return;
         }
         
         var canMove = _state.GameBoard[x + AddX][y + AddY] == null;
        
         if (canMove)
         {
             boardCopy[x + AddX][y + AddY] = "move";
         }
     }

     public string[][] CheckIfCanTakeAnyPiece()
     {
         var boardCopy = GenerateCopyOfBoard();

         for (var x = 0; x < _state.GameBoard.Length; x++)
         {
             for (var y = 0; y <_state.GameBoard[0].Length; y++)
             {
                 if (_state.GameBoard[x][y] is EGamePiece.BlackKing or EGamePiece.WhiteKing)
                 {
                     CheckIfKingCanMoveOrEat(x, y, boardCopy, true);
                 }
                 else
                 {
                     CheckIfCanTakeThePiece(x, y, boardCopy, _state.GameBoard[x][y]);
                 }
             }
         }

         return boardCopy;
     }

     public bool CheckIfCanTakeThePiece(int x, int y, string[][]? boardCopy, EGamePiece? piece)
     {
         var take = false;

         boardCopy ??= GenerateCopyOfBoard();

         if (piece != null)
         {
             if (CheckTake(x, y, 2, -2, boardCopy, piece)) { take = true; }
             if (CheckTake(x, y, 2, 2, boardCopy, piece)) { take = true; }
             if (CheckTake(x, y, -2, 2, boardCopy, piece)) { take = true; }
             if (CheckTake(x, y, -2, -2, boardCopy, piece)) { take = true; }
         }

         return take;
     }
     
     private bool CheckTake(int x, int y, int AddX, int AddY, string[][] boardCopy, EGamePiece? piece)
     {
         if (x + AddX < 0 || x + AddX >= _state.GameBoard.Length || y + AddY < 0 || y + AddY >= _state.GameBoard[0].Length)
         {
             return false;
         }

         var enemyPiece = piece is EGamePiece.Black or EGamePiece.BlackKing ? new List<EGamePiece?>{EGamePiece.White, EGamePiece.WhiteKing} : new List<EGamePiece?>{EGamePiece.Black, EGamePiece.BlackKing};
         var canTake = _state.GameBoard[x + AddX][y + AddY] == null && enemyPiece.Contains(_state.GameBoard[x + AddX / 2][y + AddY / 2]);
        
         if ((piece is EGamePiece.Black or EGamePiece.BlackKing && NextMoveByBlack()
              || piece is EGamePiece.White or EGamePiece.WhiteKing && !NextMoveByBlack()) && canTake)
         {
             ShouldTake = true;
             if (_state.GameBoard[x][y] != null)
             {
                 boardCopy[x][y] = "needsToMove";
             }
             boardCopy[x + AddX][y + AddY] = "take";
             return true;
         }

         return canTake;
     }

     private bool CheckKingMovesInAnyDirection(int x, int y, int addX, int addY, string[][] boardCopy, EGamePiece? piece, bool onlyTakeCheck)
     {
         if (x + addX < 0 || x + addX >= _state.GameBoard.Length || y + addY < 0 || y + addY >= _state.GameBoard[0].Length)
         {
             return false;
         }
         
         var canMoveMore = true;
         var canTake = false;
         
         while (x + addX >= 0 && x + addX < _state.GameBoard.Length && y + addY >= 0 && y + addY < _state.GameBoard[0].Length && canMoveMore)
         {
             
             if (_state.GameBoard[x + addX][y + addY] == null)
             {
                 if (onlyTakeCheck) { x += addX; y += addY; continue; }
                 boardCopy[x + addX][y + addY] = "move";
             }
             else
             {
                 var multiplyX = addX < 0 ? -1 : 1;
                 var multiplyY = addY < 0 ? -1 : 1;
                 canTake = CheckTake(x, y, multiplyX * (Math.Abs(addX) + 1), multiplyY * (Math.Abs(addY) + 1), boardCopy, piece);
                 canMoveMore = false;
             }

             x += addX;
             y += addY;
         }

         return canTake;
     }

     private bool CheckIfKingCanMoveOrEat(int x, int y, string[][] boardCopy, bool onlyTakeCheck)
     {
         var piece = _state.GameBoard[x][y];
         var canTake = false;

         
         if (CheckKingMovesInAnyDirection(x, y, 1, -1, boardCopy, piece, onlyTakeCheck)
             || CheckKingMovesInAnyDirection(x, y, 1, 1, boardCopy, piece, onlyTakeCheck)
             || CheckKingMovesInAnyDirection(x, y, -1, 1, boardCopy, piece, onlyTakeCheck)
             || CheckKingMovesInAnyDirection(x, y, -1, -1, boardCopy, piece, onlyTakeCheck))
         {
             if (NextMoveByBlack() && piece == EGamePiece.BlackKing || !NextMoveByBlack() && piece == EGamePiece.WhiteKing)
             {
                 boardCopy[x][y] = "needsToMove";
                 ShouldTake = true;
             }
             canTake = true;
         }
         
         return canTake;
     }

     public void MakeAMove(int x, int y, int startX, int startY)
     {
         if (_state.GameBoard[x][y] == null)
         {
             TakeThePiece(x, y, startX, startY);
             _state.GameBoard[x][y] = _state.GameBoard[startX][startY];
             _state.GameBoard[startX][startY] = null;
             ChangeThePieceToKing(x, y);
         }
     }

     public bool CheckIfTheCorrectPieceHasBeenChosen(int x, int y)
     {
         var piece = _state.GameBoard[x][y];
         
         if (piece is EGamePiece.White or EGamePiece.WhiteKing && !NextMoveByBlack())
         {
             return true;
         }
         
         if (piece is EGamePiece.Black or EGamePiece.BlackKing && NextMoveByBlack())
         {
             return true;
         }

         return false;
     }

     private void ChangeThePieceToKing(int x, int y)
     {
         var piece = _state.GameBoard[x][y];
         
         _state.GameBoard[x][y] = piece switch
         {
             EGamePiece.Black when x == _state.GameBoard.Length - 1 => EGamePiece.BlackKing,
             EGamePiece.White when x == 0 => EGamePiece.WhiteKing,
             _ => _state.GameBoard[x][y]
         };
     }
     
     public void TakeThePiece(int EndX, int EndY, int startX, int startY)
     {
         var differenceX = Math.Abs(startX - EndX);
         var differenceY = Math.Abs(startY - EndY);

         if (differenceX > 1 && differenceY > 1)
         {
             if (EndX - differenceX == startX && EndY + differenceY == startY && _state.GameBoard[EndX - 1][EndY + 1] != null)
             {
                 _state.GameBoard[EndX - 1][EndY + 1] = null;
                 _state.BlackPoints += NextMoveByBlack() ? 1 : 0;
                 _state.WhitePoints += !NextMoveByBlack() ? 1 : 0;

             } else if (EndX - differenceX == startX && EndY - differenceY == startY && _state.GameBoard[EndX - 1][EndY - 1] != null)
             {
                 _state.GameBoard[EndX - 1][EndY - 1] = null;
                 _state.BlackPoints += NextMoveByBlack() ? 1 : 0;
                 _state.WhitePoints += !NextMoveByBlack() ? 1 : 0;
             
             } else if (EndX + differenceX == startX && EndY + differenceY == startY && _state.GameBoard[EndX + 1][EndY + 1] != null)
             {
                 _state.GameBoard[EndX + 1][EndY + 1] = null;
                 _state.BlackPoints += NextMoveByBlack() ? 1 : 0;
                 _state.WhitePoints += !NextMoveByBlack() ? 1 : 0;
             
             } else if (EndX + differenceX == startX && EndY - differenceY == startY && _state.GameBoard[EndX + 1][EndY - 1] != null)
             {
                 _state.GameBoard[EndX + 1][EndY - 1] = null;
                 _state.BlackPoints += NextMoveByBlack() ? 1 : 0;
                 _state.WhitePoints += !NextMoveByBlack() ? 1 : 0;
             }
         }
     }

     private List<(int, int, int, int)> ParseData(string?[][] boardCopy, int x, int y)
     {
         List<(int, int, int, int)> moves = new List<(int startX, int startY, int EndX, int EndY)>();
         List<(int, int)> takeStart = new List<(int, int)>();
         List<(int, int)> takeEnd = new List<(int, int)>();

         for (int i = 0; i < boardCopy.Length; i++)
         {
             string?[] row = boardCopy[i];

             for (int j = 0; j < row.Length; j++)
             {
                 if (row[j] == "move") { moves.Add((x, y, i, j)); }
                 
                 else if (row[j] == "take")
                 {
                     if (takeStart.Count != 0 && takeStart[0].Item1 != i && takeStart[0].Item2 != j || takeStart.Count == 0)
                     {
                         takeEnd.Add((i, j));
                     }
                 }
                 else if (row[j] == "needsToMove")
                 {
                     if (takeEnd.Count != 0 && takeEnd[0].Item1 != i && takeEnd[0].Item2 != j || takeEnd.Count == 0)
                     {
                         takeStart.Add((i, j));
                     }
                 }
             }
         }

         if (takeStart.Count != 0)
         {
             moves.Add((takeStart[0].Item1, takeStart[0].Item2, takeEnd[0].Item1, takeEnd[0].Item2));   
         }

         return moves;
     }

     public void AiMakesAMove()
     {
         var AllPossibleMoves = new List<(int, int, int, int)>();
         var rnd = new Random();
         
         for (int x = 0; x < _state.GameBoard.Length; x++)
         {
             for (int y = 0; y < _state.GameBoard[0].Length; y++)
             {
                 if (CheckIfTheCorrectPieceHasBeenChosen(x, y))
                 {
                     List<(int, int, int, int)> moves = ParseData(GetPossibleMoves(x, y), x, y);

                     if (moves.Count != 0)
                     {
                         AllPossibleMoves.Add(moves[0]);
                     }
                 }
             }
         }

         if (AllPossibleMoves.Count > 0)
         {
             var (x, y, EndX, EndY) = AllPossibleMoves[rnd.Next(0, AllPossibleMoves.Count)];
             MakeAMove(EndX, EndY, x,y);

             if (!CheckIfCanTakeThePiece(EndX, EndY, null, _state.GameBoard[EndX][EndY]) || Math.Abs(EndX - x) == 1) { SetNextMove(); }
         }
     }

     public EGamePiece?[][] GetBoard()
     {
         var jsonStr = System.Text.Json.JsonSerializer.Serialize(_state.GameBoard);
         return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;

     }

     public string GetSerializedState()
     {
         return JsonConvert.SerializeObject(_state);
     }
    
}