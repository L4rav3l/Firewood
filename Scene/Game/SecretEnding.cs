using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace firewood;

public class SecretEnding : IScene
{
    private _graphicsDevice _graphicsDevice;
    private ContentManager _contentManager;
    private SceneManager _sceneManager;

    private SpriteFont _pixelfont;

    public SecretEnding(GraphicsDevice graphicsDevice, SceneManager sceneManager, ContentManager contentManager)
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
        Keyboard state = Keyboard.GetState();

        if(state.IsKeyDown(Keys.E) && !GameData.previousState.IsKeyDown(Keys.E))
        {
            _sceneManager.ChangeScene("Menu");
        }

        GameData.previousState = state;
    }

    public void Draw()
}