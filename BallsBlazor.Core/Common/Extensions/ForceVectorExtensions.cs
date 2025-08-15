using BallsBlazor.Core.Environment;
using BallsBlazor.Core.Infrastructure.Models;

namespace BallsBlazor.Core.Common.Extensions;

public static class ForceVectorExtensions
{
    public static void Add(this ForceVector originalVector, ForceVector newVector)
    {
        originalVector.X += newVector.X * EnvironmentalVariables.TimeStep;
        originalVector.Y += newVector.Y * EnvironmentalVariables.TimeStep;
    }
}