using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class Tests : MonoBehaviour
{
    public bool DEBUG = false;
    private int numTests;
    void Update()
    {
        if (EditorApplication.isPlaying) return;

        numTests = 0;
        Test<FInt>(
            () => { return FInt.Atan(new FInt(1), new FInt(0)); },
            FInt.Zero(),
            "Test Atan for zero");
        Test<FInt>(
            () => { return FInt.Atan(new FInt(-1), new FInt(0)); },
            FInt.PI(),
            "Test Atan for pi");
        RangeTest(
            () => { return FInt.Atan(new FInt(-1), new FInt(1)); },
            3 * FInt.PI() / 4,
            new FInt(0.01),
            "Test Atan for 3*pi/4");
        RangeTest(
            () => { return FInt.Atan(new FInt(1), new FInt(1)); },
            FInt.PI() / 4,
            new FInt(0.01),
            "Test Atan for pi/4");
        RangeTest(
            () => { return FInt.Atan(new FInt(1), new FInt(1000)); },
            FInt.PI() / 2,
            new FInt(0.01),
            "Test Atan for values close to pi/2");

        Test(
            () => { return FInt.Sin(FInt.Zero()); },
            FInt.Zero(),
            "Test Sin for zero");
        RangeTest(
            () => { return FInt.Sin(FInt.PI() / 2); },
            FInt.One(),
            new FInt(0.01),
            "Test Sin for one");
        RangeTest(
            () => { return FInt.Sin(FInt.PI() / 4); },
            FInt.Sqrt(new FInt(2)) / 2,
            new FInt(0.01),
            "Test Sin for sqrt(2) / 2");
        RangeTest(
            () => { return FInt.Sin(-FInt.PI() / 4); },
            -FInt.Sqrt(new FInt(2)) / 2,
            new FInt(0.01),
            "Test Sin for -sqrt(2) / 2. This one is through -pi / 4");
        RangeTest(
            () => { return FInt.Sin(7 * FInt.PI() / 4); },
            -FInt.Sqrt(new FInt(2)) / 2,
            new FInt(0.01),
            "Test Sin for -sqrt(2) / 2. This one is through 7 * pi / 4");

        Test(
            () => { return FInt.Cos(FInt.Zero()); },
            FInt.One(),
            "Test Cos for one");
        RangeTest(
            () => { return FInt.Cos(FInt.PI() / 2); },
            FInt.Zero(),
            new FInt(0.01),
            "Test Cos for zero");
        RangeTest(
            () => { return FInt.Cos(FInt.PI()); },
            -FInt.One(),
            new FInt(0.01),
            "Test Cos for -1");
        RangeTest(
            () => { return FInt.Cos(-FInt.PI() / 4); },
            FInt.Sqrt(new FInt(2)) / 2,
            new FInt(0.01),
            "Test Cos for -sqrt(2) / 2. This one is through -pi / 4");
        RangeTest(
            () => { return FInt.Cos(7 * FInt.PI() / 4); },
            FInt.Sqrt(new FInt(2)) / 2,
            new FInt(0.01),
            "Test Cos for -sqrt(2) / 2. This one is through 7 * pi / 4");

    }

    private void Test<T>(
        Func<T> test,
        T expected,
        string description = "")
    {
        numTests += 1;
        T result = test();
        if (!result.Equals(expected))
        {
            Debug.LogError(
                "Test #" +
                numTests.ToString() +
                " has failed. Gave: " +
                result.ToString() +
                ", expected: " +
                expected.ToString() +
                ". " +
                description);
        }
        else if (DEBUG)
        {
            Debug.Log("Test #" + numTests.ToString() + " has passed. " + description);
        }
    }

    private void RangeTest(
        Func<FInt> test,
        FInt expected,
        FInt tolerance,
        string description = "")
    {
        numTests += 1;
        FInt result = test();
        if (result > expected + tolerance ||
            result < expected - tolerance)
        {
            Debug.LogError(
                "Test #" +
                numTests.ToString() +
                " has failed. Gave: " +
                result.ToString() +
                ", expected: " +
                expected.ToString() +
                ". " +
                description);
        }
        else if (DEBUG)
        {
            Debug.Log("Test #" + numTests.ToString() + " has passed. " + description);
        }
    }
}
