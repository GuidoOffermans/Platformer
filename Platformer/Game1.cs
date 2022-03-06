using Nez;
using Nez.ImGuiTools;

namespace Platformer
{
    public class Game1 : Core
    {
        protected override void Initialize()
        {
           base.Initialize();
           
           var imGuiManager = new ImGuiManager();
           RegisterGlobalManager( imGuiManager );
           
           IsMouseVisible = true;
           DebugRenderEnabled = true;
           Window.AllowUserResizing = true;
           
           imGuiManager.SetEnabled(true);

           Scene = new Scenes.MainScene();
        }
    }
}