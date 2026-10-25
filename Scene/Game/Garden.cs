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
    private List<Rectangle> _doorTiles;

    private List<Rectangle> _wood1Tiles;
    private bool _inWood1 = false;

    private List<Rectangle> _wood2Tiles;
    private bool _inWood2 = false;

    private List<Rectangle> _wood3Tiles;
    private bool _inWood3 = false;

    private List<Rectangle> _wood4Tiles;
    private bool _inWood4 = false;

    private List<Rectangle> _wood5Tiles;
    private bool _inWood5 = false;

    private float _timer = 240000;

    private bool _isWood1 = true;
    private bool _isWood2 = true;
    private bool _isWood3 = true;
    private bool _isWood4 = true;
    private bool _isWood5 = true;

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

        _doorTiles = LoadObject("Content/garden.tmx", "Home");

        _pixelfont = _contentManager.Load<SpriteFont>("pixelfont");
        _playerTexture = _contentManager.Load<Texture2D>("player");
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 64, _map.Height * 64));

        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(_timer >= 0)
        {
            _timer -= elapsed;
        } else {
            _sceneManager.ChangeScene("SecretEnding");
        }

        _inWood1 = false;
        _inWood2 = false;
        _inWood3 = false;
        _inWood4 = false;
        _inWood5 = false; 
    
        _isDoor = false;

        foreach(Rectangle wood in _wood1Tiles)
        {
            if(_player.Hitbox.Intersects(wood))
            {
                _inWood1 = true;
            }
        }

        foreach(Rectangle wood in _wood2Tiles)
        {
            if(_player.Hitbox.Intersects(wood))
            {
                _inWood2 = true;
            }
        }

        foreach(Rectangle wood in _wood3Tiles)
        {
            if(_player.Hitbox.Intersects(wood))
            {
                _inWood3 = true;
            }
        }

        foreach(Rectangle wood in _wood4Tiles)
        {
            if(_player.Hitbox.Intersects(wood))
            {
                _inWood4 = true;
            }
        }

        foreach(Rectangle wood in _wood5Tiles)
        {
            if(_player.Hitbox.Intersects(wood))
            {
                _inWood5 = true;
            }
        }

        foreach(Rectangle wood in _wood5Tiles)
        {
            if(_player.Hitbox.Intersects(wood))
            {
                _inWood5 = true;
            }
        }

        foreach(Rectangle door in _doorTiles)
        {
            if(_player.Hitbox.Intersects(door))
            {
                _isDoor = true;
            }
        }

        KeyboardState state = Keyboard.GetState();
        _interactManager.Update();

        if(_inWood1 && _isWood1 && _interactManager.Interact(Keys.E))
        {
            _isWood1 = false;
            GameData.QuestionNumber++;
        }

        if(_inWood2 && _isWood2 && _interactManager.Interact(Keys.E))
        {
            _isWood2 = false;
            GameData.QuestionNumber++;
        }

        if(_inWood3 && _isWood3 && _interactManager.Interact(Keys.E))
        {
            _isWood3 = false;
            GameData.QuestionNumber++;
        }

        if(_inWood4 && _isWood4 && _interactManager.Interact(Keys.E))
        {
            _isWood4 = false;
            GameData.QuestionNumber++;
        }

        if(_inWood5 && _isWood5 && _interactManager.Interact(Keys.E))
        {
            _isWood5 = false;
            GameData.QuestionNumber++;
        }

        if(_isDoor && _interactManager.Interact(Keys.E))
        {
            if(GameData.QuestionNumber == 7)
            {
                GameData.QuestionNumber++;
            }

            _sceneManager.ChangeScene("Home");
        }

        GameData.previousState = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int mapWidth = _map.Width;
        int mapHeight = _map.Height;

        int Width = _graphicsDevice.Viewport.Width;
        int Height = _graphicsDevice.Viewport.Height;

        Vector2 PickupM = _pixelfont.MeasureString("Press E to pickup firewood");
        Vector2 Pickup = new Vector2((Width / 2) - (PickupM.X / 4), ((Height / 4) * 3) - (PickupM.Y / 2));

        Vector2 EnterM = _pixelfont.MeasureString("Press E to go back to the house");
        Vector2 Enter = new Vector2((Width / 2) - (EnterM.X / 4), ((EnterM.Y / 4) * 3) - (PickupM.Y / 2));

        _graphicsDevice.Clear(Color.Black);

        float LayerDeep = 0.01f;

        foreach(var layer in _map.Layers)
        {
            if(layer.Name == "Wood1" && _isWood1)
            {
                _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            } else if(layer.Name == "Wood2" && _isWood2) {
                _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            } else if(layer.Name == "Wood3" && _isWood3) {
                _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            } else if(layer.Name == "Wood4" && _isWood4) {
                _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            } else if(layer.Name == "Wood5" && _isWood5) {
                _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            } else if(layer.Name == "ITEMS") {
                _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            } else if(layer.Name == "Tile Layer 1") {
                _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            }
                LayerDeep+= 0.01f;
        }

        _player.Draw(spriteBatch, _playerTexture, _camera);
        _questManager.DrawQuest(GameData.Quests[GameData.QuestionNumber], spriteBatch, _graphicsDevice, _pixelfont);

        if((_inWood1 && _isWood1) || (_inWood2 && _isWood2) || (_inWood3 && _isWood3) || (_inWood4 && _isWood4) || (_inWood5 && _isWood5))
        {
                spriteBatch.DrawString(_pixelfont, "Press E to pickup firewood", Pickup, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f); 
        }

        if(_isDoor)
        {
            spriteBatch.DrawString(_pixelfont, "Press E to go back to the house", Pickup, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f); 
        }
    }

}