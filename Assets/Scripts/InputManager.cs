using UnityEngine;
using System.Collections;

public class InputManager
{
    // Player 1 ------------------------------------------
    public static float p1axisx
    {
        get { return Input.GetAxis("p1axisx"); }
    }
    public static float p1axisy
    {
        get { return Input.GetAxis("p1axisy"); }
    }
    public static bool p1button1
    {
        get { return Input.GetAxis("p1button1") != 0.0f; }
    }
    public static bool p1button2
    {
        get { return Input.GetAxis("p1button2") != 0.0f; }
    }
    public static bool p1button3
    {
        get { return Input.GetAxis("p1button3") != 0.0f; }
    }
    public static bool p1button4
    {
        get { return Input.GetAxis("p1button4") != 0.0f; }
    }

    // Player 2 ------------------------------------------
    public static float p2axisx
    {
        get { return Input.GetAxis("p2axisx"); }
    }
    public static float p2axisy
    {
        get { return Input.GetAxis("p2axisy"); }
    }
    public static bool p2button1
    {
        get { return Input.GetAxis("p2button1") != 0.0f; }
    }
    public static bool p2button2
    {
        get { return Input.GetAxis("p2button2") != 0.0f; }
    }
    public static bool p2button3
    {
        get { return Input.GetAxis("p2button3") != 0.0f; }
    }
    public static bool p2button4
    {
        get { return Input.GetAxis("p2button4") != 0.0f; }
    }

    // Player 3 ------------------------------------------
    public static float p3axisx
    {
        get { return Input.GetAxis("p3axisx"); }
    }
    public static float p3axisy
    {
        get { return Input.GetAxis("p3axisy"); }
    }
    public static bool p3button1
    {
        get { return Input.GetAxis("p3button1") != 0.0f; }
    }
    public static bool p3button2
    {
        get { return Input.GetAxis("p3button2") != 0.0f; }
    }
    public static bool p3button3
    {
        get { return Input.GetAxis("p3button3") != 0.0f; }
    }
    public static bool p3button4
    {
        get { return Input.GetAxis("p3button4") != 0.0f; }
    }

    // Player 4 ------------------------------------------
    public static float p4axisx
    {
        get { return Input.GetAxis("p4axisx"); }
    }
    public static float p4axisy
    {
        get { return Input.GetAxis("p4axisy"); }
    }
    public static bool p4button1
    {
        get { return Input.GetAxis("p4button1") != 0.0f; }
    }
    public static bool p4button2
    {
        get { return Input.GetAxis("p4button2") != 0.0f; }
    }
    public static bool p4button3
    {
        get { return Input.GetAxis("p4button3") != 0.0f; }
    }
    public static bool p4button4
    {
        get { return Input.GetAxis("p4button4") != 0.0f; }
    }
}
