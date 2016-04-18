using UnityEngine;
using System.Collections;

[System.Serializable]
public struct FVector
{
    public FInt x;
    public FInt y;

    public FVector(FVector z)
    {
        x = new FInt(z.x);
        y = new FInt(z.y);
    }

    public FVector(FInt first, FInt second)
    {
        x = first;
        y = second;
    }

    public static FVector operator +(FVector x, FVector y)
    {
        FVector z = new FVector(x.x + y.x, x.y + y.y);
        return z;
    }

    public static FVector operator *(FVector x, FInt y)
    {
        FVector z = new FVector(x.x * y, x.y * y);
        return z;
    }

    public static FVector operator /(FVector x, FInt y)
    {
        FVector z = new FVector(x.x / y, x.y / y);
        return z;
    }

    // Normalize this vector
    public FVector Normalize()
    {
        FInt length = FInt.Sqrt(x * x + y * y);
        x = x / length;
        y = y / length;
        return this;
    }
}

[System.Serializable]
public class FInt : ISerializationCallbackReceiver
{
    public const int FLOATING_BITS = 16;

    [HideInInspector]
    public long rawValue;

    [SerializeField]
    private float floatValue;

    public void OnBeforeSerialize()
    {
        floatValue = ((float)rawValue) / (1 << FLOATING_BITS);
    }

    public void OnAfterDeserialize()
    {
        rawValue = (long)(floatValue * (1 << FLOATING_BITS));
    }

    // I'm not sure if this is necessary to get the functionality I want.
    // It didn't do anything initially.
    /*
    void OnGUI()
    {
        GUILayout.Label("Value: ");
        floatValue = float.Parse(GUILayout.TextField(floatValue.ToString(), GUILayout.Width(200)));
    }*/

    public FInt()
    {
    }

    public FInt(FInt other)
    {
        rawValue = other.rawValue;
    }

    public FInt(int value)
    {
        rawValue = ((long)value) << FLOATING_BITS;
    }

    public FInt(long value)
    {
        rawValue = value << FLOATING_BITS;
    }

    public FInt(float value)
    {
        rawValue = (long)(value * (1 << FLOATING_BITS));
    }

    public FInt(double value)
    {
        rawValue = (long)(value * (1 << FLOATING_BITS));
    }

    public static FInt RawFInt(int raw)
    {
        FInt z = new FInt(0);
        z.rawValue = (long)raw;
        return z;
    }

    public static FInt RawFInt(long raw)
    {
        FInt z = new FInt(0);
        z.rawValue = raw;
        return z;
    }

    public static FInt Zero()
    {
        return new FInt(0);
    }

    public static FInt One()
    {
        return new FInt(1);
    }

    // Largest signed long
    public static FInt Max()
    {
        FInt z = new FInt();
        z.rawValue = long.MaxValue;
        return z;
    }

    // Return largest FInt
    public static FInt Max(FInt a, FInt b)
    {
        FInt z = new FInt();
        z.rawValue = (a.rawValue > b.rawValue) ? a.rawValue : b.rawValue;
        return z;
    }

    // Smallest signed long
    public static FInt Min()
    {
        FInt z = new FInt();
        z.rawValue = long.MinValue;
        return z;
    }

    // Return smallest FInt
    public static FInt Min(FInt a, FInt b)
    {
        FInt z = new FInt();
        z.rawValue = (a.rawValue > b.rawValue) ? b.rawValue : a.rawValue;
        return z;
    }

    // Square root
    public static FInt Sqrt(FInt a)
    {
        FInt x = FInt.One();
        long rx = x.rawValue;
        long ax = a.rawValue;

        for (int i = 0; i < 64; i++)
        {
            rx = (rx + ax / rx) / 2;
        }

        x.rawValue = rx;

        return x;
    }

    // Sin
    public static FInt Sin(FInt a)
    {
        FInt x = new FInt(a);
        x = x 
          - x * x * x / 6 
          + x * x * x * x * x / 120 
          - x * x * x * x * x * x * x / 5040;

        return x;
    }

    // Cosine
    public static FInt Cos(FInt a)
    {
        FInt x = new FInt(a);
        x = FInt.One()
          - x * x / 4
          + x * x * x * x / 24
          - x * x * x * x * x * x / 720;

        return x;
    }

    // Tangent
    public static FInt Tan(FInt a)
    {
        FInt x = new FInt(a);
        x = x
          - x * x * x / 3
          + x * x * x * x * x * 2 / 15
          - x * x * x * x * x * x * x * 17 / 315
          - x * x * x * x * x * x * x * x * x * 62 / 2835;

        return x;
    }

