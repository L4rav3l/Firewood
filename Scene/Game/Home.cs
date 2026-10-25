using System;
using System.IO;
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
    private MapManager _mapManager;
    private QuestManager _questManager;
    private InteractManager _interactManager;

    private SpriteFont _pixelfont;
    private Song _yawn;

    private TmxMap _map;
    private Texture2D _playerTexture;

    private List<Rectangle> _solidTiles;
    private List<Rectangle> _doorTiles;
    private List<Rectangle> _bedTiles;
    private List<Rectangle> _furnaceTiles;
    private List<Rectangle> _chestTiles;

    private int _drop = 0;

    private bool _isFurnace = false;
    private bool _isBed = false;
    private bool _isChest = false;
    private bool _isDoor = false;
    private bool _isTurned = false;
    private bool _startDialogues = true;
    private bool _doorDialogues = false;
    private bool _dialogues1 = false;
    private bool _dialogues2 = false;

    private Player _player;
    private Camera2D _camera;

    private Texture2D _pixelTexture;

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
        _map = new TmxMap("Content/house.tmx");

        var dict = new Dictionary<int, Texture2D>();

        foreach (var ts in _map.Tilesets)
        {
            string asset = Path.GetFileNameWithoutExtension(ts.Image.Source);
            dict[ts.FirstGid] = _contentManager.Load<Texture2D>(asset);
        }

        _mapManager = new MapManager(_map, dict);
        _questManager = new QuestManager();
        _interactManager = new InteractManager();

        _player = new Player(new Vector2(672, 864));
        _camera = new Camera2D(_graphicsDevice.Viewport);

        _solidTiles = new List<Rectangle>();
        _chestTiles = new List<Rectangle>();

        _solidTiles = LoadObject("Content/house.tmx", "Collision");
        _chestTiles = LoadObject("Content/house.tmx", "Chest");
        _bedTiles = LoadObject("Content/house.tmx", "Bed");
        _doorTiles = LoadObject("Content/house.tmx", "Door");
        _furnaceTiles = LoadObject("Content/house.tmx", "Furnace");

        _pixelfont = _contentManager.Load<SpriteFont>("pixelfont");
        _yawn = _contentManager.Load<Song>("yawn");
        _playerTexture = _contentManager.Load<Texture2D>("player");

        _pixelTexture = new Texture2D(_graphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        GameData.Move = false;
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _solidTiles, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 64, _map.Height * 64));

        _isFurnace = false;
        _isBed = false;
        _isChest = false;
        _isDoor = false;

        KeyboardState state = Keyboard.GetState();
        _interactManager.Update();

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

        if(_isBed && GameData.QuestionNumber == 0 && _interactManager.Interact(Keys.E) && _isTurned)
        {
            _sceneManager.ChangeScene("SecretEnding");
        }

        if(_dialogues1 && _interactManager.Interact(Keys.E))
        {
            GameData.Move = true;
            _isTurned = true;
            _dialogues1 = false;
        }

        if(_isFurnace && GameData.QuestionNumber == 0 && _interactManager.Interact(Keys.E) && _isTurned)
        {
            GameData.QuestionNumber++;
            GameData.Move = false;
            _isTurned = false;
            _dialogues1 = true;
        }

        if(_isFurnace && GameData.QuestionNumber == 8 && _interactManager.Interact(Keys.E) && _isTurned)
        {
            _drop++;

            if(_drop >= 5)
            {
                GameData.QuestionNumber++;
            }
        }

        if(_dialogues2 && _interactManager.Interact(Keys.E))
        {
            GameData.Move = true;
            _isTurned = true;
            _dialogues2 = false;
        }

        if(_isChest && GameData.QuestionNumber == 1 && _interactManager.Interact(Keys.E) && _isTurned) 
        {
            GameData.QuestionNumber++;
            GameData.Move = false;
            _isTurned = false;
            _dialogues2 = true;
        }

        if(_startDialogues && _interactManager.Interact(Keys.E))
        {
            GameData.Move = true;
            _isTurned = true;
            _startDialogues = false;
        }

        if(_doorDialogues && _interactManager.Interact(Keys.E))
        {
            GameData.Move = true;
            _isTurned = true;
            _doorDialogues = false;
        } else if(_isDoor && _interactManager.Interact(Keys.E) && GameData.QuestionNumber < 2)
        {
            GameData.Move = false;
            _isTurned = false;
            _doorDialogues = true;
        } else if(_isDoor && _interactManager.Interact(Keys.E) && GameData.QuestionNumber >= 2)
        {
            _sceneManager.ChangeScene("Garden");
        }

        GameData.previousState = state;
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

        Vector2 ContinueM = _pixelfont.MeasureString("Press E to continue");

        _graphicsDevice.Clear(Color.Black);

        if(_isTurned)
        {
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
        }

        float LayerDeep = 0.01f;

        foreach(var layer in _map.Layers)
        {
            _mapManager.DrawLayer(layer, spriteBatch, LayerDeep, _camera);
            LayerDeep+=0.01f;
        }

        _player.Draw(spriteBatch, _playerTexture, _camera);
        _questManager.DrawQuest(GameData.Quests[GameData.QuestionNumber], spriteBatch, _graphicsDevice, _pixelfont);
        
        if(!_isTurned)
        {
            spriteBatch.Draw(_pixelTexture, new Rectangle(Width / 2 - 300, (Height / 4)* 3 - 100, 600, 100), null, Color.White * 0.1f, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
        }

        if(_dialogues1)
        {
            spriteBatch.DrawString(_pixelfont, "Ohh, the furnace is burnt out. I need some wood.", new Vector2(Width / 2 - 285, (Height / 4) * 3 - 75), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(_pixelfont, "Press E to continue", new Vector2(Width / 2 - ContinueM.X / 4, (Height / 4) * 3 - 15 - ContinueM.Y / 2), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        }

        if(_dialogues2)
        {
            spriteBatch.DrawString(_pixelfont, "It is empty. I need to search wood in forest.", new Vector2(Width / 2 - 265, (Height / 4) * 3 - 75), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(_pixelfont, "Press E to continue", new Vector2(Width / 2 - ContinueM.X / 4, (Height / 4) * 3 - 15 - ContinueM.Y / 2), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        }

        if(_startDialogues)
        {
            spriteBatch.DrawString(_pixelfont, "I feel cold. I think I need to see the furnace.", new Vector2(Width / 2 - 265, (Height / 4) * 3 - 75), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(_pixelfont, "Press E to continue", new Vector2(Width / 2 - ContinueM.X / 4, (Height / 4) * 3 - 15 - ContinueM.Y / 2), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        }

        if(_doorDialogues)
        {
            spriteBatch.DrawString(_pixelfont, "Outside is very cold. Come back later.", new Vector2(Width / 2 - 225, (Height / 4) * 3 - 75), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(_pixelfont, "Press E to continue", new Vector2(Width / 2 - ContinueM.X / 4, (Height / 4) * 3 - 15 - ContinueM.Y / 2), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        }
    }
}