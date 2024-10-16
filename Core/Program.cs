using Core;

class Program
{
    static void Main(string[] args)
    {
        // Step 1: Initialize the game
        Game game = new Game();

        // Step 3: Manually set dice values (e.g., dice roll of 3 and 5)
        game.Dice.Value1 = 3;  // Assuming you have a method to manually set dice values for testing
        game.Dice.Value2 = 5;

        // Step 4: Perform a move (move from point 1 to point 4 using dice value of 3)
        bool moveSuccessful = game.MakeMove(1, 4);  // Move from index 1 to 4 using one of the dice values

        // Output the result of the move
        if (moveSuccessful)
        {
            Console.WriteLine("Move was successful!");
        }
        else
        {
            Console.WriteLine("Move was invalid.");
        }
    }
}