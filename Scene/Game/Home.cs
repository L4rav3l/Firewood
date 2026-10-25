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
    private Texture2D _tileset;
    private Texture2D _playerTexture;

    private List<Rectangle> _solidTiles;
    private List<Rectangle> _doorTiles;
    private List<Rectangle> _bedTiles;
    private List<Rectangle> _furnaceTiles;
    private List<Rectangle> _chestTiles;

    private bool _isFurnace = false;
    private bool _isBed = false;
    private bool _isChest = false;
    private bool _isDoor = false;

    private Player _player;
    private Camera2D _camera;

    public List<Rectangle> LoadObject(string mapFilePath, string name)
    {
        var map = new TmxMap(mapFilePath);
        var solidTiles = new List<Rectangle>();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == name)
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
        _map = new TmxMap("Content/house.tmx");

        _solidTiles = new List<Rectangle>();
        _chestTiles = new List<Rectangle>();

        _solidTiles = LoadObject("Content/house.tmx", "Collision");
        _chestTiles = LoadObject("Content/house.tmx", "Chest");
        _bedTiles = LoadObject("Content/house.tmx", "Bed");
        _doorTiles = LoadObject("Content/house.tmx", "Door");
        _furnaceTiles = LoadObject("Content/house.tmx", "Furnace");

        _pixelfont = _contentManager.Load<SpriteFont>("pixelfont");
        _playerTexture = _contentManager.Load<Texture2D>("player");
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 64, _map.Height * 64));

        _isFurnace = false;
        _isBed = false;
        _isChest = false;
        _isDoor = false;

        foreach(Rectangle furnace in _furnaceTiles)
        {
            if(_player.Hitbox.Intersects(furnace))
            {
                _isFurnace = true;
            }
        }

        foreach(Rectangle bed in _bedTiles)
        {
            if(_player.Hitbox.Intersects(bed))
            {
                _isBed = true;
            }
        }

        foreach(Rectangle chest in _chestTiles)
        {
            if(_player.Hitbox.Intersects(chest))
            {
                _isChest = true;
            }
        }

        foreach(Rectangle door in _doorTiles)
        {
            if(_player.Hitbox.Intersects(door))
            {
                _isDoor = true;
            }
        }

    }

    public void Draw(SpriteBatch spriteBatch)
    {

        int mapWidth = _map.Width;
        int mapHeight = _map.Height;

        int Width = _graphicsDevice.Viewport.Width;
        int Height = _graphicsDevice.Viewport.Height;

        Vector2 FurnaceCheckM = _pixelfont.MeasureString("Press E to interact Furnace");
        Vector2 FurnacePosition = new Vector2((Width / 2) - (FurnaceCheckM.X / 4), ((Height / 4) * 3) - (FurnaceCheckM.Y / 2));

        Vector2 ChestCheckM = _pixelfont.MeasureString("Press E to interact Chest");
        Vector2 ChestPosition = new Vector2((Width / 2) - (ChestCheckM.X / 4), ((Height / 4) * 3) - (ChestCheckM.Y / 2));

        Vector2 DoorCheckM = _pixelfont.MeasureString("Press E to interact Door");
        Vector2 DoorPosition = new Vector2((Width / 2) - (DoorCheckM.X / 4), ((Height / 4) * 3) - (DoorCheckM.Y / 2));

        Vector2 BedCheckM = _pixelfont.MeasureString("Press E to interact Bed");
        Vector2 BedPosition = new Vector2((Width / 2) - (BedCheckM.X / 4), ((Height / 4) * 3) - (BedCheckM.Y / 2));

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

        if(_isFurnace)
        {
            spriteBatch.DrawString(_pixelfont, "Press E to interact Furnace", FurnacePosition, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f); 
        }

        if(_isBed)
        {
            spriteBatch.DrawString(_pixelfont, "Press E to interact Bed", BedPosition, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f); 
        }

        if(_isChest)
        {
            spriteBatch.DrawString(_pixelfont, "Press E to interact Chest", ChestPosition, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f); 
        }

        if(_isDoor)
        {
            spriteBatch.DrawString(_pixelfont, "Press E to interact Door", DoorPosition, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f); 
        }

        _player.Draw(spriteBatch, _playerTexture, _camera);
    }
}