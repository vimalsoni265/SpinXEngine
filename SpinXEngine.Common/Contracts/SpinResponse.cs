namespace SpinXEngine.Common.Contracts
{
    /// <summary>
    /// Represents the result of a spin operation
    /// </summary>
    public record SpinResponse
    {
        /// <summary>
        /// Multi-dimensional array representing the reel symbols after the spin
        /// Each element is an integer representing a specific symbol
        /// </summary>
        public string ReelSymbols { get; init; } = string.Empty;

        /// <summary>
        /// The amount won by the player for this spin
        /// </summary>
        public decimal Win { get; init; }

        /// <summary>
        /// The player's updated balance after the spin
        /// </summary>
        public decimal CurrentBalance { get; init; }
    }
}
