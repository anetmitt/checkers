using System.Media;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace WebApp.Pages.CheckersGames;

public class Play : PageModel
{
    private readonly IGameRepository _repo;

    public bool GameOver = false;

    private SoundPlayer Player{ get; set; } = new SoundPlayer();
    
    private string _soundLocation = "/Users/anetm/icd0008-2022f/Checkers/WebApp/wwwroot/game-piece-slide-.wav";

    public CheckersGame Game { get; set; } = default!;

    public string[][] PossibleMoves { get; set; } = default!;
    
    public string[][] TakePieceIfPossible { get; set; } = default!;

    public bool NextMoveByBlack;
    
    public EGamePiece?[][] GameBoard { get; set; } = default!;

    public Play(IGameRepository repo)
    {
        _repo = repo;
        Player.SoundLocation = _soundLocation;
        Player.Load();
    }

    public CheckersBrain CheckersBrain { get; set; } = default!;

    public async Task<IActionResult> OnGet(int? id, int? x, int? y, int? pieceX, int? pieceY, bool? ai)
    {
        var game = _repo.GetGame(id.Value);

        if (game == null || game.CheckersOptions == null)
        {
            return NotFound();
        }

        Game = game;

        CheckersBrain = new CheckersBrain(game.CheckersOptions, game.CheckersGameStates?.LastOrDefault());
        NextMoveByBlack = CheckersBrain.NextMoveByBlack();
        GameBoard = CheckersBrain.GetBoard();
        

        if (CheckersBrain.CheckIfWin())
        {
            GameOver = true;
                
            game.GameWinner = CheckersBrain.GetWhitePoints() > CheckersBrain.GetWhitePoints()
                ? game.PlayerOneName
                : game.PlayerTwoName;
            
            game.GameOver = DateTime.Now;
            
            game.CheckersGameStates!.Add(new CheckersGameState
            {
                SerializedGameState = CheckersBrain.GetSerializedState()
            });

            _repo.SaveChanges();
        }
        

        if (x != null && y != null && pieceX != null && pieceY != null && x != pieceX && y != pieceY)
        {
            Player.Play();

            CheckersBrain.MakeAMove(x.Value, y.Value, pieceX.Value, pieceY.Value);

            if (!CheckersBrain.CheckIfCanTakeThePiece(x.Value, y.Value, null, CheckersBrain.GetBoard()[x.Value][y.Value])
                || Math.Abs(x.Value - pieceX.Value) == 1)
            {
                CheckersBrain.SetNextMove();
                NextMoveByBlack = CheckersBrain.NextMoveByBlack();
            }
            
            game.CheckersGameStates!.Add(new CheckersGameState
            {
                SerializedGameState = CheckersBrain.GetSerializedState()
            });

            _repo.SaveChanges();
            
        } else if (pieceX != null && pieceY != null)
        {
            PossibleMoves = CheckersBrain.GetPossibleMoves(pieceX.Value, pieceY.Value);
            
        } else if ((game.PlayerOneType == EPlayerType.Ai && !NextMoveByBlack ||
                   game.PlayerTwoType == EPlayerType.Ai && NextMoveByBlack) && ai == true && GameOver == false)
        {
            Player.Play();
            
            CheckersBrain.AiMakesAMove();
            GameBoard = CheckersBrain.GetBoard();
            NextMoveByBlack = CheckersBrain.NextMoveByBlack();

            game.CheckersGameStates!.Add(new CheckersGameState
            {
                SerializedGameState = CheckersBrain.GetSerializedState()
            });

            _repo.SaveChanges();
            
        }

        TakePieceIfPossible = CheckersBrain.CheckIfCanTakeAnyPiece();

        if (x != null && y != null && pieceX != null && pieceY != null)
        {
            return Redirect("http://localhost:5000/CheckersGames/Play?id=" + id + "&ai=false");
        }
        
        return Page();
    }
}