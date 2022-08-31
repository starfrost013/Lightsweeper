using LightningGL;

namespace Minesweeper
{
    public class MainScene : Scene
    {
        public MineField Field { get; set; }

        public MainScene()
        {
            Field = new MineField();
        }

        public override void Start()
        {
            FontManager.LoadFont("comic", 12, "Comic Sans"); // load comic sans
            FontManager.LoadFont("comic", 36, "Comic Sans 36pt"); // load comic sans but bigger
        }

        public override void Shutdown()
        {

        }

        public override void SwitchTo(Scene oldScene)
        {

        }

        public override void SwitchAway(Scene newScene)
        {

        }

        public override void Render(Window cWindow)
        {
            Field.OnRender(cWindow);
        }
    }
}