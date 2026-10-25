using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using TiledSharp;

namespace firewood;

public class Garden : IScene {
    
    private GraphicsDevice _graphicsDevice;
    private ContentManager _contentManager;
    private SceneManager _sceneManager;
    private MapManager _mapManager;
    private QuestManager _questManager;
    private InteractManager _interactManager;

    private SpriteFont _pixelfont; 
    
    private TmxMap _map;
    private Texture2D _playerTexture;

    private List<Rectangle> _solidTiles;

    private List<Rectangle> _wood1Tiles;
    private List<Rectangle> _wood2Tiles;
    private List<Rectangle> _wood3Tiles;
    private List<Rectangle> _wood4Tiles;
    private List<Rectangle> _wood5Tiles;

    private bool _isWood1 = true;
    private bool _isWood2 = true;
    private bool _isWood3 = true;
    private bool _isWood4 = true;
    private bool _isWood5 = true;

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
                foreach(var obj in objectGroup.Objects)
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

    public Garden(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _sceneManager = sceneManager;
        _contentManager = contentManager;
    }

    public void LoadContent()
    {
        _map = new TmxMap("Content/garden.tmx");

        var dict = new Dictionary<int, Texture2D>();

        foreach(var ts in _map.Tilesets)
        {
            string asset = Path.GetFileNameWithoutExtension(ts.Image.Source);
            dict[ts.FirstGid] = _contentManager.Load<Texture2D>(asset);
        }

        _mapManager = new MapManager(_map, dict);
        _questManager = new QuestManager();
        _interactManager = new InteractManager();

        _player = new Player(new Vector2(1984, 2240));
        _camera = new Camera2D(_graphicsDevice.Viewport);

        _solidTiles = LoadObject("Content/garden.tmx", "Collision");
        _wood1Tiles = LoadObject("Content/garden.tmx", "Wood1");
        _wood2Tiles = LoadObject("Content/garden.tmx", "Wood2");
        _wood3Tiles = LoadObject("Content/garden.tmx", "Wood3");
        _wood4Tiles = LoadObject("Content/garden.tmx", "Wood4");
        _wood5Tiles = LoadObject("Content/garden.tmx", "Wood5");

        _pixelfont = _contentManager.Load<SpriteFont>("pixelfont");
        _playerTexture = _contentManager.Load<Texture2D>("player");
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 64, _map.Height * 64));

        KeyboardState state = Keyboard.GetState();
        _interactManager.Update();

        GameData.previousState = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int mapWidth = _map.Width;
        int mapHeight = _map.Height;

        int Width = _graphicsDevice.Viewport.Width;
        int Height = _graphicsDevice.Viewport.Height;

        _graphicsDevice.Clear(Color.Black);

        float LayerDeep = 0.01f;

        foreach(var layer in _map.Layers)
        {
            _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            LayerDeep+=0.01f;
        }

        _player.Draw(spriteBatch, _playerTexture, _camera);
        _questManager.DrawQuest(GameData.Quests[GameData.QuestionNumber], spriteBatch, _graphicsDevice, _pixelfont);

    }

}