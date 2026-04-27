using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class Background
{
    private Texture2D _sheet;
    private int _rows;
    private int _cols;
    private float _frameTime;
    private float _timer;
    private int _currentFrame;
    private int _totalFrames;

    public Background(Texture2D sheet, int rows, int cols, float frameTime)
    {
        _sheet = sheet;
        _rows = rows;
        _cols = cols;
        _frameTime = frameTime;
        _totalFrames = rows * cols;
    }

    public void Update(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= _frameTime)
        {
            _timer = 0;
            _currentFrame = (_currentFrame + 1) % _totalFrames;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle screenBounds, float scale = 1.0f)
    {
        int frameWidth = _sheet.Width / _cols;
        int frameHeight = _sheet.Height / _rows;

        int row = _currentFrame / _cols;
        int col = _currentFrame % _cols;
        Rectangle sourceRect = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
        
        int drawWidth = (int)(frameWidth * scale);
        int drawHeight = (int)(frameHeight * scale);
        
        for (int x = 0; x < screenBounds.Width; x += drawWidth)
        {
            for (int y = 0; y < screenBounds.Height; y += drawHeight)
            {
                spriteBatch.Draw(
                    _sheet, 
                    new Vector2(x, y), 
                    sourceRect, 
                    Color.White, 
                    0f, 
                    Vector2.Zero, 
                    scale, 
                    SpriteEffects.None, 
                    0f
                );
            }
        }
    }
}