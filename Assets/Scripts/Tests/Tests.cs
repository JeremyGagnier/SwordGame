using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class Tests : EditorWindow
{
    public bool debug = false;
    private int numTests;

    [MenuItem("SwordGame/Tests")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Tests));
    }

    void OnGUI()
    {
        debug = GUILayout.Toggle(debug, "Print Successes");
        if (GUILayout.Button("Run Tests"))
        {
            RunTests();
        }
    }

    void RunTests()
    {
        numTests = 0;
        Test(
            () => { return FInt.Atan(new FInt(1), new FInt(0)); },
            FInt.Zero(),
            "Test Atan for zero");
        Test(
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
        RangeTest(
            () => { return FInt.Atan(new FInt(2), new FInt(1)); },
            new FInt(0.46365),
            new FInt(0.01),
            "Test Atan for 0.46365");
        RangeTest(
            () => { return FInt.Atan(new FInt(1), new FInt(2)); },
            new FInt(1.10715),
            new FInt(0.01),
            "Test Atan for 1.10715");

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

        RangeTest(
            () => { return FInt.Sqrt(new FInt(2)); },
            new FInt(1.4142),
            new FInt(0.01),
            "Test Square root for sqrt(2)");
        RangeTest(
            () => { return FInt.Sqrt(new FInt(16)); },
            new FInt(4),
            new FInt(0.01),
            "Test Square root for 4");

        System.Random r1 = new System.Random();
        //System.Random r2 = new System.Random(0);
        RangeTest(
            () => { return FInt.RandomRange(r1, FInt.Zero(), FInt.One()); },
            new FInt(0.5),
            new FInt(0.5),
            "Test Random Range to make sure the range is correct");
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
        else if (debug)
        {
            Debug.Log(
                "Test #" +
                numTests.ToString() +
                " has passed. Result: " +
                result.ToString() +
                " " +
                description);
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
        else if (debug)
        {
            Debug.Log(
                "Test #" +
                numTests.ToString() +
                " has passed. Result: " +
                result.ToString() +
                " " +
                description);
        }
    }
}
