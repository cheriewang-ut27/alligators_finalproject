using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class TextureRegion
{
    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
    
    public int Width => SourceRectangle.Width;
    public int Height => SourceRectangle.Height;
    
    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x, y, width, height);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Draw(spriteBatch, position, 0.0f, Vector2.Zero, 
            Vector2.One, SpriteEffects.None, 0);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 origin, Vector2 scale,
        SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(Texture, position, SourceRectangle, Color.White, 
            rotation, origin, scale, effects, layerDepth);
    }
}