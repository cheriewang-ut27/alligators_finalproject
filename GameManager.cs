using System.Collections.Generic;
using alligators_finalproject.Content;
using Microsoft.Xna.Framework;

namespace alligators_finalproject;

public enum GameState
{
    Menu,
    Playing,
    LevelTransition,
    Win,
    Lose,
}

public class GameManager
{
    public int Score { get; private set; } = 0;
    public int Health { get; private set; } = 100;
    public float TimeRemaining { get; private set; } = 70;
    public int CurrentLevel { get; private set; } = 1;
    public float InvincibilityTimer { get; private set; } = 0;
    private const float InvincibilityDuration = 1.5f;

    public int TargetScore => CurrentLevel * 5;
    public GameState CurrentState { get; set; } = GameState.Menu;

    public void AddScore(int amount)
    {
        Score += amount;
        
        if (Score >= TargetScore)
        {
            if (CurrentLevel < 3)
                CurrentState = GameState.LevelTransition;
            else
                CurrentState = GameState.Win;
        }
    }

    public void NextLevel()
    {
        CurrentLevel++; 
        TimeRemaining = 70;
        CurrentState = GameState.Playing;
    }
    

    public void UpdateTimer(GameTime gameTime)
    {
        if (CurrentState == GameState.Playing)
        {
            TimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (InvincibilityTimer > 0)
                InvincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                CurrentState = GameState.Lose;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (InvincibilityTimer <= 0)
        {
            Health -= amount;
            InvincibilityTimer = InvincibilityDuration;

            if (Health <= 0)
            {
                Health = 0;
                CurrentState = GameState.Lose;
            }
        }
    }
    

    public void StartGame()
    {
        Score = 0;
        Health = 100;
        TimeRemaining = 70;
        CurrentLevel = 1;
        CurrentState = GameState.Playing;
    }
}