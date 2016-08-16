using UnityEngine;
using System.Collections.Generic;

// TODO: Break this into one class per input
public class ButtonInput
{
    private bool isLocalPlayer = false;
    private int playerNum;
    private string buttonName;
    private List<int> frameCounts = new List<int>();
    private int unchangedFrames = 0;

    private Queue<bool> buffer = new Queue<bool>();

    public bool justChanged
    {
        get { return (unchangedFrames == 0); }
    }
    public bool pressed
    {
        get { return (frameCounts.Count % 2 == 1); }
    }

    public ButtonInput(bool isLocalPlayer, int playerNum, string buttonName)
    {
        this.isLocalPlayer = isLocalPlayer;
        this.playerNum = playerNum;
        this.buttonName = buttonName;
    }

    // There is a special protocol to call input advances until the buffer is full
    // so that other advances can be skipped. Additionally during online play if
    // there isn't enough input then no advance will be called until there is.
    public bool Advance()
    {
        bool newValue = false;
        if (isLocalPlayer)
        {
            if (Game.isOnline)
            {
                newValue = Input.GetAxis(string.Format("p1button{0}", buttonName)) == 1;
            }
            else
            {
                newValue = Input.GetAxis(string.Format("p{0}button{1}", playerNum + 1, buttonName)) == 1;
            }
            buffer.Enqueue(newValue);

            // Keep filling the buffer!
            if (Game.isOnline && buffer.Count <= OnlineNetwork.bufferSize)
            {
                return newValue;
            }
        }
        else if (buffer.Count == 0)
        {
            Debug.LogError("Advance was called and there was no input!");
        }

        bool value = buffer.Dequeue();
        unchangedFrames += 1;

        if ((frameCounts.Count % 2 == 0 && value) ||
            (frameCounts.Count % 2 == 1 && !value))
        {
            frameCounts.Add(unchangedFrames);
            unchangedFrames = 0;
        }
        return newValue;
    }

    public void AddToBuffer(bool x)
    {
        buffer.Enqueue(x);
    }
}
