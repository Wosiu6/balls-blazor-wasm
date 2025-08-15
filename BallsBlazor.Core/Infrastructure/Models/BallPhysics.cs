namespace BallsBlazor.Core.Infrastructure.Models;

public static class BallPhysics
{
    public static void CheckDistance(Ballfield ballfield)
    {
        int numberOfBalls = ballfield.Balls.Count;

        for (var i = 0; i < numberOfBalls - 1; i++)
        {
            for (var j = i + 1; j < numberOfBalls; j++)
            {
                Ball currentTopBall = ballfield.Balls[i];
                Ball currentBottomBall = ballfield.Balls[j];

                if (Math.Sqrt((currentTopBall.X - currentBottomBall.X) * (currentTopBall.X - currentBottomBall.X) + (currentTopBall.Y - currentBottomBall.Y) * (currentTopBall.Y - currentBottomBall.Y))
                    < (currentTopBall.Radius + currentBottomBall.Radius) || (currentBottomBall.Mode == 2 && j == i + 1))
                {
                    CalculateVelocityRefactored(currentTopBall, currentBottomBall);
                }
            }
        }
    }

    public static void CalculateVelocity(Ball currentTopBall, Ball currentBottomBall)
    {
        double deltaTopX = currentTopBall.X - currentBottomBall.X;
        double deltaTopY = currentTopBall.Y - currentBottomBall.Y;

        double deltaTopR = deltaTopX * deltaTopX + deltaTopY * deltaTopY;

        double deltaBottomX = currentBottomBall.X - currentTopBall.X;
        double deltaBottomY = currentBottomBall.Y - currentTopBall.Y;
        double deltaBottomR = deltaBottomX * deltaBottomX + deltaBottomY * deltaBottomY;

        if (deltaTopR != 0)
        {
            double velocityXTop = ((currentBottomBall.Velocity.X * deltaBottomX + currentBottomBall.Velocity.Y * deltaBottomY) * deltaBottomX + (currentTopBall.Velocity.X * deltaTopY - currentTopBall.Velocity.Y * deltaTopX) * deltaTopY) / deltaTopR;
            double velocityYTop = ((currentBottomBall.Velocity.X * deltaBottomX + currentBottomBall.Velocity.Y * deltaBottomY) * deltaBottomY - (currentTopBall.Velocity.X * deltaTopY - currentTopBall.Velocity.Y * deltaTopX) * deltaTopX) / deltaTopR;
            double velocityXBottom = ((currentTopBall.Velocity.X * deltaTopX + currentTopBall.Velocity.Y * deltaTopY) * deltaTopX + (currentBottomBall.Velocity.X * deltaBottomY - currentBottomBall.Velocity.Y * deltaBottomX) * deltaBottomY) / deltaBottomR;
            double velocityYBottom = ((currentTopBall.Velocity.X * deltaTopX + currentTopBall.Velocity.Y * deltaTopY) * deltaTopY - (currentBottomBall.Velocity.X * deltaBottomY - currentBottomBall.Velocity.Y * deltaBottomX) * deltaBottomX) / deltaBottomR;
            
            currentTopBall.Velocity.X = velocityXTop;
            currentTopBall.Velocity.Y = velocityYTop;
            currentBottomBall.Velocity.X = velocityXBottom;
            currentBottomBall.Velocity.Y = velocityYBottom;

            double sqrtDeltaTop = Math.Sqrt(deltaTopR);
            double sqrtDeltaBottom = Math.Sqrt(deltaBottomR);

            double firstRatio = currentBottomBall.Radius / currentTopBall.Radius;
            double secondRatio = currentTopBall.Radius / currentBottomBall.Radius;
            double inverseRatio = firstRatio / secondRatio;

            if (currentTopBall.Mode != 1)
            {
                currentTopBall.X += (currentBottomBall.Radius + currentTopBall.Radius - sqrtDeltaTop) * (deltaTopX / sqrtDeltaTop) * secondRatio;
                currentTopBall.Y += (currentBottomBall.Radius + currentTopBall.Radius - sqrtDeltaTop) * (deltaTopY / sqrtDeltaTop) * secondRatio;
            }
            if (currentBottomBall.Mode != 1)
            {
                currentBottomBall.X += (currentTopBall.Radius + currentBottomBall.Radius - sqrtDeltaBottom) * (deltaBottomX / sqrtDeltaBottom) * inverseRatio;
                currentBottomBall.Y += (currentTopBall.Radius + currentBottomBall.Radius - sqrtDeltaBottom) * (deltaBottomY / sqrtDeltaBottom) * inverseRatio;
            }
        }
    }

