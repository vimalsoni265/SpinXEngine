using System.Text;

namespace SpinXEngine.Core.GameDesigner
{
    /// <summary>
    /// Provides functionality for designing and calculating wins for slot games
    /// </summary>
    public class SpinGame
    {
        private readonly List<IWinCalculationStrategy> m_winStrategies = [];
        private readonly Random m_random = new();
        private int[,]? m_currentMatrix;

        /// <summary>
        /// Creates a new GameDesigner with default win calculation strategies
        /// </summary>
        public SpinGame()
        {
            // Register the default strategies
            RegisterWinStrategy(new LineWinStrategy());
            RegisterWinStrategy(new ZigzagWinStrategy());
        }

        /// <summary>
        /// Registers a new win calculation strategy
        /// </summary>
        /// <param name="strategy">The strategy to register</param>
        public void RegisterWinStrategy(IWinCalculationStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            m_winStrategies.Add(strategy);
        }

        /// <summary>
        /// Clears all registered win calculation strategies
        /// </summary>
        public void ClearWinStrategies()
        {
            m_winStrategies.Clear();
        }

        /// <summary>
        /// Generates a random symbol matrix for the reels
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        /// <returns>A matrix of symbols</returns>
        public int[,] GenerateReelSymbols(int rows, int cols)
        {
            if (rows <= 0)
                throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be greater than zero.");

            if (cols <= 0)
                throw new ArgumentOutOfRangeException(nameof(cols), "Columns must be greater than zero.");

            m_currentMatrix = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    m_currentMatrix[i, j] = m_random.Next(0, 9);
                }
            }

            return m_currentMatrix;
        }

        /// <summary>
        /// Spins the reels and calculates the winnings based on the current reel matrix and the specified bet amount.
        /// </summary>
        /// <param name="bet">The amount of money wagered on the spin. Must be a positive value.</param>
        /// <returns>The total winnings as a decimal value. Returns 0 if no winnings are achieved.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the reel symbols have not been generated. Ensure that <see cref="GenerateReelSymbols"/> is called
        /// before invoking this method.</exception>
        public decimal Spin(decimal bet)
        {
            if (m_currentMatrix == null)
                throw new InvalidOperationException("No symbols have been generated. Call DesignReelSymbols first.");

            if (bet <= 0)
                throw new ArgumentOutOfRangeException(nameof(bet), "Bet amount must be greater than zero.");

            // Make a local copy to ensure thread safety
            var matrix = m_currentMatrix;
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            // Copy strategies to a local variable to prevent issues if collection changes during enumeration
            var strategies = m_winStrategies.ToList();

            int totalWinMultiplier = 0;

            // Apply each registered strategy
            foreach (var strategy in strategies)
            {
                int winMultiplier = strategy.Calculate(matrix);
                totalWinMultiplier += winMultiplier;
            }

            return totalWinMultiplier * bet;
        }

        /// <summary>
        /// Calculates detailed win amounts using the current matrix
        /// </summary>
        /// <param name="bet">The bet amount</param>
        /// <returns>A dictionary containing the win amount for each strategy</returns>
        public Dictionary<string, decimal> SpinWithDetailedWin(decimal bet)
        {
            if (m_currentMatrix == null)
                throw new InvalidOperationException("No symbols have been generated. Call DesignReelSymbols first.");

            // Make a local copy to ensure thread safety
            var matrix = m_currentMatrix;

            if (bet <= 0)
                throw new ArgumentOutOfRangeException(nameof(bet), "Bet amount must be greater than zero.");

            // Copy strategies to a local variable to prevent issues if collection changes during enumeration
            var strategies = m_winStrategies.ToList();

            var result = new Dictionary<string, decimal>();

            foreach (var strategy in strategies)
            {
                int winMultiplier = strategy.Calculate(matrix);
                result[strategy.Name] = winMultiplier * bet;
            }

            return result;
        }

        /// <summary>
        /// Converts a multidimensional array to a JSON-like string representation
        /// </summary>
        /// <param name="matrix">The matrix to convert</param>
        /// <returns>A string representation of the matrix</returns>
        public string ConvertMatrixToString()
        {
            if (m_currentMatrix == null)
                return string.Empty;

            // Make a local copy to ensure thread safety
            var matrix = m_currentMatrix;
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < rows; i++)
            {
                sb.Append('[');
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(matrix[i, j]);
                    if (j < cols - 1)
                        sb.Append(',');
                }
                sb.Append(']');

                if (i < rows - 1)
                    sb.Append(',');
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
