using Core;

Game game = new Game("Alice", "Bob");

// Roll the dice to begin the turn
game.RollDice();

// Try to make a move from point 1 to point 3 (assuming valid move)
bool moveSuccess = game.MakeMove(1, 3);

// If the move is successful, proceed with the next steps, e.g., bear off if eligible
if (game.CanBearOff())
{
    game.BearOffChecker(3);
}
    
// End the player's turn
game.EndTurn();