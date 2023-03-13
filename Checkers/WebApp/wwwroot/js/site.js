let activeButtons = document.getElementsByClassName("active-button");
let activeCells = document.getElementsByClassName("active-cell");
let takeCells = document.getElementsByClassName("take");
let buttonNeedsToMove = document.getElementsByClassName("needsToMove");
let currentCell;
let currentActiveCell;

if (buttonNeedsToMove.length > 0) { activeButtons = buttonNeedsToMove; }

for (let i = 0; i < activeButtons.length; i++) {
    
    currentCell = activeButtons[i];
    
    currentCell.addEventListener("click", (e) => {
        
        const gameId = new URLSearchParams(window.location.search)
            .get('id');
        const x = e.target.parentNode.getAttribute('x');
        const y = e.target.parentNode.getAttribute('y');
        
        if (x != null && y != null) {
            window.location = '/CheckersGames/Play?id=' + gameId + '&pieceX=' + x
                + '&pieceY=' + y;   
        }
    });
}

if (takeCells.length > 0) { activeCells = takeCells }

for (let i = 0; i < activeCells.length; i++) {
    
    currentActiveCell = activeCells[i];
    
    currentActiveCell.addEventListener("click", async (e) =>
        {
            const urlSearchParams = new URLSearchParams(window.location.search);

            const gameId = urlSearchParams
                .get('id');
            const pieceX = urlSearchParams
                .get('pieceX');
            const pieceY = urlSearchParams
                .get('pieceY');

            if (gameId != null && pieceY != null && pieceX != null) {
                window.location = '/CheckersGames/Play?id=' + gameId + '&pieceX=' + pieceX + '&pieceY=' + pieceY + '&x='
                    + e.target.getAttribute('x') + '&y=' + e.target.getAttribute('y');
                history.pushState(null, null, '/CheckersGames/Play?id=' + gameId);
            }
        }
    )
}
