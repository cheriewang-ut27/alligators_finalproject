using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class AnimatedSprite
{
    public TextureRegion Region { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;
    public Vector2 Origin { get; set; }
    //public Vector2 Position { get; set; }
    public SpriteEffects SpriteEffects { get; set; }
    public float LayerDepth { get; set; } = 0;
    public float Width => Region.Width * Scale.X;
    public float Height => Region.Height * Scale.Y;
    private int _currentFrame;
    private float _elapsedTime;
    private Animation _animation;
    public Animation Animation
    {
        get =>  _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }
    
    public void Center()
    {
        Origin = new Vector2(Region.Width / 2, Region.Height / 2);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Region.Draw(spriteBatch, position, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
        //Region.Draw(spriteBatch, position);
    }

    public void Update(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_elapsedTime > _animation.FrameTime)
        {
            _elapsedTime -= _animation.FrameTime;
            _currentFrame++;

            if (_currentFrame >= _animation.Frames.Count)
            {
                _currentFrame = 0;
            }
            
            Region = _animation.Frames[_currentFrame];
        }
    }
    
}