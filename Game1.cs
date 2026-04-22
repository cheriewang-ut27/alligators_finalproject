using System;
using System.Collections.Generic;
using alligators_finalproject.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace alligators_finalproject;

public class Game1 : Globals
{

    private List<Jellyfish> jellyfishList = new List<Jellyfish>();
    
    private Fish testFish;
    private Texture2D fishTexture1;
    private Texture2D fishTexture2;
    private Texture2D fishTexture3;

    
    private Alligator alligator;
    private Texture2D gator1;
    private Texture2D gator2;
    private Texture2D gator3;
    private Texture2D gator4;

   
    private SpriteFont hudFont;
    
    private Tilemap tilemap;
    private GameManager gameManager;
    private HUD hud;

    private float volume = 0.5f;
    private bool isMuted = false;
    private KeyboardState previousKeyboardState;

    public Game1()  : base(1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        gameManager = new GameManager();
    }

    protected override void LoadContent()
    {
        if (Atlas == null) {
            Atlas = TextureAtlas.FromFile(Content, "textures/atlas.xml");
        }

        tilemap = Tilemap.FromFile(Content, "textures/tilemap.xml");
        tilemap.Scale = new Vector2(0.4f, 0.4f);
        
        fishTexture1 = Content.Load<Texture2D>("fish/fish1");
        fishTexture2 = Content.Load<Texture2D>("fish/fish2");
        fishTexture3 = Content.Load<Texture2D>("fish/fish3");

        testFish = new Fish(
            fishTexture1,
            new Vector2(200, 200),
            5f, 10f,
            ScreenBounds.Width,
            ScreenBounds.Height,
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
            8f,
            ScreenBounds.Width,
            ScreenBounds.Height,
            0.2f
        );
        
        jellyfishList.Add(new Jellyfish(new Vector2(800, 300), ScreenBounds));
        jellyfishList.Add(new Jellyfish(new Vector2(600, 300), ScreenBounds));

        hudFont = Content.Load<SpriteFont>("HudFont");
        hud = new HUD(hudFont);

        SoundEffect.MasterVolume = volume;
     
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState kb = Keyboard.GetState();

        if (kb.IsKeyDown(Keys.Escape))
            Exit();

        HandleVolumeControls(kb);

        switch (gameManager.CurrentState)
        {
            case GameState.Menu:
                if (kb.IsKeyDown(Keys.Space))
                    gameManager.StartGame();
                
                break;

            case GameState.Playing:
                foreach (Jellyfish jellyfish in jellyfishList)
                {
                    jellyfish.Update(gameTime);
                }

                alligator.Update(gameTime);
                testFish.FleeFrom(alligator.GetPosition());
                testFish.Update(gameTime);
                
                HandleCollisions();
                gameManager.UpdateTimer(gameTime);
                gameManager.CheckWinCondition();
                break;

            case GameState.Win:
            case GameState.Lose:
                if (kb.IsKeyDown(Keys.R))
                {
                    RestartGame();
                }
                break;
        }

        previousKeyboardState = kb;
        base.Update(gameTime);
    }

    private void HandleVolumeControls(KeyboardState kb)
    {
        if (kb.IsKeyDown(Keys.M) && !previousKeyboardState.IsKeyDown(Keys.M))
        {
            isMuted = !isMuted;
            SoundEffect.MasterVolume = isMuted ? 0f : volume;
        }

        if (kb.IsKeyDown(Keys.OemComma) && !previousKeyboardState.IsKeyDown(Keys.OemComma))
        {
            volume = Math.Max(0f, volume - 0.1f);
            if (!isMuted)
                SoundEffect.MasterVolume = volume;
        }

        if (kb.IsKeyDown(Keys.OemPeriod) && !previousKeyboardState.IsKeyDown(Keys.OemPeriod))
        {
            volume = Math.Min(1f, volume + 0.1f);
            if (!isMuted)
                SoundEffect.MasterVolume = volume;
        }
    }

    private void HandleCollisions()
    {
        if (testFish.GetBounds().Intersects(alligator.GetBounds()) &&
            alligator.IsMouthClosed())
        {
            gameManager.AddScore(1);
            testFish.SpawnFish();
        }

    
        foreach (Jellyfish jellyfish in jellyfishList)
        {
            if (alligator.GetBounds().Intersects(jellyfish.BoundingRectangle))
            {
                if (gameManager.InvincibilityTimer <= 0)
                {
                    gameManager.TakeDamage(20);
                    jellyfish.Animate("jelly-attack");
                }
            }
        }
    }

    private void RestartGame()
    {
        gameManager.StartGame();
        testFish.SpawnFish(); 
        alligator.position = new Vector2(100, 100); 
    
        jellyfishList.Clear();
        jellyfishList.Add(new Jellyfish(new Vector2(800, 300), ScreenBounds));
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin();
        
        tilemap.Draw(SpriteBatch);

        if (gameManager.CurrentState != GameState.Menu)
        {
            testFish.Draw(SpriteBatch);
            alligator.Draw(SpriteBatch);

            foreach (Jellyfish jellyfish in jellyfishList)
            {
                jellyfish.Draw(SpriteBatch);
            }
        }

        hud.Draw(SpriteBatch, gameManager);

        
        string volumeText = isMuted ? "Volume: Muted" : $"Volume: {(int)(volume * 100)}%";
        Vector2 textSize = hudFont.MeasureString(volumeText);
        Vector2 textPosition = new Vector2(ScreenBounds.Width - textSize.X - 20, 20);

        SpriteBatch.DrawString(hudFont, volumeText, textPosition, Color.White);
        SpriteBatch.DrawString(hudFont, "M = Mute   , = Down   . = Up", new Vector2(ScreenBounds.Width - 320, 50), Color.White);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
