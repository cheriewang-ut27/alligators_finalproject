using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject.Content;

public class Fish : Creature
{
    private float directionChangeTimer = 0f;
    private float directionChangeInterval;
    private Random random = new Random();
    
    //fleeing radius
    private float minFleeRadius = 80f; // can tweak
    private float maxFleeRadius = 150f;
    private float fleeRadius;
    
    private int screenWidth;
    private int screenHeight;

    //Add screenWidth and screenHeight as parameters
    public Fish(Texture2D texture, Vector2 position, float minSpeed, float maxSpeed, int screenWidth, int screenHeight, float scale = 1f)
        : base(texture, position, 0, screenWidth, screenHeight, scale) // pass scale to base
    {
        speed = (float)(minSpeed + random.NextDouble() * (maxSpeed - minSpeed));
        directionChangeInterval = 1f + (float)random.NextDouble() * 2f;
        SetRandomDirection();
        fleeRadius = minFleeRadius + (float)random.NextDouble() * (maxFleeRadius - minFleeRadius);

        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
    }
    
    public override void Update(GameTime gameTime, Tilemap map)
    {
        base.Update(gameTime, map); //uses update from Creature
    }
    
    protected override void UpdateDirectionLogic(GameTime gameTime)
    {
        // Change direction randomly after interval
        directionChangeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (directionChangeTimer >= directionChangeInterval)
        {
            SetRandomDirection();
            directionChangeTimer = 0f;
            directionChangeInterval = 1f + (float)random.NextDouble() * 2f;
        }

        // Bounce off screen edges
        float scaledWidth = texture.Width * scale;
        float scaledHeight = texture.Height * scale;

        if (position.X <= 0)
        {
            position.X = 0;
            direction.X = Math.Abs(direction.X); // bounce right
        }
        else if (position.X + scaledWidth >= screenWidth)
        {
            position.X = screenWidth - scaledWidth;
            direction.X = -Math.Abs(direction.X); // bounce left
        }

        if (position.Y <= 0)
        {
            position.Y = 0;
            direction.Y = Math.Abs(direction.Y); // bounce down
        }
        else if (position.Y + scaledHeight >= screenHeight)
        {
            position.Y = screenHeight - scaledHeight;
            direction.Y = -Math.Abs(direction.Y); // bounce up
        }
    }
    
    private void SetRandomDirection()
    {
        do
        {
            direction = new Vector2(
                (float)(random.NextDouble() * 2 - 1), // -1 to 1
                (float)(random.NextDouble() * 2 - 1)  // -1 to 1
            );
        } while (Math.Abs(direction.X) < 0.2f || Math.Abs(direction.Y) < 0.2f);

        direction.Normalize();
    }
    
    public void FleeFrom(Vector2 predatorPosition)
    {
        Vector2 toPredator = predatorPosition - position;
        float distance = toPredator.Length();

        if (distance < fleeRadius && distance > 0)
        {
            Vector2 fleeDirection = position - predatorPosition;
            fleeDirection.Normalize();
            direction = Vector2.Lerp(direction, fleeDirection, 0.5f); // stronger reaction
            direction.Normalize();
        }
    }
    
    public void SpawnFish(Vector2 playerPosition, Tilemap map)
    {
        float minDistance = 300f;
        Vector2 newPos;
        bool inSand = true;
        
        do
        {
            int x = random.Next(50, screenWidth - 50);
            int y = random.Next(50, screenHeight - 50);
            newPos = new Vector2(x, y);
            
            inSand = map.IsTileImpassable(newPos);

        } while (inSand || Vector2.Distance(newPos, playerPosition) < minDistance);

        position = newPos;
    }
}