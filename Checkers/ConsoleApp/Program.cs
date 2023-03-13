using ConsoleUI;
using DAL;
using DAL.Db;
using DAL.FileSystem;
using Domain;
using GameBrain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

var gameOptions = new CheckersOptions();
var gameBoard = new CheckersBrain(gameOptions, null);
var game = new CheckersGame();

var dbOptions =
    new DbContextOptionsBuilder<AppDbContext>()
        //.UseLoggerFactory(Helpers.MyLoggerFactory)
        .UseSqlite("Data Source=/Users/anetm/icd0008-2022f/checkers.db")
        .Options;

var ctx = new AppDbContext(dbOptions);
ctx.Database.Migrate();

// ---------------- OPTION REPO ----------------

IGameOptionsRepository repoFs = new GameOptionsRepositoryFileSystem();
IGameOptionsRepository repoDb = new GameOptionsRepositoryDb(ctx);

var repo = repoDb;

// // ---------------- GAME REPO ----------------

IGameRepository gameRepoFs = new GameRepositoryFileSystem();
IGameRepository gameRepoDb = new GameRepositoryDb(ctx);

var gameRepo = gameRepoDb;

// ---------------- VARIABLES ----------------
var stopGame = false;

// ---------------- MENUS ----------------

var optionsMenu = new Menu(">> - - CHECKERS OPTIONS - - <<", EMenuLevel.Second, new List<MenuItem>()
{
    new ("Create options", CreateNewOptions),
    new ("Load options", LoadGameOptions),
    new ("Delete options", DeleteOption)
});

var gameActions = new Menu(">>  Play or Delete?  <<", EMenuLevel.Other,
    new List<MenuItem>() {
        new ("Play", PlayGame),
        new ("Delete", DeleteGame)});

var getPlayerType = new Menu(">>  Player options  <<", EMenuLevel.Special, new List<MenuItem>()
{
    new ("Player vs AI", null),
    new ("Player vs player", null)
});

var mainMenu = new Menu(">> - - - CHECKERS - - - <<", EMenuLevel.Main, new List<MenuItem>()
{
    new ("New Game", NewGame),
    new ("Load Game", LoadGame),
    new ("Options", optionsMenu.RunMenu),
    new ("Persistence method swap", SwapPersistenceEngine)
});


// ---------------- RUN MENU ----------------

mainMenu.RunMenu();

// ---------------- SWAP PERSISTENCE ENGINE ----------------

string SwapPersistenceEngine()
{
    if (repo.BaseType == "DB")
    {
        repo = repoFs;
        gameRepo = gameRepoFs;
    }
    else
    {
        repo = repoDb;
        gameRepo = gameRepoDb;
    }
    
    ShowMessageWithTimer(1000, "Has been swapped to " + repo.BaseType + "!");

    return "";
}


// ---------------- GAME OPTIONS LOGIC ----------------

List<MenuItem> ListGameOptions()
{
    var menuItems = new List<MenuItem>();
    
    foreach (var id in repo.GetGameOptionsList())
    {
        menuItems.Add(new MenuItem(id, null));
    }

    return menuItems;
}

string LoadGameOptions()
{
    var savedOptions = new Menu(">>  Please choose your options  <<", EMenuLevel.Other,
        new List<MenuItem>(ListGameOptions()));

    var movements = new List<string> {"Back", "Main menu", "Exit"};
    var id = savedOptions.RunMenu();

    if (!movements.Contains(id))
    {
        gameOptions = repo.GetGameOptions(id);
        gameOptions.LoadedOptions = true;
        gameBoard = new CheckersBrain(gameOptions, null);

        ShowMessageWithTimer(1000, $"The option '{id}' was loaded!");
    }
    return id;
}

string CreateNewOptions()
{
    string? id;
    int width;
    int height;
    bool whiteStarts;

    do
    {
        Console.WriteLine("Board width should be bigger than 3 and dividable by 2. Type in the game board width: ");
        if (int.TryParse(Console.ReadLine(), out width) && width % 2 == 0 && width > 3)
        {
            break;
        }
        
    } while (true);
    
    do
    {
        Console.WriteLine("Board height should be bigger than 3 and dividable by 2. Type in the game board height: ");
        if (int.TryParse(Console.ReadLine(), out height) && height % 2 == 0 && height > 3)
        {
            break;
        }
        
    } while (true);
    
    do
    {
        Console.WriteLine("White starts (true/false): ");
        if (Boolean.TryParse(Console.ReadLine(), out whiteStarts))
        {
            break;
        }
        
    } while (true);

    do
    {
        Console.WriteLine("How would you like to name the option? Please type here: ");
        id = Console.ReadLine();
    } while (id == null);

    var newOption = new CheckersOptions();
    
    newOption.Name = id;
    newOption.Width = width;
    newOption.Height = height;
    newOption.WhiteStarts = whiteStarts;
    
    repo.SaveGameOptions(id, newOption);

    gameOptions = newOption;

    return "";
}

