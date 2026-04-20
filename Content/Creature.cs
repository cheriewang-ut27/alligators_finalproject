using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject.Content;

public class Creature
{
    protected Texture2D texture;
    public Vector2 position;
    protected Vector2 velocity; //if acceleration, otherwise dont need
    protected float speed;
    protected Vector2 direction;
    protected float scale = 1f;
    
    //screen info
    protected int screenWidth;
    protected int screenHeight;

    protected SpriteEffects spriteEffect = SpriteEffects.None; //gets assigned to flip texture
    
    public Creature(Texture2D texture, Vector2 position, float speed, int screenWidth, int screenHeight, float scale = 1f)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.texture = texture;
        this.position = position;
        this.speed = speed;
        this.scale = scale;
    }
    
    public virtual void Update(GameTime gameTime)
    {
        UpdateDirectionLogic(gameTime); //child decides direction with separate function

        if (direction != Vector2.Zero)
            direction.Normalize();

        position += direction * speed; //generic update position with normalized direction

        UpdateDirection(direction); //flip texture as needed
    }

    protected virtual void UpdateDirectionLogic(GameTime gameTime)
    {
        //child classes will overwrite this, nothing for generic class
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, null, Color.White, 0f,
            Vector2.Zero, scale, spriteEffect, 0f);
    }

    protected void UpdateDirection(Vector2 movement)
    {
        if (movement.X < 0)
            spriteEffect = SpriteEffects.None; //texture default faces left
        else if (movement.X > 0)
            spriteEffect = SpriteEffects.FlipVertically; //turn texture to face right
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)(texture.Width * scale),
            (int)(texture.Height * scale)
        );
    }
}