    // Arc Tangent (inverse tangent)
    public static FInt Atan(FInt dx, FInt dy)
    {
        FInt x = new FInt(dy / dx);
        x = x
          - x * x * x / 3
          + x * x * x * x * x / 5
          - x * x * x * x * x * x * x / 7
          + x * x * x * x * x * x * x * x * x / 9
          - x * x * x * x * x * x * x * x * x * x * x / 11
          + x * x * x * x * x * x * x * x * x * x * x * x * x / 13;

        // TODO: Adjust for dx or dy being negative
        // BREAKS: Certain directions will give incorrect values

        return x;
    }

    // Return random FInt within a range. Includes both end points.
    public static FInt RandomRange(System.Random seed, FInt min, FInt max)
    {
        FInt r = FInt.RawFInt(seed.Next());

        FInt diff = max - min;

        FInt x = r * diff / FInt.RawFInt(4294967296) + min;

        return min;
    }

    public int ToInt()
    {
        return (int)(rawValue >> FLOATING_BITS);
    }

    public int Round()
    {
        long flat = rawValue >> FLOATING_BITS;
        long remainder = rawValue - (flat << FLOATING_BITS);
        if (remainder > (long)(1 << (FLOATING_BITS - 1)))
        {
            return (int)flat + 1;
        }
        return (int)flat;
    }

    public float ToFloat()
    {
        return ((float)rawValue) / (1 << FLOATING_BITS);
    }

    public FInt Abs()
    {
        FInt z = new FInt();
        z.rawValue = (rawValue < 0) ? -rawValue : rawValue;
        return z;
    }

    public int FractionalBits()
    {
        long flat = rawValue >> FLOATING_BITS;
        long remainder = rawValue - (flat << FLOATING_BITS);
        return (int)remainder;
    }

    public static FInt operator +(FInt x, FInt y)
    {
        FInt z = new FInt(x);
        z.rawValue += y.rawValue;
        return z;
    }

    public static FInt operator +(FInt x, int y)
    {
        FInt z = new FInt(x);
        z.rawValue += ((long)y) << FLOATING_BITS;
        return z;
    }

    public static FInt operator -(FInt x, FInt y)
    {
        FInt z = new FInt(x);
        z.rawValue -= y.rawValue;
        return z;
    }

    public static FInt operator -(FInt x)
    {
        FInt z = new FInt(x);
        z.rawValue = -z.rawValue;
        return z;
    }

    public static FInt operator *(FInt x, FInt y)
    {
        FInt z = new FInt();
        z.rawValue = (x.rawValue * y.rawValue) >> FLOATING_BITS;
        return z;
    }

    public static FInt operator *(FInt x, int y)
    {
        FInt z = new FInt();
        z.rawValue = x.rawValue * y;
        return z;
    }

    public static FInt operator *(int y, FInt x)
    {
        FInt z = new FInt();
        z.rawValue = x.rawValue * y;
        return z;
    }

    public static FInt operator /(FInt x, FInt y)
    {
        FInt z = new FInt();
        z.rawValue = (x.rawValue << FLOATING_BITS) / y.rawValue;
        return z;
    }

    public static FInt operator /(FInt x, int y)
    {
        FInt z = new FInt();
        z.rawValue = x.rawValue / y;
        return z;
    }

    public override int GetHashCode()
    {
        return rawValue.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        FInt other = (FInt)obj;
        if (other == null)
        {
            return false;
        }
        return rawValue == other.rawValue;
    }

    public static bool operator ==(FInt x, FInt y)
    {
        if (object.ReferenceEquals(null, x) && object.ReferenceEquals(null, y))
        {
            return true;
        }
        else if (object.ReferenceEquals(null, x) || object.ReferenceEquals(null, y))
        {
            return false;
        }
        return x.rawValue == y.rawValue;
    }

    public static bool operator !=(FInt x, FInt y)
    {
        if (object.ReferenceEquals(null, x) && object.ReferenceEquals(null, y))
        {
            return false;
        }
        else if (object.ReferenceEquals(null, x) || object.ReferenceEquals(null, y))
        {
            return true;
        }
        return x.rawValue != y.rawValue;
    }

    public static bool operator >(FInt x, FInt y)
    {
        return x.rawValue > y.rawValue;
    }

    public static bool operator >=(FInt x, FInt y)
    {
        return x.rawValue >= y.rawValue;
    }

    public static bool operator <(FInt x, FInt y)
    {
        return x.rawValue < y.rawValue;
    }

    public static bool operator <=(FInt x, FInt y)
    {
        return x.rawValue <= y.rawValue;
    }

    public override string ToString()
    {
        return (((float)rawValue) / (1 << FLOATING_BITS)).ToString();
    }
}
