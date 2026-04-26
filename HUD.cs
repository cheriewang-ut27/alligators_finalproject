using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class HUD
{
    private SpriteFont _font;
    private Texture2D _pixel; 

    public HUD(SpriteFont font, GraphicsDevice graphics)
    {
        _font = font;
        _pixel = new Texture2D(graphics, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    private void DrawStringWithShadow(SpriteBatch spriteBatch, string text, Vector2 pos, Color color)
    {
        spriteBatch.DrawString(_font, text, pos + new Vector2(2, 2), Color.Black * 0.5f);
        spriteBatch.DrawString(_font, text, pos, color);
    }

    public void Draw(SpriteBatch spriteBatch, GameManager gm, Rectangle screenBounds)
    {
        int centerX = screenBounds.Width / 2;
        int centerY = screenBounds.Height / 2;

        if (gm.CurrentState == GameState.Menu)
        {
            string title = "ALLIGATOR";
            string subTitle = "Press [SPACE] to Start";
            
            Vector2 titleSize = _font.MeasureString(title);
            DrawStringWithShadow(spriteBatch, title, new Vector2(centerX - titleSize.X/2, centerY - 100), Color.Cyan);
            DrawStringWithShadow(spriteBatch, subTitle, new Vector2(centerX - _font.MeasureString(subTitle).X/2, centerY - 20), Color.Yellow);
        }
        else 
        {
            DrawStringWithShadow(spriteBatch, $"LEVEL: {gm.CurrentLevel}", new Vector2(20, 20), Color.Lime);
            DrawStringWithShadow(spriteBatch, $"SCORE: {gm.Score} / {gm.TargetScore}", new Vector2(20, 60), Color.White);
            
            string timeText = $"TIME: {Math.Ceiling(gm.TimeRemaining)}s";
            Vector2 timeSize = _font.MeasureString(timeText);
            DrawStringWithShadow(spriteBatch, timeText, new Vector2(screenBounds.Width - timeSize.X - 20, 20), 
                                 gm.TimeRemaining < 10 ? Color.Red : Color.Gold);
            
            int barWidth = 200;
            int barHeight = 20;
            Vector2 barPos = new Vector2(20, screenBounds.Height - 40);
            
            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, barHeight), Color.DarkRed * 0.5f);
            float healthPercent = (float)gm.Health / 100f;
            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, (int)(barWidth * healthPercent), barHeight), Color.LimeGreen);
            DrawStringWithShadow(spriteBatch, "HP", new Vector2(barPos.X + barWidth + 10, barPos.Y - 5), Color.White);

            if (gm.CurrentState == GameState.Win || gm.CurrentState == GameState.Lose)
            {
                string mainMsg = gm.CurrentState == GameState.Win ? "VICTORY!" : "GAME OVER!";
                Color msgColor = gm.CurrentState == GameState.Win ? Color.Gold : Color.OrangeRed;
                
                Vector2 sz = _font.MeasureString(mainMsg);
                DrawStringWithShadow(spriteBatch, mainMsg, new Vector2(centerX - sz.X/2, centerY - 50), msgColor);
                DrawStringWithShadow(spriteBatch, "Press [R] to Restart", new Vector2(centerX - 120, centerY + 20), Color.White);
            }

            if (gm.CurrentState == GameState.LevelTransition)
            {
                string msg = $"LEVEL {gm.CurrentLevel} COMPLETE!";
                DrawStringWithShadow(spriteBatch, msg, new Vector2(centerX - _font.MeasureString(msg).X/2, centerY - 50), Color.Cyan);
                DrawStringWithShadow(spriteBatch, "Press [X] for Next Level", new Vector2(centerX - 180, centerY + 20), Color.White);
            }
        }
    }
}