    public static void CalculateVelocityRefactored(Ball currentTopBall, Ball currentBottomBall)
    {
        double deltaTopX = currentTopBall.X - currentBottomBall.X;
        double deltaTopY = currentTopBall.Y - currentBottomBall.Y;
        double deltaTopR = deltaTopX * deltaTopX + deltaTopY * deltaTopY;

        if (deltaTopR == 0)
        {
            return; // Avoid division by zero
        }

        double deltaBottomX = -deltaTopX;
        double deltaBottomY = -deltaTopY;

        double dotProductTop = currentTopBall.Velocity.X * deltaTopX + currentTopBall.Velocity.Y * deltaTopY;
        double dotProductBottom = currentBottomBall.Velocity.X * deltaBottomX + currentBottomBall.Velocity.Y * deltaBottomY;

        double velocityXTop = (dotProductBottom * deltaBottomX + (currentTopBall.Velocity.X * deltaTopY - currentTopBall.Velocity.Y * deltaTopX) * deltaTopY) / deltaTopR;
        double velocityYTop = (dotProductBottom * deltaBottomY - (currentTopBall.Velocity.X * deltaTopY - currentTopBall.Velocity.Y * deltaTopX) * deltaTopX) / deltaTopR;

        double velocityXBottom = (dotProductTop * deltaTopX + (currentBottomBall.Velocity.X * deltaBottomY - currentBottomBall.Velocity.Y * deltaBottomX) * deltaBottomY) / deltaTopR;
        double velocityYBottom = (dotProductTop * deltaTopY - (currentBottomBall.Velocity.X * deltaBottomY - currentBottomBall.Velocity.Y * deltaBottomX) * deltaBottomX) / deltaTopR;

        currentTopBall.Velocity = new Velocity(velocityXTop, velocityYTop);
        currentBottomBall.Velocity = new Velocity(velocityXBottom, velocityYBottom);

        double sqrtDelta = Math.Sqrt(deltaTopR);

        double radiusSum = currentTopBall.Radius + currentBottomBall.Radius;
        double topRatio = currentBottomBall.Radius / radiusSum;
        double bottomRatio = currentTopBall.Radius / radiusSum;

        double commonFactor = radiusSum - sqrtDelta;

        if (currentTopBall.Mode != 1) // top ball
        {
            double moveX = commonFactor * (deltaTopX / sqrtDelta) * bottomRatio;
            double moveY = commonFactor * (deltaTopY / sqrtDelta) * bottomRatio;

            currentTopBall.Move(moveX, moveY);
        }

        if (currentBottomBall.Mode != 1) // bottom ball
        {
            double moveX = commonFactor * (deltaBottomX / sqrtDelta) * topRatio;
            double moveY = commonFactor * (deltaBottomY / sqrtDelta) * topRatio;

            currentBottomBall.Move(moveX, moveY);
        }
    }

