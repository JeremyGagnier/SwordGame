using UnityEngine;

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
    
    public static FVector operator -(FVector x, FVector y)
    {
        FVector z = new FVector(x.x - y.x, x.y - y.y);
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
        if (length.Value() == 0)
        {
            return this;
        }
        x = x / length;
        y = y / length;
        return this;
    }

    public FVector Rotate(FInt angle)
    {
        FInt oldAngle = FInt.Atan(x, y);
        FInt newAngle = oldAngle + angle;
        FInt length = FInt.Sqrt(x * x + y * y);
        x = FInt.Cos(newAngle) * length;
        y = FInt.Sin(newAngle) * length;
        return this;
    }

    public override string ToString()
    {
        return "FVector x: " + x.ToString() + ", y: " + y.ToString();
    }
}

[System.Serializable]
public struct FInt : ISerializationCallbackReceiver
{
    public const int FLOATING_BITS = 16;
    private const long SHIFT = (long)(1 << FLOATING_BITS);
    private const float FSHIFT = (float)(1 << FLOATING_BITS);
    private const double DSHIFT = (double)(1 << FLOATING_BITS);
    public static FInt PI = new FInt(3.14159);

    #region Critical Variables
    private long value;
    #endregion

    #region Serialization
    [SerializeField]
    private float floatValue;

    public void OnBeforeSerialize()
    {
        floatValue = ((float)value) / FSHIFT;
    }

    public void OnAfterDeserialize()
    {
        value = (long)(floatValue * FSHIFT);
    }
    #endregion

    #region Constructors
    public FInt(FInt x)
    {
        value = x.Value();
        floatValue = (float)(value / FSHIFT);
    }

    // This constructor is special because it assumes the long is raw
    public FInt(long x)
    {
        value = x;
        floatValue = (float)(value / FSHIFT);
    }

    public FInt(int x)
    {
        value = ((long)x) * SHIFT;
        floatValue = (float)(value / FSHIFT);
    }

    public FInt(double x)
    {
        value = (long)(x * DSHIFT);
        floatValue = (float)(value / FSHIFT);
    }

    public FInt(float x)
    {
        value = (long)(x * FSHIFT);
        floatValue = (float)(value / FSHIFT);
    }
    #endregion

    #region Implicit Casts
    public static implicit operator FInt(long x)
    {
        return new FInt(x);
    }
    #endregion

    #region Getters
    // This gives the raw value, all the others strip information
    public long Value()
    {
        return value;
    }

    public long ToLong()
    {
        return value / SHIFT;
    }

    public int ToInt()
    {
        return (int)(value / SHIFT);
    }

    public double ToDouble()
    {
        return ((double)value) / DSHIFT;
    }

    public float ToFloat()
    {
        return ((float)value) / FSHIFT;
    }

    public override string ToString()
    {
        return (((double)value) / DSHIFT).ToString();
    }
    #endregion

    #region Operator Overloads
    #region + Overload
    public static FInt operator +(FInt x, FInt y)
    {
        return new FInt(x.Value() + y.Value());
    }

    public static FInt operator +(FInt x, long y)
    {
        return new FInt(x.Value() + y * SHIFT);
    }

    public static FInt operator +(long x, FInt y)
    {
        return new FInt(y.Value() + x * SHIFT);
    }

    public static FInt operator +(FInt x, int y)
    {
        return new FInt(x.Value() + ((long)y) * SHIFT);
    }

    public static FInt operator +(int x, FInt y)
    {
        return new FInt(y.Value() + ((long)x) * SHIFT);
    }
    #endregion

    #region - Overload
    public static FInt operator -(FInt x, FInt y)
    {
        return new FInt(x.Value() - y.Value());
    }

    public static FInt operator -(FInt x, long y)
    {
        return new FInt(x.Value() - y * SHIFT);
    }

    public static FInt operator -(long x, FInt y)
    {
        return new FInt(x * SHIFT - y.Value());
    }

    public static FInt operator -(FInt x, int y)
    {
        return new FInt(x.Value() - ((long)y) * SHIFT);
    }

    public static FInt operator -(int x, FInt y)
    {
        return new FInt(((long)x) * SHIFT - y.Value());
    }

    public static FInt operator -(FInt x)
    {
        return new FInt(-x.Value());
    }
    #endregion

    #region * Overload
    public static FInt operator *(FInt x, FInt y)
    {
        return new FInt((x.Value() * y.Value()) / SHIFT);
    }

    public static FInt operator *(FInt x, long y)
    {
        return new FInt(x.Value() * y);
    }

    public static FInt operator *(long x, FInt y)
    {
        return new FInt(x * y.Value());
    }

    public static FInt operator *(FInt x, int y)
    {
        return new FInt(x.Value() * ((long)y));
    }

    public static FInt operator *(int x, FInt y)
    {
        return new FInt(((long)x) * y.Value());
    }
    #endregion

