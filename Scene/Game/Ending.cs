using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace firewood;

public class Ending : IScene
{
    private GraphicsDevice _graphicsDevice;
    private ContentManager _contentManager;
    private SceneManager _sceneManager;

    private SpriteFont _pixelfont;

    public Ending(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
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
        KeyboardState state = Keyboard.GetState();

        if(state.IsKeyDown(Keys.E) && !GameData.previousState.IsKeyDown(Keys.E))
        {
            _sceneManager.ChangeScene("Menu");
        }

        GameData.previousState = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int width = _graphicsDevice.Viewport.Width;
        int height = _graphicsDevice.Viewport.Height;

        _graphicsDevice.Clear(Color.Gray);

        Vector2 LabelM = _pixelfont.MeasureString("You slept until noon in the warmth.   ");
        Vector2 Label2M = _pixelfont.MeasureString("Press E to return to the menu.");

        Vector2 Label = new Vector2((width / 2) - (LabelM.X / 2), (height / 2) - (LabelM.Y / 2) - 50);
        Vector2 Label2 = new Vector2((width / 2) - (Label2M.X / 2), (height / 2) - (Label2M.Y / 2) + 50);

        spriteBatch.DrawString(_pixelfont, "You slept until noon in the warmth.   ", Label, Color.White);
        spriteBatch.DrawString(_pixelfont, "Press E to return to the menu.", Label2, Color.White);
    }
}