namespace SpinXEngine.Core.GameDesigner
{
    /// <summary>
    /// Interface for win calculation strategies
    /// </summary>
    public interface IWinCalculationStrategy
    {
        /// <summary>
        /// Calculates the win multiplier based on pattern
        /// </summary>
        /// <param name="matrix">The symbol matrix representing the reels</param>
        /// <returns>The win multiplier (to be multiplied by the bet amount)</returns>
        int Calculate(int[,] matrix);

        /// <summary>
        /// Gets the name of this win calculation strategy
        /// </summary>
        string Name { get; }
    }
}
