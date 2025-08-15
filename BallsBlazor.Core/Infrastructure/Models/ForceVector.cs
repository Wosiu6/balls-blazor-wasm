namespace BallsBlazor.Core.Infrastructure.Models;

public class ForceVector
{
    public double X { get; set; }
    public double Y { get; set; }

    public ForceVector(double x, double y) => (X, Y) = (x, y);
}