    #region / Overload
    public static FInt operator /(FInt x, FInt y)
    {
        return new FInt((x.Value() * SHIFT) / y.Value());
    }

    public static FInt operator /(FInt x, long y)
    {
        return new FInt(x.Value() / y);
    }

    public static FInt operator /(FInt x, int y)
    {
        return new FInt(x.Value() / ((long)y));
    }
    #endregion

    public static FInt operator %(FInt x, FInt y)
    {
        return new FInt(x.Value() % y.Value());
    }

    #region Comparison Overloads
    public static bool operator ==(FInt x, FInt y)
    {
        return x.Value() == y.Value();
    }

    public static bool operator !=(FInt x, FInt y)
    {
        return x.Value() != y.Value();
    }

    public static bool operator >(FInt x, FInt y)
    {
        return x.Value() > y.Value();
    }

    public static bool operator >=(FInt x, FInt y)
    {
        return x.Value() >= y.Value();
    }

    public static bool operator <(FInt x, FInt y)
    {
        return x.Value() < y.Value();
    }

    public static bool operator <=(FInt x, FInt y)
    {
        return x.Value() <= y.Value();
    }
    #endregion

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is FInt))
        {
            return false;
        }
        return ((FInt)obj).Value() == value;
    }
    #endregion

    #region Math
    public static FInt Max(FInt a, FInt b)
    {
        long x = a.Value();
        long y = b.Value();
        return (x >= y) ? a : b;
    }

    public static FInt Min(FInt a, FInt b)
    {
        long x = a.Value();
        long y = b.Value();
        return (x >= y) ? b : a;
    }

    public static FInt Sqrt(FInt a)
    {
        if (a.Value() == 0L)
        {
            return a;
        }
        FInt x = 1L;
        for (int i = 0; i < 64; i++)
        {
            x = (x + a / x) / 2;
        }

        return x;
    }

    public static FInt Sin(FInt a)
    {
        FInt x = a % (2 * PI);
        bool overPi = x > PI;
        if (overPi)
        {
            x -= PI;
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

    public static FInt Cos(FInt a)
    {
        FInt x = a % (2 * PI);
        bool overPi = x > PI;
        if (overPi)
        {
            x -= PI;
        }
        x = new FInt(1)
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

    public static FInt Tan(FInt a)
    {
        FInt x = a % (2 * PI);
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
        if (dx.Value() == 0)
        {
            if (dy.Value() >= 0)
            {
                return new FInt(0.5) * PI;
            }
            else
            {
                return new FInt(1.5) * PI;
            }
        }
        if (dy.Value() == 0)
        {
            if (dx.Value() > 0)
            {
                return 0L;
            }
            else
            {
                return PI;
            }
        }

        if (dx.Value() < 0)
        {
            if (dy.Value() < 0)
            {
                return PI + Atan(-dx, -dy);
            }
            else
            {
                return PI - Atan(-dx, dy);
            }
        }
        else
        {
            if (dy.Value() < 0)
            {
                return 2 * PI - Atan(dx, -dy);
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

        long t = (dy.Value() << FLOATING_BITS) / dx.Value();
        long t2 = -(t * t) >> FLOATING_BITS;
        long t4 = (t2 * t2) >> FLOATING_BITS;
        long t6 = (t4 * t2) >> FLOATING_BITS;
        long t8 = (t4 * t4) >> FLOATING_BITS;
        long t10 = (t6 * t4) >> FLOATING_BITS;
        long t12 = (t6 * t6) >> FLOATING_BITS;
        long t14 = (t8 * t6) >> FLOATING_BITS;
        FInt x = new FInt(
            ((65536L * t) >> FLOATING_BITS) +
            ((((21845L * t) >> FLOATING_BITS) * t2) >> FLOATING_BITS) +
            ((((13107L * t) >> FLOATING_BITS) * t4) >> FLOATING_BITS) +
            ((((9102L * t) >> FLOATING_BITS) * t6) >> FLOATING_BITS) +
            ((((6317L * t) >> FLOATING_BITS) * t8) >> FLOATING_BITS) +
            ((((3664L * t) >> FLOATING_BITS) * t10) >> FLOATING_BITS) +
            ((((1432L * t) >> FLOATING_BITS) * t12) >> FLOATING_BITS) +
            ((((266L * t) >> FLOATING_BITS) * t14) >> FLOATING_BITS));

        if (swapAxes)
        {
            return PI / 2 - x;
        }
        return x;
    }

    // Return random FInt within a range. Includes both end points.
    public static FInt RandomRange(System.Random seed, FInt min, FInt max)
    {
        if (min > max)
        {
            Debug.LogError("Min must be less than max. Min: " + min.ToString() + ", Max: " + max.ToString());
            return 0L;
        }
        FInt r = new FInt((long)seed.Next());
        FInt diff = max - min;
        return (r * diff) / new FInt(2147483648L) + min;
    }
    #endregion
}
