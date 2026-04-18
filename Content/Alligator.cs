using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace alligators_finalproject.Content;

public class Alligator : Creature
{
    private Texture2D[] mouthFrames; // this is to account for the four diff stages of the alligator's movement + tail
    private int currentFrame = 0;

    private float animationTimer = 0f;
    private float animationSpeed = 0.1f;

    private bool isClosing = false;

    private bool tailUp = true;
    private float tailTimer = 0f;
    private float tailInterval = 1f;

    public Alligator(
        Texture2D frame1,
        Texture2D frame2,
        Texture2D frame3,
        Texture2D frame4,
        Vector2 position,
        float speed,
        int screenWidth,
        int screenHeight,
        float scale = 1f
    ) : base(frame1, position, speed, screenWidth, screenHeight, scale)
    {
        mouthFrames = new Texture2D[] { frame1, frame2, frame3, frame4 };
    }

    public override void Update(GameTime gameTime)
    {
        HandleInput();

        base.Update(gameTime);

        AnimateMouth(gameTime);
        AnimateTail(gameTime);
    }
    
    private void AnimateTail(GameTime gameTime)
    {
        tailTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (tailTimer >= tailInterval)
        {
            tailTimer = 0f;
            
            if (!isClosing)
            {
                if (currentFrame == 1)
                    currentFrame = 2;
                else
                    currentFrame = 1;
            }
        }
    }

    private void HandleInput()
    {
        KeyboardState kb = Keyboard.GetState();
        
        if (kb.IsKeyDown(Keys.Space)) // mouth closes when u press space 
        // so that way you can catch the fish
            isClosing = true;
        else
            isClosing = false;
    }

    protected override void UpdateDirectionLogic(GameTime gameTime)
    {
        direction = Vector2.Zero;
        KeyboardState kb = Keyboard.GetState();

        if (kb.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (kb.IsKeyDown(Keys.S)) direction.Y += 1;
        if (kb.IsKeyDown(Keys.A)) direction.X -= 1;
        if (kb.IsKeyDown(Keys.D)) direction.X += 1;
        
        
        float width = texture.Width * scale; // clamping 2 screen 
        float height = texture.Height * scale;

        position = Vector2.Clamp(position,
            Vector2.Zero,
            new Vector2(screenWidth - width, screenHeight - height));
    }

    private void AnimateMouth(GameTime gameTime)
    {
        animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;

            if (isClosing)
            {
                if (currentFrame < 3) // moving it to closed 
                    currentFrame++;
            }
            else
            {
                if (currentFrame > 0) // moving it to open 
                    currentFrame--;
            }
        }
    }

    public bool IsMouthClosed()
    {
        return currentFrame == 3;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D currentTexture = mouthFrames[currentFrame];
        
        spriteBatch.Draw(
            currentTexture,
            position,
            null,
            Color.White,
            -MathHelper.PiOver2,
            Vector2.Zero,
            scale,
            spriteEffect,
            0f
        );
    }
}