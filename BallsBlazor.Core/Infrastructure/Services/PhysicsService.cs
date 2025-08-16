using BallsBlazor.Core.Environment;
using BallsBlazor.Core.Infrastructure.Models;

namespace BallsBlazor.Core.Infrastructure.Services;

public class PhysicsService
{
    private double _deltaT = 0.1;
    private double _deviceTiltX = 0;
    private double _deviceTiltY = 0.1;
    
    public async Task PerformMainCalculations(Ballfield ballfield)
    {
        await Task.Run(() => CalculateTouchedBalls(ballfield));
        
        // Main physics
        for (int t = 0; t < EnvironmentalVariables.TimeStep; t++)
        {
            for (int i = 0; i < ballfield.Balls.Count; i++)
            {
                Ball currentBall = ballfield.Balls[i];

                if (currentBall.Mode != 1)
                {
                    currentBall.X += currentBall.Velocity.X * _deltaT;
                    currentBall.Y += currentBall.Velocity.Y * _deltaT;

                    currentBall.Velocity.X -= currentBall.Velocity.X * EnvironmentalVariables.AirResistance;
                    currentBall.Velocity.Y -= currentBall.Velocity.Y * EnvironmentalVariables.AirResistance;

                    ReflectOffWalls(currentBall, ballfield);
                }
                else
                {
                    currentBall.Velocity.X = 0;
                    currentBall.Velocity.Y = 0;
                }

                ApplyBallInteractions(i, ballfield);
            }
            await Task.Run(() => BallPhysics.CheckDistance(ballfield));
        }

        await Task.Run(() => CalculateGravity(ballfield));
    }
    
    private void CalculateTouchedBalls(Ballfield ballfield)
    {
        // Physics for touched balls
        for (var i = 0; i < ballfield.TouchedBalls.Count; i++)
        {
            if (ballfield.TouchedBalls[i] < 0)
                continue;

            ballfield.TouchXAfter[i] = ballfield.TouchX[i];
            ballfield.TouchYAfter[i] = ballfield.TouchY[i];

            var ball = ballfield.Balls[i];

            if (ball.Mode != 1)
            {
                var factor = -0.5 / this._deltaT / 10.0;

                ball.Velocity.X = factor * (ball.X - ballfield.TouchXAfter[i]);
                ball.Velocity.Y = factor * (ball.Y - ballfield.TouchYAfter[i]);
            }
            else
            {
                ball.X = ballfield.TouchX[i];
                ball.Y = ballfield.TouchY[i];
            }
        }
    }
    
    void ReflectOffWalls(Ball currentBall, Ballfield ballfield)
    {
        if (currentBall.X + currentBall.Radius > ballfield.Width)
        {
            currentBall.Velocity.X *= -EnvironmentalVariables.WallElasticity;
            currentBall.X = ballfield.Width - currentBall.Radius;
        }
        else if (currentBall.X - currentBall.Radius < 0)
        {
            currentBall.Velocity.X *= -EnvironmentalVariables.WallElasticity;
            currentBall.X = currentBall.Radius;
        }

        if (currentBall.Y + currentBall.Radius > ballfield.Height)
        {
            currentBall.Velocity.Y *= -EnvironmentalVariables.WallElasticity;
            currentBall.Y = ballfield.Height - currentBall.Radius;
        }
        else if (currentBall.Y - currentBall.Radius < 0)
        {
            currentBall.Velocity.Y *= -EnvironmentalVariables.WallElasticity;
            currentBall.Y = currentBall.Radius;
        }
    }

    void ApplyBallInteractions(int i, Ballfield ballfield)
    {
        Ball currentBall = ballfield.Balls[i];

        if (currentBall.Mode == 3 && i != 0)
        {
            Ball previousBall = ballfield.Balls[i - 1];
            CalculateAndApplyInteraction(currentBall, previousBall);
        }

        if (currentBall.Mode != 1 && i != ballfield.Balls.Count - 1 && ballfield.Balls[i + 1].Mode == 3)
        {
            // Interaction with the next ball
            Ball nextBall = ballfield.Balls[i + 1];
            CalculateAndApplyInteraction(currentBall, nextBall);
        }
    }
    
    static void CalculateAndApplyInteraction(Ball ball1, Ball ball2)
    {
        double deltaX = ball1.X - ball2.X;
        double deltaY = ball1.Y - ball2.Y;

        ball1.Velocity.X -= 0.3 / ball1.Radius * (deltaX - 2 * ball1.Radius * deltaX / Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
        ball1.Velocity.Y -= 0.3 / ball1.Radius * (deltaY - 2 * ball1.Radius * deltaY / Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
    }

    private void CalculateGravity(Ballfield ballfield)
    {
        var gravitationalForceX = _deviceTiltX * EnvironmentalVariables.GravitationalStrength;
        var gravitationalForceY = _deviceTiltY * EnvironmentalVariables.GravitationalStrength;

        for (var i = 0; i < ballfield.Balls.Count; i++)
        {
            if (ballfield.Balls[i].Mode == 1) continue;
            
            ballfield.Balls[i].Velocity.X += gravitationalForceX;
            ballfield.Balls[i].Velocity.Y += gravitationalForceY;
        }
    }

    private void DoAccelerometer(Acceleration a)
    {
        this._deviceTiltX = -0.1 * a.X;
        this._deviceTiltY = 0.1 * a.Y;
    }
}