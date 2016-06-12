using UnityEngine;
using System.Collections.Generic;

public class ButtonInput
{
    private bool isLocalPlayer = false;
    private List<int> frameCounts = new List<int>();
    private int unchangedFrames = 0;

    // TODO: Append to the buffer from the NetworkingManager
    private Queue<bool> buffer = new Queue<bool>();
    public int bufferSize = 0;

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

    public void Advance(string inputName=null)
    {
        if (isLocalPlayer)
        {
            buffer.Enqueue(Input.GetAxis(inputName) == 1);

            // Keep filling the buffer!
            if (buffer.Count <= bufferSize) return;
        }
        else
        {
            while (buffer.Count <= bufferSize)
            {
                NetworkingManager.WaitForInput();
            }
        }

        bool value = buffer.Dequeue();
        unchangedFrames += 1;

        if ((frameCounts.Count % 2 == 0 && value) ||
            (frameCounts.Count % 2 == 1 && !value))
        {
            frameCounts.Add(unchangedFrames);
            unchangedFrames = 0;
        }
    }
}
