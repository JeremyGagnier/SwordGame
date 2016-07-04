using UnityEngine;
using System.Collections.Generic;

// TODO: Break this into one class per input
public class ButtonInput
{
    private bool isLocalPlayer = false;
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

    public ButtonInput(bool isLocalPlayer)
    {
        this.isLocalPlayer = isLocalPlayer;
    }

    // There is a special protocol to call input advances until the buffer is full
    // so that other advances can be skipped. Additionally during online play if
    // there isn't enough input then no advance will be called until there is.
    public bool Advance(string inputName=null)
    {
        bool newValue = false;
        if (isLocalPlayer)
        {
            newValue = Input.GetAxis(inputName) == 1;
            buffer.Enqueue(newValue);

            // Keep filling the buffer!
            if (Game.isOnline && buffer.Count <= NetworkingManager.bufferSize)
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
