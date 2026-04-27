using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class MenuJellyfish
{
    private Texture2D _bell;
    private Texture2D _tentacleLeft;
    private Texture2D _tentacleCenter;
    private Texture2D _tentacleRight;

    private Vector2 _position;
    private float _time;
    private float _speed;
    private float _scale;

    public MenuJellyfish(
        Texture2D bell,
        Texture2D tentacleLeft,
        Texture2D tentacleCenter,
        Texture2D tentacleRight,
        Vector2 startPosition,
        float speed = 70f,
        float scale = 0.2f)
    {
        _bell = bell;
        _tentacleLeft = tentacleLeft;
        _tentacleCenter = tentacleCenter;
        _tentacleRight = tentacleRight;
        _position = startPosition;
        _speed = speed;
        _scale = scale;
    }

    public void Update(GameTime gameTime, Rectangle screenBounds)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time += dt;

        float cycle = (MathF.Sin(_time * 2.2f) + 1f) / 2f;

        float easeOut = 1f - MathF.Pow(1f - cycle, 7f);

        float velocity = (_speed * 0.25f) + (easeOut * _speed * 1.8f);

        _position.Y -= velocity * dt;

        _position.X += MathF.Sin(_time * 2.2f) * 0.8f;

        if (_position.Y < -150)
        {
            _position.Y = screenBounds.Height + 150;
            _position.X = Random.Shared.Next(100, screenBounds.Width - 100);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float sway = MathF.Sin(_time * 1.2f) * 0.38f;

        Vector2 bellOrigin = new Vector2(_bell.Width / 2f, _bell.Height / 2f);
        Vector2 bellPosition = _position;

        float scaledBellWidth = _bell.Width * _scale;
        float scaledBellHeight = _bell.Height * _scale;

        Vector2 tentacleBase = bellPosition + new Vector2(0, scaledBellHeight * 0.25f);

        DrawTentacle(spriteBatch, _tentacleCenter, tentacleBase + new Vector2(.1f*scaledBellWidth, scaledBellHeight * 0.05f),sway * 0.5f);

        DrawTentacle(spriteBatch, _tentacleLeft, tentacleBase + new Vector2(-scaledBellWidth * 0.22f, 0), sway * 0.8f);

        DrawTentacle(spriteBatch, _tentacleRight, tentacleBase + new Vector2(scaledBellWidth * 0.22f, 0), sway);

        spriteBatch.Draw(_bell, bellPosition, null, Color.White, MathF.Sin(_time * 2f) * 0.08f, bellOrigin, _scale, 
            SpriteEffects.None, 0f);
    }

    private void DrawTentacle(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float rotation)
    {
        Vector2 origin = new Vector2(texture.Width / 2f, 0);

        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            rotation,
            origin,
            _scale,
            SpriteEffects.None,
            0f
        );
    }
}