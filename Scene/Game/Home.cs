using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using TiledSharp;

namespace firewood;

public class Home : IScene {

    private GraphicsDevice _graphicsDevice;
    private ContentManager _contentManager;
    private SceneManager _sceneManager;

    private SpriteFont _pixelfont;

    private TmxMap _map;
    private List<Rectangle> _solidTiles;
    private Texture2D _tileset;
    private Texture2D _playerTexture;
    
    private Player _player;
    private Camera2D _camera;

    public List<Rectangle> LoadCollisionObject(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var solidTiles = new List<Rectangle>();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Collision")
            {
                foreach (var obj in objectGroup.Objects)
                {
                    if(obj.Width > 0 && obj.Height > 0)
                    {
                        var rect = new Rectangle(
                            (int)obj.X,
                            (int)obj.Y,
                            (int)obj.Width,
                            (int)obj.Height
                        );

                        solidTiles.Add(rect);
                    }
                }
            }
        }

        return solidTiles;
    }

    public Home(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _sceneManager = sceneManager;
        _contentManager = contentManager;
    }

    public void LoadContent()
    {
        _player = new Player(new Vector2(672, 864));
        _camera = new Camera2D(_graphicsDevice.Viewport);

        _tileset = _contentManager.Load<Texture2D>("home_tilemap");
        _solidTiles = new List<Rectangle>();
        _map = new TmxMap("Content/house.tmx");
        _solidTiles = LoadCollisionObject("Content/house.tmx");

        _pixelfont = _contentManager.Load<SpriteFont>("pixelfont");
        _playerTexture = _contentManager.Load<Texture2D>("player");
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 64, _map.Height * 64));
    }

    public void Draw(SpriteBatch spriteBatch)
    {

        int mapWidth = _map.Width;
        int mapHeight = _map.Height;

        int Width = _graphicsDevice.Viewport.Width;
        int Height = _graphicsDevice.Viewport.Height;

        _graphicsDevice.Clear(Color.Black);

        for(int layerIndex = 0; layerIndex < _map.Layers.Count; layerIndex++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                for(int x = 0; x < mapWidth; x++)
                {
                    int i = y * mapWidth + x;

                    if(i >= _map.Layers[layerIndex].Tiles.Count)
                    {
                        continue;
                    }

                    var tile = _map.Layers[layerIndex].Tiles[i];

                    if(tile.Gid == 0)
                    {
                        continue;
                    }

                    int tilesPerRow = _tileset.Width / _map.TileWidth;
                    int tileIndex = tile.Gid - 1;

                    int tileIndexX = tileIndex % tilesPerRow;
                    int tileIndexY = tileIndex / tilesPerRow;

                    Rectangle source = new Rectangle(
                        tileIndexX * _map.TileWidth,
                        tileIndexY * _map.TileHeight,
                        _map.TileWidth,
                        _map.TileHeight
                    );

                    Vector2 worldPosition = new Vector2(x * _map.TileWidth, y * _map.TileHeight);
                    Vector2 screenPosition = _camera.WorldToScreen(worldPosition);

                    spriteBatch.Draw(_tileset, screenPosition, source, Color.White);
                }
            }
        }

        _player.Draw(spriteBatch, _playerTexture, _camera);
    }
}