    public static void CalculateVelocityRefactoredWithMass(Ball currentTopBall, Ball currentBottomBall)
    {
        double deltaTopX = currentTopBall.X - currentBottomBall.X;
        double deltaTopY = currentTopBall.Y - currentBottomBall.Y;
        double deltaTopR = deltaTopX * deltaTopX + deltaTopY * deltaTopY;

        if (deltaTopR == 0)
        {
            return; // Avoid division by zero
        }

        double deltaBottomX = -deltaTopX;
        double deltaBottomY = -deltaTopY;

        double dotProductTop = currentTopBall.Velocity.X * deltaTopX + currentTopBall.Velocity.Y * deltaTopY;
        double dotProductBottom = currentBottomBall.Velocity.X * deltaBottomX + currentBottomBall.Velocity.Y * deltaBottomY;

        // Calculate the relative mass of the balls
        double totalMass = currentTopBall.Mass + currentBottomBall.Mass;
        double massTop = currentTopBall.Mass / totalMass;
        double massBottom = currentBottomBall.Mass / totalMass;

        double velocityXTop = (dotProductBottom * deltaBottomX + (currentTopBall.Velocity.X * deltaTopY - currentTopBall.Velocity.Y * deltaTopX) * deltaTopY) / deltaTopR * (0.5 + massTop);
        double velocityYTop = (dotProductBottom * deltaBottomY - (currentTopBall.Velocity.X * deltaTopY - currentTopBall.Velocity.Y * deltaTopX) * deltaTopX) / deltaTopR * (0.5 + massTop);

        double velocityXBottom = (dotProductTop * deltaTopX + (currentBottomBall.Velocity.X * deltaBottomY - currentBottomBall.Velocity.Y * deltaBottomX) * deltaBottomY) / deltaTopR * (0.5 + massBottom);
        double velocityYBottom = (dotProductTop * deltaTopY - (currentBottomBall.Velocity.X * deltaBottomY - currentBottomBall.Velocity.Y * deltaBottomX) * deltaBottomX) / deltaTopR * (0.5 + massBottom);

        currentTopBall.Velocity = new Velocity(velocityXTop, velocityYTop);
        currentBottomBall.Velocity = new Velocity(velocityXBottom, velocityYBottom);

        double sqrtDelta = Math.Sqrt(deltaTopR);

        double radiusSum = currentTopBall.Radius + currentBottomBall.Radius;
        double topRatio = currentBottomBall.Radius / radiusSum;
        double bottomRatio = currentTopBall.Radius / radiusSum;

        double commonFactor = radiusSum - sqrtDelta;

        if (currentTopBall.Mode != 1) // top ball
        {
            double moveX = commonFactor * (deltaTopX / sqrtDelta) * bottomRatio;
            double moveY = commonFactor * (deltaTopY / sqrtDelta) * bottomRatio;

            currentTopBall.Move(moveX, moveY);
        }

        if (currentBottomBall.Mode != 1) // bottom ball
        {
            double moveX = commonFactor * (deltaBottomX / sqrtDelta) * topRatio;
            double moveY = commonFactor * (deltaBottomY / sqrtDelta) * topRatio;

            currentBottomBall.Move(moveX, moveY);
        }
    }

    public static void CalculateVelocityWithMass(Ball currentTopBall, Ball currentBottomBall)
    {
        // Calculate the relative position and squared distance
        double deltaTopX = currentTopBall.X - currentBottomBall.X;
        double deltaTopY = currentTopBall.Y - currentBottomBall.Y;
        double deltaTopR = deltaTopX * deltaTopX + deltaTopY * deltaTopY;

        // Avoid division by zero
        if (deltaTopR == 0)
        {
            return;
        }

        double radiusSum = currentTopBall.Radius + currentBottomBall.Radius;
        double topRatio = currentBottomBall.Radius / radiusSum;
        double bottomRatio = currentTopBall.Radius / radiusSum;

        // Calculate the relative mass of the balls
        double totalMass = currentTopBall.Mass + currentBottomBall.Mass;
        double massTop = currentTopBall.Mass / totalMass;
        double massBottom = currentBottomBall.Mass / totalMass;

        // Calculate dot products for both balls
        double dotProductTop = (currentTopBall.Velocity.X * deltaTopX + currentTopBall.Velocity.Y * deltaTopY);
        double dotProductBottom = (currentBottomBall.Velocity.X * -deltaTopX + currentBottomBall.Velocity.Y * -deltaTopY);

        // Calculate the new velocities
        double commonFactor = (2 * dotProductTop) / deltaTopR;

        double velocityXTop = currentTopBall.Velocity.X - commonFactor * deltaTopX * massTop;
        double velocityYTop = currentTopBall.Velocity.Y - commonFactor * deltaTopY * massTop;

        double velocityXBottom = currentBottomBall.Velocity.X - commonFactor * -deltaTopX * massBottom;
        double velocityYBottom = currentBottomBall.Velocity.Y - commonFactor * -deltaTopY * massBottom;

        // Update velocities
        currentTopBall.Velocity = new Velocity(velocityXTop, velocityYTop);
        currentBottomBall.Velocity = new Velocity(velocityXBottom, velocityYBottom);

        double sqrtDeltaTop = Math.Sqrt(deltaTopR);

        // Calculate movement for both balls
        if (currentTopBall.Mode != 1)
        {
            double moveX = (radiusSum - sqrtDeltaTop) * (deltaTopX / sqrtDeltaTop) * bottomRatio;
            double moveY = (radiusSum - sqrtDeltaTop) * (deltaTopY / sqrtDeltaTop) * bottomRatio;
            currentTopBall.Move(moveX, moveY);
        }

        if (currentBottomBall.Mode != 1)
        {
            double moveX = (radiusSum - sqrtDeltaTop) * (-deltaTopX / sqrtDeltaTop) * topRatio;
            double moveY = (radiusSum - sqrtDeltaTop) * (-deltaTopY / sqrtDeltaTop) * topRatio;
            currentBottomBall.Move(moveX, moveY);
        }
    }
}