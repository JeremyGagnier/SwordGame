using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


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
            new FInt(0.1),
            "Test Atan for 3*pi/4");
        RangeTest(
            () => { return FInt.Atan(new FInt(1), new FInt(1)); },
            FInt.PI() / 4,
            new FInt(0.1),
            "Test Atan for pi/4");
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
            Debug.Log("Test #" + numTests.ToString() + " has passed.");
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
            Debug.Log("Test #" + numTests.ToString() + " has passed.");
        }
    }
}
