namespace SpinXEngine.Core.GameDesigner
{
    /// <summary>
    /// Implement <see cref="IWinCalculationStrategy"/> to calculates wins 
    /// based on horizontal lines with matching symbols
    /// </summary>
    public class LineWinStrategy : IWinCalculationStrategy
    {
        ///<inheritdoc/> 
        public string Name => "Line Win Calculation";

        ///<summary> 
        ///<inheritdoc/> <see cref="LineWinStrategy"/>
        ///</summary>
        public int Calculate(int[,] matrix)
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
    }
}
