namespace BallsBlazor.Core.Infrastructure.Models;

public class Ball
{
    public Velocity Velocity { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius { get; private set; }
    public double Mass { get; private set; }
    public string Color { get; private set; }
    public int Mode { get; private set; }

    public bool IsRolling { get; private set; }

    public Ball(Velocity velocity, double x, double y, double radius, string color, int mode = 0)
    {
        Radius = radius;
        Mass = radius * 100;
        X = x;
        Y = y;
        Velocity = velocity;
        Color = color;
        Mode = mode;
        IsRolling = false;
    }

    public void Move(double moveX, double moveY)
    {
        X += moveX;
        Y += moveY;
    }
}

public static class BallConstants
{
    public const int MaxRadius = 40;
    public const int MinRadius = 20;
}