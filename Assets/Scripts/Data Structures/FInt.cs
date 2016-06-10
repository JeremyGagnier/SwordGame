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

    public static FInt PI()
    {
        return new FInt(3.14159);
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
        if (a.rawValue == 0)
        {
            return FInt.Zero();
        }
        FInt x = FInt.One();
        for (int i = 0; i < 64; i++)
        {
            x = (x + a / x) / 2;
        }

        return x;
    }

    // Sin
    public static FInt Sin(FInt a)
    {
        FInt x = a % (2 * PI());
        bool overPi = x > PI();
        if (overPi)
        {
            x -= PI();
        }
        x = x 
          - x * x * x / 6 
          + x * x * x * x * x / 120 
          - x * x * x * x * x * x * x / 5040
          + x * x * x * x * x * x * x * x * x / 362880;
        if (overPi)
        {
            x = -x;
        }
        return x;
    }

    // Cosine
    public static FInt Cos(FInt a)
    {
        FInt x = a % (2 * PI());
        bool overPi = x > PI();
        if (overPi)
        {
            x -= PI();
        }
        x = FInt.One()
          - x * x / 2
          + x * x * x * x / 24
          - x * x * x * x * x * x / 720
          + x * x * x * x * x * x * x * x / 40320
          - x * x * x * x * x * x * x * x * x * x / 3628800;
        if (overPi)
        {
            x = -x;
        }
        return x;
    }

    // Tangent
    public static FInt Tan(FInt a)
    {
        FInt x = a % (2 * PI());
        x = x
          - x * x * x / 3
          + x * x * x * x * x * 2 / 15
          - x * x * x * x * x * x * x * 17 / 315
          - x * x * x * x * x * x * x * x * x * 62 / 2835;

        return x;
    }

    // Arc Tangent (inverse tangent)
    // Uses GBA implementation: http://www.coranac.com/documents/arctangent/
    public static FInt Atan(FInt dx, FInt dy)
    {
        if (dx.rawValue == 0)
        {
            if (dy.rawValue >= 0)
            {
                return new FInt(0.5) * PI();
            }
            else
            {
                return new FInt(1.5) * PI();
            }
        }
        if (dy.rawValue == 0)
        {
            if (dx.rawValue > 0)
            {
                return FInt.Zero();
            }
            else
            {
                return PI();
            }
        }
        
        if (dx.rawValue < 0)
        {
            if (dy.rawValue < 0)
            {
                return PI() + Atan(-dx, -dy);
            }
            else
            {
                return PI() - Atan(-dx, dy);
            }
        }
        else
        {
            if (dy.rawValue < 0)
            {
                return 2 * PI() - Atan(dx, -dy);
            }
        }

        bool swapAxes = false;
        if (dy > dx)
        {
            FInt tmp = dy;
            dy = dx;
            dx = tmp;
            swapAxes = true;
        }

        long t = (dy.rawValue << FLOATING_BITS) / dx.rawValue;
        long t2 = -(t * t) >> FLOATING_BITS;
        long t4 = (t2 * t2) >> FLOATING_BITS;
        long t6 = (t4 * t2) >> FLOATING_BITS;
        long t8 = (t4 * t4) >> FLOATING_BITS;
        long t10 = (t6 * t4) >> FLOATING_BITS;
        long t12 = (t6 * t6) >> FLOATING_BITS;
        long t14 = (t8 * t6) >> FLOATING_BITS;
        FInt x = RawFInt(
            ((65536 * t) >> FLOATING_BITS) +
            ((((21845 * t) >> FLOATING_BITS) * t2) >> FLOATING_BITS) +
            ((((13107 * t) >> FLOATING_BITS) * t4) >> FLOATING_BITS) +
            ((((9102 * t) >> FLOATING_BITS) * t6) >> FLOATING_BITS) +
            ((((6317 * t) >> FLOATING_BITS) * t8) >> FLOATING_BITS) +
            ((((3664 * t) >> FLOATING_BITS) * t10) >> FLOATING_BITS) +
            ((((1432 * t) >> FLOATING_BITS) * t12) >> FLOATING_BITS) +
            ((((266 * t) >> FLOATING_BITS) * t14) >> FLOATING_BITS));
        
        if (swapAxes)
        {
            return PI() / 2 - x;
        }
        return x;
    }

    // Return random FInt within a range. Includes both end points.
    public static FInt RandomRange(System.Random seed, FInt min, FInt max)
    {
        FInt r = FInt.RawFInt(seed.Next());
        FInt diff = max - min;
        return (r * diff) / FInt.RawFInt(4294967296) + min;
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

    public static FInt operator %(FInt x, FInt y)
    {
        FInt z = new FInt();
        z.rawValue = x.rawValue % y.rawValue;
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
