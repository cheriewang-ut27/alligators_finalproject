using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class Globals : Game
{
    // sets up everything
    public static GraphicsDeviceManager GraphicsDeviceManager {get; private set;}
    public static SpriteBatch SpriteBatch {get; private set;}
    public static SpriteFont SpriteFont {get; private set;}
    public static GraphicsDevice GraphicsDevice {get; private set;}
    public static new ContentManager ContentManager {get; private set;}
    public static TextureAtlas Atlas {get; set;}
    public Rectangle ScreenBounds {get; private set;}

    public Globals(int width, int height, bool isFullScreen)
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
        
        GraphicsDeviceManager.PreferredBackBufferWidth = width;
        GraphicsDeviceManager.PreferredBackBufferHeight = height;
        
        GraphicsDeviceManager.IsFullScreen = isFullScreen;
        
        GraphicsDeviceManager.ApplyChanges();
        ScreenBounds = new Rectangle(0, 0, width, height);
        
        Content = base.Content;
        Content.RootDirectory = "Content";
        
    }

    protected override void Initialize()
    {
        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        
        Atlas = TextureAtlas.FromFile(Content, "atlas.xml");
        
        base.Initialize();
    }
}
