using Nez;
using Microsoft.Xna.Framework.Input;

namespace Platformer.Player
{
    public class Controller
    {
        private VirtualButton _jumpInput;
        private VirtualIntegerAxis _xAxisInput;

        public void SetupInput()
        {
            _jumpInput = new VirtualButton();
            _jumpInput.Nodes.Add( new VirtualButton.KeyboardKey( Keys.Z ) );
            _jumpInput.Nodes.Add( new VirtualButton.GamePadButton( 0, Buttons.A ) );

            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add( new VirtualAxis.GamePadDpadLeftRight() );
            _xAxisInput.Nodes.Add( new VirtualAxis.GamePadLeftStickX() );
            _xAxisInput.Nodes.Add( new VirtualAxis.KeyboardKeys(
                VirtualInput.OverlapBehavior.TakeNewer,
                Keys.Left,
                Keys.Right ) );
        }
    }
}