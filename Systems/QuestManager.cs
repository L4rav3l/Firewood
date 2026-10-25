using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace firewood;

public class QuestManager
{

    public void DrawQuest(string questText, SpriteBatch spriteBatch, GraphicsDevice _graphicsDevice, SpriteFont _pixelfont)
    {
        int width = _graphicsDevice.Viewport.Width;
        int height = _graphicsDevice.Viewport.Height;

        Vector2 QuestM = _pixelfont.MeasureString(questText);
        Vector2 QuestPosition = new Vector2((width / 2) - (QuestM.X / 4), 50 - (QuestM.Y / 4));

        spriteBatch.DrawString(_pixelfont, questText, QuestPosition, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
    }
}