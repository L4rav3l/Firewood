using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace firewood;

public interface IScene {
    public void LoadContent();

    public void Update(GameTime gameTime);

    public void Draw(SpriteBatch spriteBatch);
}