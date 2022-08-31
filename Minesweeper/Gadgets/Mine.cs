using LightningGL;
using static NuCore.SDL2.SDL;
using NuCore.Utilities;
using System.Drawing;
using System.Numerics;

namespace Minesweeper
{
    /// <summary>
    /// MineElement
    /// 
    /// It's a mine element
    /// </summary>
    public class Mine : Button
    {
        public Button? MineButton { get; set; }

        /// <summary>
        /// The type of the mine.
        /// 
        /// See <see cref="MineType"/> for further information.
        /// </summary>
        public MineType Type { get; set; }

        /// <summary>
        /// The position of the mine element within the minefield.
        /// </summary>
        public Vector2 MinePosition { get; set; }

        /// <summary>
        /// Used to determine if the area needs to return to being a mine or not.
        /// </summary>
        internal bool WasMine { get; set; }

        /// <summary>
        /// Used to determine if this mine has already been revealed.
        /// </summary>
        internal bool IsRevealed { get; set; }

        public Mine() : base()
        {
            Size = MineField.MineElementSize;
            Type = MineType.None;
            OnRender += ElementOnDraw;
            BackgroundColour = Color.Gray;
            HoverColour = Color.DarkGray;
            PressedColour = Color.DarkSlateGray;
            ForegroundColour = Color.White;
            Font = "Consolas 12pt";
            Filled = true;

            UIManager.AddElement(this);
        }

        public void ElementOnDraw(Window cWindow)
        {
            Render(cWindow);

            if (Type > MineType.None
                    && MineField.Sprites != null)
            {
                MineField.Sprites.Index = Convert.ToInt32(Type);
                MineField.Sprites.Position = Position;
                MineField.Sprites.Draw(SceneManager.MainWindow);
            }
        }

        
    }
}