string DeleteOption()
{
    var savedOptions = new Menu(">>  Please choose your options  <<", EMenuLevel.Other,
        new List<MenuItem>(ListGameOptions()));
    var movements = new List<string> {"Back", "Main menu", "Exit"};
    var id = savedOptions.RunMenu();

    if (!movements.Contains(id))
    { 
        repo.DeleteGameOptions(id);
        ShowMessageWithTimer(1000, $"The option '{id}' was deleted!");
    }
    
    return id;
}


// ---------------- PLAYER INFO LOGIC ----------------

void GetPlayerTypes()
{
    var type = getPlayerType.RunMenu();
    game.PlayerOneType = EPlayerType.Human;

    if (type == "Player vs player")
    {
        game.PlayerTwoType = EPlayerType.Human;
        return;
    }
    
    game.PlayerTwoType = EPlayerType.Ai;
}

void GetPlayerNames()
{
    if (game.PlayerTwoType == EPlayerType.Ai)
    {
        game.PlayerOneName = CheckIfCorrectPlayerName("Please enter the player's name: ");
        return;
    }
    
    game.PlayerOneName = CheckIfCorrectPlayerName("Please enter the player1 name: ");
    game.PlayerTwoName = CheckIfCorrectPlayerName("Please enter the player2 name: ");
}

string CheckIfCorrectPlayerName(string question)
{
    var name = "";
    var hasName = false;

    while (!hasName)
    {
        Console.Clear();
        Console.WriteLine(question);
        name = Console.ReadLine();
        hasName = name != "";
    }

    return name!;
}


// ---------------- NEW GAME LOGIC ----------------

string NewGame()
{
    if (!gameBoard.loadedOptions)
    {
        CreateNewOptions(); 
        gameBoard = new CheckersBrain(gameOptions, null);
    }
    
    game.CheckersOptions = gameOptions;
    
    GetPlayerTypes();
    GetPlayerNames();
    
    Console.Clear();

    PlayGame();

    return "Main menu";
}

// ---------------- GAME PLAY LOGIC ----------------

string PlayGame()
{

    do
    {
        if (gameBoard.CheckIfWin()) { GameOver(); break;}
        
        string?[][] boardCopy = gameBoard!.CheckIfCanTakeAnyPiece();

        if (gameBoard.NextMoveByBlack() && game.PlayerTwoType == EPlayerType.Ai
            || !gameBoard.NextMoveByBlack() && game.PlayerOneType == EPlayerType.Ai)
        {
            DrawBoard(boardCopy);
            gameBoard.AiMakesAMove();
        }
        else
        {
            var pieceCoord = AskForPieceCoordinates(boardCopy);
            
            if (stopGame) { break; }
            
            boardCopy = gameBoard.GetPossibleMoves(pieceCoord[0], pieceCoord[1]);
            var pieceNewCoord = AskForNewPieceCoordinates(boardCopy, pieceCoord[0], pieceCoord[1]);
            
            if (stopGame) { break; }

            gameBoard!.MakeAMove(pieceNewCoord[0], pieceNewCoord[1], pieceCoord[0], pieceCoord[1]);
            if (!gameBoard.CheckIfCanTakeThePiece(pieceNewCoord[0], pieceNewCoord[1],
                    null, gameBoard.GetBoard()[pieceNewCoord[0]][pieceNewCoord[1]])
                || Math.Abs(pieceCoord[0] - pieceNewCoord[0]) == 1)
            {
                gameBoard!.SetNextMove();
            }
        }
        
    } while (!stopGame);

    return SaveGame();
}

void GameOver()
{
    var winner = gameBoard!.GetBlackPoints() > gameBoard.GetWhitePoints() ? game.PlayerTwoName : game.PlayerOneName;
    game.GameWinner = winner;
    game.GameOver = DateTime.Now;
    ShowMessageWithTimer(1000, $"{winner} won!!!");
}

