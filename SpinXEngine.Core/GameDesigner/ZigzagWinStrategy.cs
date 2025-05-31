namespace SpinXEngine.Core.GameDesigner
{
    /// <summary>
    /// Implement <see cref="IWinCalculationStrategy"/> to calculates wins 
    /// based on zigzag patterns with matching symbols
    /// </summary>
    public class ZigzagWinStrategy : IWinCalculationStrategy
    {
        #region Private Members
        private const string m_name = "Zigzag Win";
        #endregion

        #region Propperties

        public string Name => m_name;

        #endregion

        #region Public Methods
        public int Calculate(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int totalWin = 0;

            for (int startRow = 0; startRow < rows - 1; startRow++)
            {
                bool movingDown = true;
                int? currentSymbol = null;
                int consecutiveCount = 0;
                int row = startRow;

                for (int col = 0; col < cols; col++)
                {
                    // Skip if row is out of bounds
                    if (row < 0 || row >= rows)
                        break;

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
        
        #endregion
    }
}
