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
            DrawStringWithShadow(spriteBatch, subTitle, new Vector2(centerX - _font.MeasureString(subTitle).X / 2, centerY - 20), Color.Yellow);

            string[] rules =
            {
                "Eat 5 fish to complete each level",
                "Complete 3 levels to win",
                "Avoid jellyfish or get zapped!",
                "Move with WASD",
                "Press SPACE to chomp"
            };

            for (int i = 0; i < rules.Length; i++)
            {
                string rule = rules[i];
                Vector2 ruleSize = _font.MeasureString(rule);

                DrawStringWithShadow(
                    spriteBatch,
                    rule,
                    new Vector2(centerX - ruleSize.X / 2, centerY + 40 + i * 30),
                    Color.White
                );
            }
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
                string restartMsg = "Press [R] to Restart";

                Color msgColor = gm.CurrentState == GameState.Win ? Color.Gold : Color.OrangeRed;
    
                Vector2 mainMsgSize = _font.MeasureString(mainMsg);
                Vector2 restartMsgSize = _font.MeasureString(restartMsg);

                DrawStringWithShadow(spriteBatch, mainMsg, new Vector2(centerX - mainMsgSize.X / 2, centerY - 50), msgColor);

                DrawStringWithShadow(spriteBatch, restartMsg, new Vector2(centerX - restartMsgSize.X / 2, centerY + 20), Color.White);
            }

            if (gm.CurrentState == GameState.LevelTransition)
            {
                string msg = $"LEVEL {gm.CurrentLevel} COMPLETE!";
                string nextMsg = "Press [X] for Next Level";

                Vector2 msgSize = _font.MeasureString(msg);
                Vector2 nextMsgSize = _font.MeasureString(nextMsg);

                DrawStringWithShadow(spriteBatch, msg, new Vector2(centerX - msgSize.X / 2, centerY - 50), Color.Cyan);

                DrawStringWithShadow(spriteBatch, nextMsg, new Vector2(centerX - nextMsgSize.X / 2, centerY + 20), Color.White);
            }
        }
    }
}