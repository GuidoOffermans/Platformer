using Nez;
using Nez.Tiled;
using Microsoft.Xna.Framework;

namespace Platformer.Scenes
{
    public class MainScene : BaseScene
    {
        public MainScene()
        {
            base.Initialize();
        }

        public override void Initialize()
        {
            SetDesignResolution( 640, 360, SceneResolutionPolicy.ShowAllPixelPerfect );
            Screen.SetSize( 640 * 3, 360 * 3 );

            var map = Content.LoadTiledMap( "Content/test.tmx" );
            var playerSpawn = map.GetObjectGroup( "objects" ).Objects[ "spawn" ];
            var playerSpawnPosition = new Vector2( playerSpawn.X, playerSpawn.Y );

            var tiledEntity = CreateEntity( "tiled-map" );
            tiledEntity.AddComponent( new TiledMapRenderer( map, "main" ) );

            // var topLeft = new Vector2( map.TileWidth, map.TileWidth );
            // var bottomRight = new Vector2( map.TileWidth * ( map.Width - 1 ),
            //     map.TileWidth * ( map.Height - 1 ) );
            // tiledEntity.AddComponent( new CameraBounds( topLeft, bottomRight ) );


            var playerEntity = CreateEntity( "player", playerSpawnPosition );
            playerEntity.AddComponent( new Player.Player() );
            playerEntity.AddComponent( new BoxCollider( -8, -16, 16, 32 ) );
            playerEntity.AddComponent( new TiledMapMover( map.GetLayer<TmxLayer>( "main" ) ) );


            Camera.Entity.AddComponent( new FollowCamera( playerEntity ) );
        }
    }
}