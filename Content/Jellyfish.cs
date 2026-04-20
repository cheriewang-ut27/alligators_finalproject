using System;
using System.Net.Http;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace alligators_finalproject.Content;

public class Jellyfish
{
    private AnimatedSprite sprite;
    public Vector2 Position
    {
        get { return position; }
    }
    Vector2 position;
    private Rectangle screenBounds;

    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }
    Vector2 velocity;
    
    private Rectangle bounds;
    public Rectangle BoundingRectangle
    {
        get
        {
            int left = (int)(position.X - (sprite.Origin.X * sprite.Scale.X)) + bounds.X;
            int top = (int)(position.Y - (sprite.Origin.Y * sprite.Scale.Y)) + bounds.Y;

            return new Rectangle(left, top, bounds.Width, bounds.Height);
        }
    }
    
    private float animationTimer = 0f;
    private bool isAttacking = false;
    private const float AttackDuration = 1.0f;

    public Jellyfish(Vector2 position, Rectangle screenbounds)
    {
        this.position = position;
        screenBounds = screenbounds;
        LoadContent();
    }

    public void LoadContent()
    {
        Animate("jelly-idle");
    }

    public void Animate(string animationKey)
    {
        if (animationKey == "jelly-attack")
        {
            isAttacking = true;
            animationTimer = AttackDuration;
        }
        else
        {
            isAttacking = false;
        }

        sprite = Globals.Atlas.CreateSprite(animationKey);
        sprite.Scale = new Vector2(3, 3);
        sprite.Center();
        
        int width = (int)(sprite.Width * 0.4);
        int height = (int)(sprite.Height * 0.8);
        int x = (int)(sprite.Width - width) / 2;
        int y = (int)(sprite.Height - height);
        bounds = new Rectangle(x, y, width, height);
    }
    


    public void Update(GameTime gameTime)
    {
        
        Random r =  new Random();
        Vector2 startPosition = this.position;
        Vector2 endPosition = new Vector2(this.position.X, -sprite.Height * 2);
        this.position = Vector2.Lerp(startPosition, endPosition, gameTime.ElapsedGameTime.Milliseconds / 3500f);

        if (this.position.Y <= (-1))
        {
            this.position = new Vector2(r.Next(screenBounds.Width), screenBounds.Height);
        }
        
        if (isAttacking)
        {
            animationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer <= 0)
            {
                Animate("jelly-idle"); 
            }
        }
        
        sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        sprite.Draw(spriteBatch, position);
    }
}