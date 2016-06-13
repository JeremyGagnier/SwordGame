public class Collision
{
    public static FInt Distance(FInt x1, FInt y1, FInt x2, FInt y2)
    {
        return FInt.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }

    public static bool CircleToCircle(FVector p1, FVector p2, FInt r1, FInt r2)
    {
        return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) <= (r1 + r2) * (r1 + r2);
    }
}
