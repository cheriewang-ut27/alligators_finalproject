using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class MenuFish
{
    private Texture2D _texture;
    private Vector2 _position;

    private float _time;
    private float _scale;

    private float _baseSpeed;
    private float _cycleSpeed;
    private float _bobAmount;
    private float _burstStrength;

    private float _startY;

    private static Random rand = new Random();

    public MenuFish(Texture2D texture, Vector2 startPosition, float speed = 90f, float scale = 0.15f)
    {
        _texture = texture;
        _position = startPosition;
        _startY = startPosition.Y;
        _scale = scale;

        RandomizeMotion(speed);
    }

    private void RandomizeMotion(float speed)
    {
        _baseSpeed = speed * (0.8f + (float)rand.NextDouble() * 0.4f);     // ±20%
        _cycleSpeed = 1.8f + (float)rand.NextDouble() * 1.5f;             // swim rhythm
        _bobAmount = 4f + (float)rand.NextDouble() * 8f;                 // vertical wobble
        _burstStrength = 1.0f + (float)rand.NextDouble() * 1.4f;         // propulsion strength
    }

    public void Update(GameTime gameTime, Rectangle screenBounds)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time += dt;

        // Swim cycle
        float cycle = (MathF.Sin(_time * _cycleSpeed) + 1f) / 2f;

        // Ease out burst
        float easeOut = 1f - MathF.Pow(1f - cycle, 5f);

        // Small jitter noise
        float jitter = ((float)rand.NextDouble() - 0.5f) * 8f;

        float velocity =
            (_baseSpeed * 0.35f) +
            (easeOut * _baseSpeed * _burstStrength) +
            jitter;

        _position.X += velocity * dt;

        _position.Y = _startY + MathF.Sin(_time * _cycleSpeed) * _bobAmount;

        if (_position.X > screenBounds.Width + 150)
        {
            _position.X = -150;
            _position.Y = rand.Next(120, screenBounds.Height - 120);
            _startY = _position.Y;

            _time = 0f;

            RandomizeMotion(_baseSpeed);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);

        float rotation = MathF.Sin(_time * _cycleSpeed) * 0.05f;

        spriteBatch.Draw(
            _texture,
            _position,
            null,
            Color.White * 0.75f,
            rotation,
            origin,
            _scale,
            SpriteEffects.FlipHorizontally,
            0f
        );
    }
}