List<int> AskForNewPieceCoordinates(string?[][] boardCopy, int x, int y)
{
    var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    var canMove = false;
    var xEnd = 0;
    var yEnd = 0;
    
    do
    {
        DrawBoard(boardCopy);
        Console.WriteLine("Enter the coordinates where you want to place the piece: ");
        var newCoord = Console.ReadLine();

        if (newCoord == "save") { stopGame = true; break;}

        if (newCoord != null && newCoord.Length == 2)
        {
            xEnd = newCoord[0] - '1';
            var letter = char.ToUpper(newCoord[1]);

            if (xEnd < gameOptions!.Height && xEnd >= 0 && alpha.Contains(letter) && newCoord.Length == 2)
            {
                yEnd = Array.IndexOf(alpha, letter);

                if (boardCopy[xEnd][yEnd] is "move" or "take" && xEnd != x && yEnd != y) 
                {
                    canMove = true;
                }
                else
                {
                    ShowMessageWithTimer(1000, "Can't place the piece there!");
                }
            }
        }

    } while (!canMove);
    
    return new List<int>{xEnd, yEnd};
}

List<int> AskForPieceCoordinates(string?[][] boardCopy)
{
    var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    var canMove = false;

    int x = 0;
    int y = 0;

    do
    {
        DrawBoard(boardCopy);
        Console.WriteLine("Enter the piece coordinates which you would like to move: ");
        var piece = Console.ReadLine();
        
        if (piece == "save") { stopGame = true; break; }
        
        if (piece != null && piece.Length == 2)
        {
            x = piece[0] - '1';
            var letter = char.ToUpper(piece[1]);
            
            if (x < gameOptions!.Height && x >= 0 && alpha.Contains(letter))
            {
                y = Array.IndexOf(alpha, letter);
                
                if (gameBoard.CheckIfGivenPieceCanMove(x, y, boardCopy))
                {
                    canMove = true;
                } 
                else
                {
                    ShowMessageWithTimer(1000, "Incorrect piece coordinates!");
                }
            }
        }
    } while (!canMove);

    return new List<int>{x, y};
}

// ---------------- GAME UI LOGIC ----------------

void DrawBoard(string?[][] boardCopy)
{
    var playerTurn = gameBoard.NextMoveByBlack() ? game.PlayerTwoName : game.PlayerOneName;
    Console.Clear();
    Console.WriteLine("Type 'save' to save the game after this move and go back to main menu!\n");
    Console.WriteLine($"It's {playerTurn}'s turn!");
    Console.WriteLine("Player2: " + game.PlayerTwoName);
    UI.DrawGameBoard(gameBoard!.GetBoard(), boardCopy);
    Console.WriteLine("Player1: " + game.PlayerOneName);
}

// ---------------- GAME LOADING LOGIC ----------------

List<MenuItem> LoadGames()
{
    var games = gameRepo.GetGamesList();
    var menuItems = new List<MenuItem>();

    foreach (var momentGame in games)
    {
        menuItems.Add(new MenuItem(momentGame.GameName!, () =>
        {
            game = gameRepo.GetGame(momentGame.GameName!);
            gameBoard = new CheckersBrain(game!.CheckersOptions!, game!.CheckersGameStates?.LastOrDefault());
            gameOptions = game!.CheckersOptions;
            
            return gameActions!.RunMenu();
        }));
    }

    return menuItems;
}

string LoadGame()
{
    var savedGamesMenu = new Menu(">>   Choose your game   <<<", EMenuLevel.Second, new List<MenuItem>(LoadGames()));
    var id = savedGamesMenu.RunMenu();

    return id;
}

// ---------------- GAME SAVING LOGIC ----------------

string SaveGame()
{
    string? saveId;
    
    if (game.GameName == null)
    {
        do
        {
            Console.WriteLine("Give this save a name: ");
            saveId = Console.ReadLine();   
        } while (saveId == null);
        
    }
    else
    { 
        saveId = game.GameName;
    }

    var serializeGameState = gameBoard!.GetSerializedState();
    var newGameState = new CheckersGameState { SerializedGameState = serializeGameState };

    game.CheckersGameStates = new List<CheckersGameState>();
    game.CheckersGameStates.Add(newGameState);
    game.CheckersOptions = gameOptions;
    game.CheckersOptionsId = gameOptions.Id;
    game.GameName = saveId;

    gameRepo.SaveGame(saveId, game);
    stopGame = false;
    
    gameOptions = new CheckersOptions();
    gameBoard = new CheckersBrain(gameOptions, null);
    game = new CheckersGame();
    
    ShowMessageWithTimer(1000, "Game saved!");

    return "Main menu";
}

// ---------------- GAME DELETING LOGIC ----------------

string DeleteGame()
{
    gameRepo.DeleteGame(game.GameName!);
    ShowMessageWithTimer(1000, "Game deleted!");
    
    return "Main menu";
}

// ---------------- SHOW MESSAGE FOR SOME TIME ----------------

void ShowMessageWithTimer(int time, string message)
{
    Console.Clear();
    Console.WriteLine(message);
    Thread.Sleep(time);

}
