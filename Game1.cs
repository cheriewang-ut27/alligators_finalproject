using System;
using System.Collections.Generic;
using alligators_finalproject.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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
    
    // AUDIO
    private SoundEffect bubbleSound;
    private SoundEffect chompSound;
    private Song bgMusic;
    private SoundEffect successSound;
    private SoundEffect failSound;
    private SoundEffect eatSound;
    private SoundEffect zapSound;

    private float moveSoundTimer = 0f;
    private float moveSoundInterval = 1.2f;
    
    private bool hasPlayedSuccessSound = false;
    private bool hasPlayedFailSound = false;
    
    private float volumeDisplay = 100f;
    private bool isMutedDisplay = false;
    private KeyboardState prevKb;
    
    private bool mouthClosedFlag = false;
    
    private Tilemap tilemap;
    private GameManager gameManager;
    private HUD hud;
    
    private Background background;
    private Texture2D oceanSheet;
    public Game1() : base(1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        gameManager = new GameManager();
    }

    protected override void LoadContent()
    {
        if (Atlas == null)
        {
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
        
        Texture2D oceanSheet = Content.Load<Texture2D>("textures/ocean_spritesheet");
        background = new Background(oceanSheet, 2, 8, 0.15f);

        hudFont = Content.Load<SpriteFont>("fonts/HudFont");
        hud = new HUD(hudFont, GraphicsDevice);
        
        // SOUNDS 
        bubbleSound = Content.Load<SoundEffect>("sounds/bubblesound");
        chompSound = Content.Load<SoundEffect>("sounds/chomp");
        bgMusic = Content.Load<Song>("sounds/bgmusic");
        successSound = Content.Load<SoundEffect>("sounds/success");
        eatSound = Content.Load<SoundEffect>("sounds/eat");
        zapSound =  Content.Load<SoundEffect>("sounds/zap");
        failSound = Content.Load<SoundEffect>("sounds/fail");
        
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = volumeDisplay / 100f;
        MediaPlayer.Play(bgMusic);

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        
        KeyboardState kb = Keyboard.GetState();

        if (kb.IsKeyDown(Keys.Escape))
            Exit();

        if (kb.IsKeyDown(Keys.M) && !prevKb.IsKeyDown(Keys.M))
        {
            isMutedDisplay = !isMutedDisplay;

            float vol = isMutedDisplay ? 0f : volumeDisplay / 100f;

            SoundEffect.MasterVolume = vol;
            MediaPlayer.Volume = vol;
        }

        if (kb.IsKeyDown(Keys.OemComma) && !prevKb.IsKeyDown(Keys.OemComma))
        {
            volumeDisplay = Math.Max(0, volumeDisplay - 10);

            if (!isMutedDisplay)
            {
                float vol = volumeDisplay / 100f;
                SoundEffect.MasterVolume = vol;
                MediaPlayer.Volume = vol; 
            }
        }

        if (kb.IsKeyDown(Keys.OemPeriod) && !prevKb.IsKeyDown(Keys.OemPeriod))
        {
            volumeDisplay = Math.Min(100, volumeDisplay + 10);
            if (!isMutedDisplay)
                SoundEffect.MasterVolume = volumeDisplay / 100f;
            MediaPlayer.Volume = volumeDisplay / 100f;
        }

        switch (gameManager.CurrentState)
        {
            case GameState.Menu:
                SetupLevel();
                if (kb.IsKeyDown(Keys.Space))
                    gameManager.StartGame();
                break;
            
            case GameState.LevelTransition:
                
                if (!hasPlayedSuccessSound)
                {
                    if (!isMutedDisplay)
                        successSound.Play(volumeDisplay / 100f, 0f, 0f);
            
                    hasPlayedSuccessSound = true; 
                }

                if (kb.IsKeyDown(Keys.X))
                {
                    gameManager.NextLevel();
                    SetupLevel();
                    hasPlayedSuccessSound = false; 
                }
                break;

            case GameState.Playing:
                foreach (Jellyfish jellyfish in jellyfishList)
                {
                    jellyfish.Update(gameTime);
                }

                Vector2 prevPos = alligator.position;
                alligator.Update(gameTime, tilemap);
                
                // bubble sound
                if (alligator.position != prevPos)
                {
                    moveSoundTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (moveSoundTimer >= moveSoundInterval)
                    {
                        if (!isMutedDisplay)
                            bubbleSound.Play(volumeDisplay / 100f, 0.5f, 0f); 

                        moveSoundTimer = 0f;
                        Random rand = new Random();
                        moveSoundInterval = 0.3f + (float)rand.NextDouble() * 0.4f;
                    }
                }
                else 
                {
                    moveSoundTimer = moveSoundInterval; 
                }
                // chomp sound
                bool isClosedNow = alligator.IsMouthClosed();

                if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
                {
                    if (!isMutedDisplay)
                        chompSound.Play(volumeDisplay / 100f, 0f, 0f);
                }
                mouthClosedFlag = isClosedNow;
                testFish.FleeFrom(alligator.GetPosition());
                testFish.Update(gameTime, tilemap);
                
                HandleCollisions();
                gameManager.UpdateTimer(gameTime);
                break;

            case GameState.Win:
            case GameState.Lose:
                if (!hasPlayedFailSound)
                {
                    if (!isMutedDisplay)
                        failSound.Play(volumeDisplay / 100f, 0f, 0f);
            
                    hasPlayedFailSound = true; 
                }
                
                if (kb.IsKeyDown(Keys.R))
                {
                    hasPlayedFailSound = false;
                    RestartGame();
                }
                
                break;
        }

        prevKb = kb;
        background.Update(gameTime);

        base.Update(gameTime);
    }
    
    private void SetupLevel()
    {
        
        alligator.position = new Vector2(100, 100); 

        float speedBoost = (gameManager.CurrentLevel - 1) * 2f;
        testFish = new Fish(
            fishTexture1, 
            new Vector2(200, 200), 
            5f + speedBoost, 10f + speedBoost, 
            ScreenBounds.Width, ScreenBounds.Height, 0.1f
        );


        jellyfishList.Clear();
        int jellyCount = gameManager.CurrentLevel * 2;
        Random rand = new Random();
        for (int i = 0; i < jellyCount; i++)
        {
            jellyfishList.Add(new Jellyfish(
                new Vector2(rand.Next(100, ScreenBounds.Width - 100), rand.Next(300, 600)), 
                ScreenBounds
            ));
        }
    }
    
    private struct DeadFish 
    {
        public Vector2 Position;
        public float Opacity;
        public float YOffset;
    }
    private List<DeadFish> ghostFish = new List<DeadFish>();

    private void HandleCollisions()
    {
        if (testFish.GetBounds().Intersects(alligator.GetBounds()) && alligator.IsMouthClosed())
        {
            ghostFish.Add(new DeadFish { 
                Position = testFish.position, 
                Opacity = 1f, 
                YOffset = 0 
            });

            eatSound.Play(volumeDisplay / 100f, 0f, 0f);
            gameManager.AddScore(1);
            testFish.SpawnFish(alligator.position, tilemap);
        }
        
        foreach (Jellyfish jellyfish in jellyfishList)
        {
            if (alligator.GetBounds().Intersects(jellyfish.BoundingRectangle))
            {
                if (gameManager.InvincibilityTimer <= 0)
                {
                    zapSound.Play(volumeDisplay / 100f, 0f, 0f);
                    gameManager.TakeDamage(10);
                    jellyfish.Animate("jelly-attack");
                }
            }
        }
        
    }

    private void RestartGame()
    {
        gameManager.StartGame();
        testFish.SpawnFish(alligator.position, tilemap); 
        alligator.position = new Vector2(100, 100); 
    
        jellyfishList.Clear();
        jellyfishList.Add(new Jellyfish(new Vector2(800, 300), ScreenBounds));
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin();

        
        if (gameManager.CurrentState == GameState.Menu)
        {
            background.Draw(SpriteBatch, ScreenBounds, 2.0f);
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.Black });
            SpriteBatch.Draw(pixel, new Rectangle(0, 0, ScreenBounds.Width, ScreenBounds.Height), Color.Black * 0.6f);
        }
        else
        {
            tilemap.Draw(SpriteBatch);
            testFish.Draw(SpriteBatch);
            
            if (gameManager.InvincibilityTimer > 0)
            {
                if ((int)(gameTime.TotalGameTime.TotalMilliseconds / 100) % 2 == 0)
                {
                    alligator.Draw(SpriteBatch);
                }
            }
            else
            {
                alligator.Draw(SpriteBatch);
            }

            foreach (Jellyfish jellyfish in jellyfishList)
            {
                jellyfish.Draw(SpriteBatch);
            }
            
            for (int i = ghostFish.Count - 1; i >= 0; i--)
            {
                var ghost = ghostFish[i];
                SpriteBatch.Draw(fishTexture1, ghost.Position + new Vector2(0, ghost.YOffset), 
                    null, Color.White * ghost.Opacity, 0f, Vector2.Zero, 0.1f, SpriteEffects.None, 0f);
                
                ghost.Opacity -= 0.02f;
                ghost.YOffset -= 1.5f; 
                ghostFish[i] = ghost;

                if (ghost.Opacity <= 0) ghostFish.RemoveAt(i);
            }
        }

        hud.Draw(SpriteBatch, gameManager, ScreenBounds);

        string volumeText = isMutedDisplay ? "Volume: Muted" : $"Volume: {(int)volumeDisplay}%";
        SpriteBatch.DrawString(hudFont, volumeText, new Vector2(ScreenBounds.Width - 220, ScreenBounds.Height-80), Color.White);
        SpriteBatch.DrawString(hudFont, "M | , | .", new Vector2(ScreenBounds.Width - 160, ScreenBounds.Height-45), Color.Gray);
        
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
