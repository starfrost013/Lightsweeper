﻿using static NuCore.SDL2.SDL;
using NuCore.Utilities;
using LightningGL;
using System.Drawing;
using System.Numerics;

namespace Minesweeper
{
    /// <summary>
    /// MineField
    /// 
    /// It's the minefield
    /// </summary>
    public class MineField : Gadget
    {
        public List<Mine> Mines { get; set; }

        internal static Vector2 MineElementSize = new(32, 32);

        internal static Vector2 MineFieldSize = new(9, 9);

        internal static int MineCount = 10;

        public static TextureAtlas? Sprites { get; set; }

        private static Random Random = new Random();

        public GameState State { get; set; }

        public MineField()
        {
            Mines = new List<Mine>();
            OnRender += Render;
            OnMousePressed += MineMousePressed;
            OnKeyPressed += MineKeyPressed;
            State = GameState.Init;
        }

        public void Load(Window cWindow)
        {
            Sprites = new TextureAtlas(cWindow, new(32, 32), new(16, 16));
            Sprites.Path = "Content/Sprites.png";
            NCLogging.Log("Loading minefield...");
            Sprites.Load(cWindow);

            UIManager.AddElement(this);
        }

        public void Create()
        {
            // hack so that we receive mousebutton events
            Size = new(MineElementSize.X * MineFieldSize.X, MineElementSize.Y * MineFieldSize.Y);
            //placeholder so we appear in the middle of the screen
            Position = new(MineElementSize.X, MineElementSize.Y);

            for (int y = 0; y < MineFieldSize.Y; y++)
            {
                for (int x = 0; x < MineFieldSize.X; x++)
                {
                    Mine mineElement = new();
                    mineElement.Position = new(MineElementSize.X * (x + 1), MineElementSize.Y * (y + 1));
                    mineElement.MinePosition = new(x, y);
                    mineElement.Type = MineType.None;

                    Mines.Add(mineElement);
                }
            }

            for (int mineId = 0; mineId < MineCount; mineId++)
            {
                int x = Random.Next(0, Convert.ToInt32(MineFieldSize.X));
                int y = Random.Next(0, Convert.ToInt32(MineFieldSize.Y));

                Mine? mine = GetMine(x, y);

                if (mine == null) _ = new NCException($"Attempted to place invalid mine at position {x},{y} (range is 0,0 to {MineFieldSize.X},{MineFieldSize.Y}", 1000, $"GetMine({x}, {y} returned null", NCExceptionSeverity.FatalError);

                NCLogging.Log($"Spawned mine at {x},{y}");

                if (mine != null
                    && mine.Type != MineType.HiddenMine)
                {
                    mine.Type = MineType.HiddenMine;
                }
                else
                {
                    // pick a new position
                    mineId--;
                }
                
            }
        }

        public Mine? GetMine(int x, int y)
        {
            foreach (Mine element in Mines)
            {
                if (element.MinePosition.X == x
                    && element.MinePosition.Y == y) return element;
            }

            return null;
        }

        public void Render(Window cWindow)
        {
            switch (State)
            {
                // clear the field
                case GameState.Init:
                    CleanupUiElements();
                    Load(SceneManager.MainWindow);
                    Create();
                    State = GameState.Playing;
                    break;
                case GameState.Playing:
                    // check if the player has won
                    break;
                case GameState.Win:
                    // these will be cleaned up later
                    foreach (Mine mine in Mines) UIManager.RemoveElement(mine);
                    Mines.Clear();
                    // you do not HAVE to call this method, but we need the size of the ACTUAL string - not the localisation string name
                    // you can just call fontmanager::drawtext with the #[ syntax
                    string winText = LocalisationManager.ProcessString("#[STRING_YOU_WIN]");
                    Vector2 textSize = FontManager.GetTextSize("Comic Sans 36pt", winText);
                    FontManager.DrawText(SceneManager.MainWindow, "You Win!", "Comic Sans 36pt", new(GlobalSettings.ResolutionX / 2 - (textSize.X / 2),
                        GlobalSettings.ResolutionY / 2 - (textSize.Y / 2)), Color.White, Color.Red, NuCore.SDL2.SDL_ttf.TTF_FontStyle.Underline);
                    break;
                case GameState.Lose:
                    break;

            }

            // render all positions
            foreach (Mine element in Mines) element.OnRender(cWindow);
        }

        private void CleanupUiElements(bool alsoRemoveField = true)
        {
            if (Mines.Count != 0) // clear everything
            {
                foreach (Mine mine in Mines) UIManager.RemoveElement(mine);

                Mines.Clear();

                if (alsoRemoveField) UIManager.RemoveElement(this);
            }
        }

        /// <summary>
        /// Determines if the player has won the game or not.
        /// </summary>
        /// <returns>Returns a boolean value determining if the player has won the game or not.</returns>
        private bool HasPlayerWon()
        {
            foreach (Mine mine in Mines)
            {
                if (!mine.IsRevealed) return false;

                if (mine.Type == MineType.HiddenMine) return false;

                // can't win with question marks!
                if (mine.Type == MineType.QuestionMark) return false;
            }

            return true;
        }

