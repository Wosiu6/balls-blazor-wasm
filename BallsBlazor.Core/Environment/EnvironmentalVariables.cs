namespace BallsBlazor.Core.Environment;

public static class EnvironmentalVariables
{
    public static double TimeStep { get; set; } = 3.0;
    public static double MinimumSpeed { get; set; } = 3.0;
    public static double MaximumSpeed { get; set; } = 10.0;
    public static double AirResistance { get; set; } = 0;
    public static double GravitationalStrength { get; set; } = 1.0;
    public static double WallElasticity { get; set; } = 0.95;
    public static bool IsPaused { get; set; } = false;
}