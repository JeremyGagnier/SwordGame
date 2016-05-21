using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


[ExecuteInEditMode]
public class Tests : MonoBehaviour
{
    private static bool DEBUG = false;
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
            new FInt(3.14),
            "Test Atan for pi");
        Test<FInt>(
            () => { return FInt.Atan(new FInt(-1), new FInt(1)); },
            new FInt(2.33193),
            "Test Atan for 3*pi/4");
        Test<FInt>(
            () => { return FInt.Atan(new FInt(1), new FInt(1)); },
            new FInt(0.80808),
            "Test Atan for pi/4");
    }

    private void Test<T>(Func<T> test, T expected, string description = "")
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
}
