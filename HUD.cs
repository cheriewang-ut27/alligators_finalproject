using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class HUD
{
    private SpriteFont _font;

    public HUD(SpriteFont font)
    {
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch, GameManager gm)
    {
        spriteBatch.DrawString(_font, "Time: " + Math.Ceiling(gm.TimeRemaining), new Vector2(20, 20), Color.White);
        spriteBatch.DrawString(_font, "Score: " + gm.Score, new Vector2(20, 70), Color.White);
        spriteBatch.DrawString(_font, "Health: " + gm.Health, new Vector2(20, 120), Color.White);
        spriteBatch.DrawString(_font, "WASD to move | Space to close mouth", new Vector2(20, 170), Color.White);

        if (gm.CurrentState == GameState.Menu)
        {
            spriteBatch.DrawString(_font, "Press SPACE to Start", new Vector2(500, 300), Color.Yellow);
        }
        else if (gm.CurrentState == GameState.Win)
        {
            spriteBatch.DrawString(_font, "YOU WIN!", new Vector2(550, 300), Color.Yellow);
            spriteBatch.DrawString(_font, "Final Score: " + gm.Score, new Vector2(550, 350), Color.White);
            spriteBatch.DrawString(_font, "Press R to Restart", new Vector2(550, 400), Color.White);
            spriteBatch.DrawString(_font, "Press ESC to Exit", new Vector2(550, 450), Color.White);
        }
        else if (gm.CurrentState == GameState.Lose)
        {
            spriteBatch.DrawString(_font, "YOU LOSE!", new Vector2(550, 300), Color.Red);
            spriteBatch.DrawString(_font, "Final Score: " + gm.Score, new Vector2(550, 350), Color.White);
            spriteBatch.DrawString(_font, "Press R to Restart", new Vector2(550, 400), Color.White);
            spriteBatch.DrawString(_font, "Press ESC to Exit", new Vector2(550, 450), Color.White);
        }
    }
}