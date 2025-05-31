namespace SpinXEngine.Repository.Models
{
    /// <summary>
    /// Represents the configuration settings for a game.
    /// </summary>
    public class GameSetting
    {
        /// <summary>
        /// Gets or sets the number of rows in the reel.
        /// </summary>
        public int ReelRows { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the reel.
        /// </summary>
        public int ReelColumns { get; set; }
    }
}
