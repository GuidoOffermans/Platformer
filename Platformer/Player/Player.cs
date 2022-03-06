using System.Collections.Generic;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Tiled;


namespace Platformer.Player
{
    public class Player : Component, IUpdatable
    {
        public float MoveSpeed = 150;
        public float Gravity = 1000;
        public float JumpHeight = 16 * 2;

        private List<Sprite> _sprites;
        private SpriteAnimator _animator;

        private Vector2 _velocity;
        private BoxCollider _boxCollider;
        private TiledMapMover _mover;
        TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();
        ColliderTriggerHelper _triggerHelper;

        private VirtualButton _jumpInput;
        private VirtualIntegerAxis _xAxisInput;

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture( "Content/adventurer.png" );
            _sprites = Sprite.SpritesFromAtlas( texture, 50, 37 );

            _boxCollider = Entity.GetComponent<BoxCollider>();
            _mover = Entity.GetComponent<TiledMapMover>();
            
            _triggerHelper = new ColliderTriggerHelper(Entity);

            SetupAnimations();
            SetupInput();
        }

        public override void OnRemovedFromEntity()
        {
            // deregister virtual input
            _jumpInput.Deregister();
            _xAxisInput.Deregister();
        }

        void SetupAnimations()
        {
            _animator = Entity.AddComponent( new SpriteAnimator( _sprites[ 0 ] ) );
            _animator.AddAnimation( "Walk", new[]
            {
                _sprites[ 0 ],
                _sprites[ 1 ],
                _sprites[ 2 ],
                _sprites[ 3 ],
                _sprites[ 4 ],
                _sprites[ 5 ]
            } );

            _animator.AddAnimation( "Run", new[]
            {
                _sprites[ 8 + 0 ],
                _sprites[ 8 + 1 ],
                _sprites[ 8 + 2 ],
                _sprites[ 8 + 3 ],
                _sprites[ 8 + 4 ],
                _sprites[ 8 + 5 ],
            } );

            _animator.AddAnimation( "Idle", new[]
            {
                _sprites[ 0 ],
                _sprites[ 1 ],
                _sprites[ 2 ],
                _sprites[ 3 ]
            } );
            
            _animator.AddAnimation( "Falling", new[]
            {
                _sprites[ 22 + 0 ],
                _sprites[ 22 + 1 ],
            } );
            
            _animator.AddAnimation( "Jumping", new[]
            {
                _sprites[ 14 + 0 ],
                _sprites[ 14 + 1 ],
                _sprites[ 14 + 2 ],
                _sprites[ 14 + 3 ]
            } );
        }

        void SetupInput()
        {
            // setup input for jumping. we will allow z on the keyboard or a on the gamepad
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

        public void Update()
        {
            // handle movement and animations
            var moveDir = new Vector2( _xAxisInput.Value, 0 );
            string animation = null;

            if ( moveDir.X < 0 )
            {
                if ( _collisionState.Below )
                    animation = "Run";
                _animator.FlipX = true;
                _velocity.X = -MoveSpeed;
            }
            else if ( moveDir.X > 0 )
            {
                if ( _collisionState.Below )
                    animation = "Run";
                _animator.FlipX = false;
                _velocity.X = MoveSpeed;
            }
            else
            {
                _velocity.X = 0;
                if ( _collisionState.Below )
                    animation = "Idle";
            }

            if ( _collisionState.Below && _jumpInput.IsPressed )
            {
                animation = "Jumping";
                _velocity.Y = -Mathf.Sqrt( 2f * JumpHeight * Gravity );
            }

            if ( !_collisionState.Below && _velocity.Y > 0 )
                animation = "Falling";

            // apply gravity
            _velocity.Y += Gravity * Time.DeltaTime;

            // move
            _mover.Move( _velocity * Time.DeltaTime, _boxCollider, _collisionState );

            // Update the TriggerHelper. This will check if our collider intersects with a
            // trigger-collider and call ITriggerListener if necessary.
            _triggerHelper.Update();

            if ( _collisionState.Below )
                _velocity.Y = 0;

            if ( animation != null && !_animator.IsAnimationActive( animation ) )
                _animator.Play( animation );
        }
    }
}