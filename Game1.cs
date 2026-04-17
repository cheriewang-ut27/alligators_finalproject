using System;
using group_9_assignment7.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_9_assignment7;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private Fish testFish;
    private Texture2D fishTexture1;
    private Texture2D fishTexture2;
    private Texture2D fishTexture3;

    // ALLIGATOR
    private Alligator alligator;
    private Texture2D gator1;
    private Texture2D gator2;
    private Texture2D gator3;
    private Texture2D gator4;

    // GUI part
    private SpriteFont hudFont;
    private float timeRemaining;
    private int score;
    private bool gameOver;
    private bool playerWon;
    private bool fishCaught;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

    
        timeRemaining = 30f;
        score = 0;
        gameOver = false;
        playerWon = false;
        fishCaught = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        fishTexture1 = Content.Load<Texture2D>("fish/fish1");
        fishTexture2 = Content.Load<Texture2D>("fish/fish2");
        fishTexture3 = Content.Load<Texture2D>("fish/fish3");

        testFish = new Fish(
            fishTexture1,
            new Vector2(200, 200),
            5f, 20f,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            0.1f
        );

        gator1 = Content.Load<Texture2D>("alligator/alligator-1");
        gator2 = Content.Load<Texture2D>("alligator/alligator-2");
        gator3 = Content.Load<Texture2D>("alligator/alligator-3");
        gator4 = Content.Load<Texture2D>("alligator/alligator-4");

        alligator = new Alligator(
            gator1,
            gator2,
            gator3,
            gator4,
            new Vector2(100, 100),
            5f,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            0.2f
        );

        hudFont = Content.Load<SpriteFont>("HudFont");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            keyboard.IsKeyDown(Keys.Escape))
            Exit();

        
        if (gameOver)
        {
            if (keyboard.IsKeyDown(Keys.R))
            {
                RestartGame();
            }
            base.Update(gameTime);
            return;
        }

        timeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            gameOver = true;
            playerWon = false;
        }

        alligator.Update(gameTime);
        testFish.FleeFrom(GetAlligatorPosition());
        testFish.Update(gameTime);

        if (!fishCaught &&
            testFish.GetBounds().Intersects(alligator.GetBounds()) &&
            alligator.IsMouthClosed())
        {
            fishCaught = true;
            playerWon = true;
            gameOver = true;

            score = (int)Math.Ceiling(timeRemaining);
            
        }

        base.Update(gameTime);
    }

    private Vector2 GetAlligatorPosition()
    {
        return new Vector2(alligator.GetBounds().X, alligator.GetBounds().Y);
    }

    private void RestartGame()
    {
        timeRemaining = 30f;
        score = 0;
        gameOver = false;
        playerWon = false;
        fishCaught = false;

        testFish = new Fish(
            fishTexture1,
            new Vector2(200, 200),
            5f, 20f,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            0.1f
        );

        alligator = new Alligator(
            gator1,
            gator2,
            gator3,
            gator4,
            new Vector2(100, 100),
            5f,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            0.2f
        );
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        testFish.Draw(_spriteBatch);
        alligator.Draw(_spriteBatch);

        if (!gameOver)
        {
            _spriteBatch.DrawString(hudFont, "Time: " + Math.Ceiling(timeRemaining), new Vector2(20, 20), Color.White);
            _spriteBatch.DrawString(hudFont, "Score: " + score, new Vector2(20, 70), Color.White);
            _spriteBatch.DrawString(hudFont, "WASD to move | Space to close mouth", new Vector2(20, 120), Color.White);
        }
        else
        {
            string result = playerWon ? "YOU WIN!" : "YOU LOSE!";
            Color color = playerWon ? Color.Yellow : Color.Red;

            _spriteBatch.DrawString(hudFont, result, new Vector2(820, 400), color);
            _spriteBatch.DrawString(hudFont, "Final Score: " + score, new Vector2(780, 470), Color.White);
            _spriteBatch.DrawString(hudFont, "Press R to Restart", new Vector2(760, 540), Color.White);
            _spriteBatch.DrawString(hudFont, "Press ESC to Exit", new Vector2(760, 600), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
