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
            FontManager.LoadFont("consola", 12, "Consolas 12pt"); // load Consolas 12pt
            FontManager.LoadFont("consola", 36, "Consolas 36pt"); // load Consolas 12pt but bigger

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