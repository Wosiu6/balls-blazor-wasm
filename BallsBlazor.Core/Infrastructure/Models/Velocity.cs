namespace BallsBlazor.Core.Infrastructure.Models;

public class Velocity
{
    public double X { get; set; }
    public double Y { get; set; }

    public Velocity(double x, double y) => (X, Y) = (x, y);
}