        public void MineMousePressed(MouseButton button)
        {
            // main mouse press handler

            // temporary
            if (State != GameState.Playing)
            {
                State = GameState.Init;
                return;
            }

            base.MousePressed(button);

            // we always start 1 MineElementSize from the left edge of the screen for padding and UI
            int minePosX = Convert.ToInt32(Math.Floor(button.Position.X / MineElementSize.X)) - 1; 
            int minePosY = Convert.ToInt32(Math.Floor(button.Position.Y / MineElementSize.Y)) - 1;

            Mine? mine = GetMine(minePosX, minePosY);

            if (mine != null)
            {
                switch (button.Button)
                {
                    case SDL_MouseButton.Left:
                        if (mine.IsRevealed) break;
    
                        switch (mine.Type)
                        {
                            case MineType.HiddenMine: // you lose

                                State = GameState.Lose;
                                mine.Type = MineType.Mine;

                                // do various thigngs to the elements
                                foreach (Mine element in Mines)
                                {
                                    switch (element.Type)
                                    {
                                        case MineType.Mine:
                                            element.BackgroundColour = Color.Red; // the one that killed you
                                            break;
                                        case MineType.HiddenMine:
                                            element.Type = MineType.Mine;
                                            element.BackgroundColour = Color.Yellow; // the ones you missed
                                            break;
                                        case MineType.Flag:
                                            if (!element.WasMine) // if you duffed up and marked something that wasn't a mine...
                                            {
                                                element.Type = MineType.NotAMine;
                                            }
                                            else
                                            {
                                                element.BackgroundColour = Color.Green; // the ones you DID get
                                            }
                                            break;
                                    }
                                }

                                break;
                            case MineType.None:
                                FloodFill(minePosX, minePosY);
                                break;
                            default: // do nothing
                                break;

                        }

                        mine.IsRevealed = true; 

                        break;
                    case SDL_MouseButton.Right:
                        switch (mine.Type)
                        {
                            default:
                                mine.WasMine = (mine.Type == MineType.HiddenMine);
                                mine.Type = MineType.Flag;
                                break;
                            case MineType.Flag:
                                mine.Type = MineType.QuestionMark;
                                break;
                            case MineType.Mine: // can't do this with a mine. duh.
                                break;
                            case MineType.QuestionMark:
                                mine.Type = mine.WasMine ? MineType.HiddenMine : MineType.None;
                                break;

                        }
                        break;
                }

                if (HasPlayerWon()) State = GameState.Win;
            }
        }

        public void MineKeyPressed(Key key)
        {
            switch (State)
            {
                case GameState.Lose:
                case GameState.Win:
                    State = GameState.Init;
                    break;
            }
            
        }
        private void FloodFill(int initialX, int initialY)
        {
            Mine? mine = GetMine(initialX, initialY);

            if (mine != null)
            {
                if (mine.Type == MineType.None
                    && !mine.IsRevealed)
                {
                    int numberOfNearbyMines = 0;

                    // figure out how many mines are near this mine 
                    for (float y = mine.MinePosition.Y - 1; y <= mine.MinePosition.Y + 1; y++)
                    {
                        for (float x = mine.MinePosition.X - 1; x <= mine.MinePosition.X + 1; x++)
                        {
                            if (x != mine.Position.X
                                && y != mine.Position.Y)
                            {
                                Mine? nearbyMine = GetMine((int)x, (int)y);
                                if (nearbyMine != null
                                    && nearbyMine.Type == MineType.HiddenMine) numberOfNearbyMines++;
                            }
                        }
                    }

                    if (numberOfNearbyMines > 0) mine.Text = numberOfNearbyMines.ToString();

                    mine.IsRevealed = true;
                    mine.BackgroundColour = Color.LightGray;

                    // set the text colour based on the number of nearby mines
                    switch (numberOfNearbyMines)
                    {
                        case 1:
                            mine.ForegroundColour = Color.Blue;
                            break;
                        case 2:
                            mine.ForegroundColour = Color.Green;
                            break;
                        case 3:
                            mine.ForegroundColour = Color.Gold;
                            break;
                        case 4:
                            mine.ForegroundColour = Color.Purple;
                            break;
                        case 5:
                            mine.ForegroundColour = Color.Indigo;
                            break;
                        case 6:
                            mine.ForegroundColour = Color.Red;
                            break;
                        case 7:
                            mine.ForegroundColour = Color.Maroon;
                            break;
                        case 8:
                            mine.ForegroundColour = Color.Black;
                            break;

                    }

                    // don't do this if there are any mines nearby so the user doesn't instantly know where all the mines are
                    if (numberOfNearbyMines == 0) 
                    {
                        FloodFill(initialX - 1, initialY - 1);
                        FloodFill(initialX - 1, initialY);
                        FloodFill(initialX - 1, initialY + 1);
                        FloodFill(initialX, initialY - 1);
                        FloodFill(initialX, initialY + 1);
                        FloodFill(initialX + 1, initialY - 1);
                        FloodFill(initialX + 1, initialY);
                        FloodFill(initialX + 1, initialY + 1);
                    }
                }
            }
        }
    }
}
