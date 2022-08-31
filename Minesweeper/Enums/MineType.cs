using System;

namespace Minesweeper
{
    /// <summary>
    /// ElementType
    /// 
    /// Enumerates the valid element types.
    /// </summary>
    public enum MineType : int
    {
        HiddenMine = -2,

        None = -1,

        Mine = 0,

        QuestionMark = 1,

        TrippedMine = 2,

        NotAMine = 3,

        Flag = 4
    }
}
