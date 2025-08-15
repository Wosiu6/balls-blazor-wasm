using System.Drawing;

namespace BallsBlazor.Core.Common.Utils;

public static class DrawingUtils
{
    public static Color GenerateRandomColor()
    {
        Random random = new();
        return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
    }
}