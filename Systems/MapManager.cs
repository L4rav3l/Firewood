using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;
using System.Collections.Generic;

namespace firewood;

public class MapManager
{
    private TmxMap _map;
    private Dictionary<int, Texture2D> _tilesetTextures;

    public MapManager(TmxMap map, Dictionary<int, Texture2D> tilesetTextures)
    {
        _map = map;
        _tilesetTextures = tilesetTextures;
    }

    public void DrawLayer(TmxLayer layer, SpriteBatch spriteBatch, float LayerDeep, Camera2D _camera)
    {
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                var tile = layer.Tiles[y * map.Width + x];

                if(tile.Gid == 0)
                {
                    continue;
                }

                TmxTileset tileset = null;
                int firstGid = 0;

                foreach(var ts in map.Tilesets)
                {
                    if(tile.Gid >= ts.FirstGid)
                    {
                        tileset = ts;
                        firstGid = ts.FirstGid;
                    }
                }

                var texture = tilesetTextures[firstGid];

                int localId = tile.Gid - firstGid;

                int tileWidth = map.TileWidth;
                int tileHeight = map.TileHeight;

                int tilesetColumns = ((int)tileset.Image.Width) / tileWidth;

                int srcX = (localId % tilesetColumns) * tileWidth;
                int srcY = (localId % tilesetColumns) * tileHeight;

                var sourceRectangle = new Rectangle(srcX, srcY, tileWidth, tileHeight);
                Vector2 dest = _camera.WorldToScreen(new Vector2(x * tileWidth, y * tileHeight));

                spriteBatch.Draw(texture, dest, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDeep);
            }
        }
    }
}