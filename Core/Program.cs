namespace Core;

class Program
{
    static void Main()
    {
        // Initialize the game
        Game game = new Game();

        // Roll the dice
        game.Dice.Value1 = 1;
        game.Dice.Value2 = 4;

        // Print the dice results
        int[] diceValues = game.Dice.GetDiceValues();
        Console.WriteLine($"Dice rolled: {string.Join(", ", diceValues)}");

        // Call AI to get the best sequence of moves with depth 2
        var ai = new AI.AI(game.CurrentPlayer, game.Board, game.Players);
        var bestMoveSequence = ai.GetBestMove(diceValues, 1);

        // Print the best move sequence
        Console.WriteLine("Best move sequence:");
        foreach (var move in bestMoveSequence)
        {
            Console.WriteLine($"Move from {move.From} to {move.To}");
        }
    }
}