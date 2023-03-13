namespace MenuSystem;
using static Console;

public class Menu
{
    public string Title { get; set; }
    private List<MenuItem> MenuItems { get; set; }
    private EMenuLevel _level { get; set; }
    private int _selectedMenuItem;

    private MenuItem goBack = new MenuItem("Back", null);
    private MenuItem backToMain = new MenuItem("Main menu", null);
    private MenuItem exit = new MenuItem("Exit", null);

    public Menu(string title, EMenuLevel level, List<MenuItem> menuItems)
    {
        Title = title;
        MenuItems = menuItems;
        _level = level;

        if (level != EMenuLevel.Main && level != EMenuLevel.Special) { MenuItems.Add(goBack); }

        if (level == EMenuLevel.Other) { MenuItems.Add(backToMain);}

        if (level != EMenuLevel.Special)
        {
            MenuItems.Add(exit);   
        }

    }

    public void DisplayMenu()
    {
        WriteLine(Title);
        for (var i = 0; i < MenuItems.Count; i++)
        {
            var momentOption = MenuItems[i];
            var before = "";
            var after = "";

            if (_selectedMenuItem == i)
            {
                ForegroundColor = ConsoleColor.Black;
                BackgroundColor = ConsoleColor.White;
                before = ">> ";
                after = " <<";
            }
            else
            {
                ForegroundColor = ConsoleColor.White;
                BackgroundColor = ConsoleColor.Black;
            }
            
            WriteLine($"{before}{momentOption}{after}");
        }
        ResetColor();
    }

    private void GetChosenIndex(ConsoleKey pressedKey)
    {
        if (pressedKey == ConsoleKey.UpArrow )
        {
            _selectedMenuItem--;
            if (_selectedMenuItem == -1)
            {
                _selectedMenuItem = MenuItems.Count - 1;
            }
        } else if (pressedKey == ConsoleKey.DownArrow)
        {
            _selectedMenuItem++;
            if (_selectedMenuItem == MenuItems.Count)
            {
                _selectedMenuItem = 0;
            }
        }
    }

    public string RunMenu()
    {
        var menuDone = false;
        var userChoice = "";
        do
        {
            if (userChoice != "-")
            {
                Clear();
                DisplayMenu(); 
            }

            var keyInfo = ReadKey(true);
            var pressedKey = keyInfo.Key;
            GetChosenIndex(pressedKey);

            if (MenuItems[_selectedMenuItem].MethodToRun != null && pressedKey == ConsoleKey.Enter)
            {
                Clear();
                userChoice = MenuItems[_selectedMenuItem].MethodToRun!();
            }

            if (MenuItems[_selectedMenuItem] == goBack && pressedKey == ConsoleKey.Enter)
            {
                menuDone = true;
            }

            if ((MenuItems[_selectedMenuItem] == backToMain && pressedKey == ConsoleKey.Enter || userChoice == backToMain.Title) && _level != EMenuLevel.Main)
            {
                return backToMain.Title;
            }

            if (MenuItems[_selectedMenuItem] == exit && pressedKey == ConsoleKey.Enter || userChoice == exit.Title) 
            {
                return exit.Title;
            }
            
            if (MenuItems[_selectedMenuItem].MethodToRun == null && pressedKey == ConsoleKey.Enter)
            {
                Clear();
                userChoice = MenuItems[_selectedMenuItem].ToString();
                menuDone = true;
            }

        } while (menuDone != true);

        return userChoice;
    }

}