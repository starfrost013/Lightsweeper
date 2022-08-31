using System;

namespace Minesweeper
{
    /// <summary>
    /// GameState
    /// 
    /// Enumerates the possible states of the game.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game is initialising.
        /// </summary>
        Init = 0,

        /// <summary>
        /// You are playing the game.
        /// </summary>
        Playing = 1,
        
        /// <summary>
        /// There are no mines left and the player has won the game.
        /// </summary>
        Win = 2,

        /// <summary>
        /// You hit a mine.
        /// </summary>
        Lose = 3
    }
}
