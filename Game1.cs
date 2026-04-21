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
    //private Jellyfish jellyfish;
    private List<Jellyfish> jellyfishList = new List<Jellyfish>();
    
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
    
    private Tilemap tilemap;
    private GameManager gameManager;
    private HUD hud;
    
    // SOUND 
    private SoundEffect bubbleSound;
    private SoundEffect chompSound;
    private Song bgMusic;

    private float volume = 0.5f;
    private bool isMuted = false;
    
    private float moveSoundTimer = 0f;
    private float moveSoundInterval = 0.3f;

    private KeyboardState previousKb;
    

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
        
        //jellyfish = new Jellyfish(new Vector2(400, 300), ScreenBounds);
        jellyfishList.Add(new Jellyfish(new Vector2(800, 300), ScreenBounds));
        jellyfishList.Add(new Jellyfish(new Vector2(600, 300), ScreenBounds));
     
        hud = new HUD(Content.Load<SpriteFont>("HudFont"));
        
        // SOUNDS
        bubbleSound = Content.Load<SoundEffect>("sounds/bubblesound");
        chompSound = Content.Load<SoundEffect>("sounds/chomp");
        bgMusic = Content.Load<Song>("sounds/bgmusic");
        
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = volume;
        MediaPlayer.Play(bgMusic);
        
        base.LoadContent();
        
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState kb = Keyboard.GetState();
        
        // SOUND CONTROL 
        bool Pressed(Keys key) => kb.IsKeyDown(key) && !previousKb.IsKeyDown(key);
        // mute control 
        if (Pressed(Keys.M))
        {
            isMuted = !isMuted;
            MediaPlayer.IsMuted = isMuted;
        }
        // volume up (.)
        if (Pressed(Keys.OemPeriod))
        {
            volume = MathHelper.Clamp(volume + 0.1f, 0f, 1f);
            MediaPlayer.Volume = volume;
        }
        // volume down (,)
        if (Pressed(Keys.OemComma))
        {
            volume = MathHelper.Clamp(volume - 0.1f, 0f, 1f);
            MediaPlayer.Volume = volume;
        }
        // chomp
        if (Pressed(Keys.Space))
        {
            if (!isMuted)
                chompSound.Play(volume, 0f, 0f);
        }

        if (kb.IsKeyDown(Keys.Escape))
            Exit();

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
                Vector2 prevPos = alligator.position;
                alligator.Update(gameTime);
                moveSoundTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (alligator.position != prevPos && moveSoundTimer >= moveSoundInterval)
                {
                    if (!isMuted)
                        bubbleSound.Play(volume, 0f, 0f);

                    moveSoundTimer = 0f;
                }
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
                    // initialize new jellyfish
                    RestartGame();
                }
                    
                break;
        }
        previousKb = kb;
        base.Update(gameTime);
    }

    private void HandleCollisions()
    {

        if (testFish.GetBounds().Intersects(alligator.GetBounds()) &&
            alligator.IsMouthClosed())
        {
            gameManager.AddScore(1);
            testFish.SpawnFish();
            
        }
        // Jellyfish collision
        
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
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
