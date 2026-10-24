using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace firewood;

public class Menu : IScene {

    private GraphicsDevice _graphicsDevice;
    private SceneManager _sceneManager;
    private ContentManager _contentManager;

    private SpriteFont _pixelfont;

    private bool _blinking = false;

    private int _Selected = 0;
    private float _blinkCooldown = 500;

    public Menu(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _sceneManager = sceneManager;
        _contentManager = contentManager;
    }

    public void LoadContent()
    {
        _pixelfont = _contentManager.Load<SpriteFont>("pixelfont");
    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(_blinkCooldown >= 0)
        {
            _blinkCooldown -= elapsed;
        }

        if(_blinkCooldown <= 0) {
            
            _blinking = !_blinking;
            _blinkCooldown = 500;

        }

        KeyboardState state = Keyboard.GetState();

        if(state.IsKeyDown(Keys.Up) && !GameData.previousState.IsKeyDown(Keys.Up))
        {
            if(_Selected == 0) {
                _Selected = 1;
            } else {
                _Selected = 0;
            }
        }

        if(state.IsKeyDown(Keys.Down) && !GameData.previousState.IsKeyDown(Keys.Down))
        {
            if(_Selected == 1) {
                _Selected = 0;
            } else {
                _Selected = 1;
            }
        }

        if(state.IsKeyDown(Keys.Enter) && !GameData.previousState.IsKeyDown(Keys.Enter)) 
        {
            if(_Selected == 0) {
                _sceneManager.AddScene(new Home(_graphicsDevice, _sceneManager, _contentManager), "Home");
                _sceneManager.ChangeScene("Home");
            } else {
                GameData.Quit = true;
            }
        }

        GameData.previousState = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphicsDevice.Clear(Color.Gray);

        int width = _graphicsDevice.Viewport.Width;
        int height = _graphicsDevice.Viewport.Height;

        string playText = "";
        string quitText = "";

        if(_Selected == 0) {

            if(_blinking == false)
            {
                playText = "> Play <";
            } else {
                playText = "Play";
            }

            quitText = "Quit";
        } else {
            playText = "Play";

            if(_blinking == false) {
                quitText = "> Quit <";
            } else {
                quitText = "Quit";
            }
        }

        Vector2 LabelM = _pixelfont.MeasureString("Firewood");
        Vector2 Label = new Vector2((width / 2) - (LabelM.X / 2), (height / 4) - (LabelM.Y / 2));

        spriteBatch.DrawString(_pixelfont, "Firewood", Label, Color.White);

        Vector2 PlayM = _pixelfont.MeasureString(playText);
        Vector2 Play = new Vector2((width / 2) - (PlayM.X / 2), ((height / 4) * 2) - (PlayM.Y / 2));

        spriteBatch.DrawString(_pixelfont, playText, Play, Color.White);

        Vector2 QuitM = _pixelfont.MeasureString(quitText);
        Vector2 Quit = new Vector2((width / 2) - (QuitM.X / 2), ((height / 4) * 2) - (QuitM.Y / 2) + 50);

        spriteBatch.DrawString(_pixelfont, quitText, Quit, Color.White);


    }
}