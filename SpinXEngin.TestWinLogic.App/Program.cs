// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

namespace SpinXEngine.TestWinLogic.App;
class Program
{
    private static Player? m_player;
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Welcome to SpinXEngine, Enter You balance");
            if (!double.TryParse(Console.ReadLine(), out double balance) || balance < 0)
            {
                throw new InvalidDataException("Please Enter valid number");
            }

            Console.WriteLine("Thank you adding credits...");
            // Create a new player with the provided balance
            m_player = new Player(balance);

            while (true)
            {
                Console.WriteLine($"\n\nYou have {m_player.Balance} credits available. \nEnter your bet: ");
                double bet = double.Parse(Console.ReadLine() ?? "0");

                // Validate and update the player's balance before placing new bet.
                m_player.Deduct(bet);

                // Show spinner for 5 seconds
                ShowSpinner();

                var create = CreateMatrix(3, 5);
                double win = CalculateWin(create, bet);

                if (win > 0)
                    m_player.Credit(win);

                Console.WriteLine($"Your win is: {win}, you have {m_player.Balance:F2} credits");
                Console.WriteLine("Press Enter to play again");
                if (!Console.ReadKey(intercept: true).Key.Equals(ConsoleKey.Enter))
                {
                    Console.WriteLine("See you again, Bye for now!! ");
                    break;
                }

                Console.Clear();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static int[,] CreateMatrix(int rows, int cols)
    {
        var matrix = new int[rows, cols];
        Random random = new();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = random.Next(0, 9);
                Console.Write($"{matrix[i, j]} ");
            }
            Console.WriteLine();
        }

        return matrix;
    }

    static double CalculateWin(int[,] matrix, double bet)
    {
        int totalWin = 0;
        totalWin += CalculateLineWin(matrix);
        totalWin += CalculateTopRowZigzagWin(matrix);
        return totalWin * bet;
    }

    static int CalculateLineWin(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int totalWin = 0;

        for (int row = 0; row < rows; row++)
        {
            var firstSymbol = matrix[row, 0];
            int consecutiveCount = 1;

            for (int col = 1; col < cols; col++)
            {
                if (matrix[row, col] != firstSymbol)
                {
                    // Stop immediately if the sequence breaks
                    break;
                }
                consecutiveCount++;
            }

            // Calculate win if there are at least 3 consecutive symbols
            if (consecutiveCount >= 3)
            {
                totalWin += consecutiveCount * firstSymbol;
            }
        }

        return totalWin;
    }

    static int CalculateTopRowZigzagWin(int[,] matrix)
    {
        // Gets the dimensions of the matrix
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int totalWin = 0;

        // Set initial conditions for zigzag traversal, with starting at the top row
        // int row = 0;

        for (int startRow = 0; startRow < rows-2; startRow++)
        {
            bool movingDown = true;
            int? currentSymbol = null;
            int consecutiveCount = 0;
            int row = startRow;

            // Traverse the top row in a zigzag pattern
            for (int col = 0; col < cols; col++)
            {
                int value = matrix[row, col];

                if (col == 0)
                {
                    currentSymbol = value;
                    consecutiveCount = 1;
                }
                else if (value == currentSymbol)
                {
                    consecutiveCount++;
                }
                else
                {
                    // Stop immediately if the sequence breaks
                    break;
                }

                // Zigzag movement
                if (movingDown)
                {
                    if (row == rows - 1)
                    {
                        movingDown = false;
                        row--;
                    }
                    else
                    {
                        row++;
                    }
                }
                else
                {
                    if (row == 0)
                    {
                        movingDown = true;
                        row++;
                    }
                    else
                    {
                        row--;
                    }
                }
            }

            // Final evaluation
            if (consecutiveCount >= 3 && currentSymbol.HasValue)
            {
                totalWin += consecutiveCount * currentSymbol.Value;
            }
        }

        return totalWin;
    }

    public static void ShowSpinner(int durationMilliseconds = 5000)
    {
        Console.WriteLine("SPIN result...");

        var spinner = new[] { '|', '/', '-', '\\' };
        int counter = 0;
        var watch = Stopwatch.StartNew();
        while (watch.ElapsedMilliseconds < durationMilliseconds)
        {
            Console.Write($"\rLoading {spinner[counter++ % spinner.Length]}");
            Thread.Sleep(100); // adjust for speed
        }

        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
    }
}


