using UnityEngine;
using System.Collections.Generic;

public class AxisInput
{
    private bool isLocalPlayer = false;
    private Stack<long> values = new Stack<long>();

    // TODO: Append to the buffer from the NetworkingManager
    private Queue<long> buffer = new Queue<long>();
    public int bufferSize = 0;
    
    public FInt value
    {
        get { return FInt.RawFInt(values.Peek()); }
    }

    public AxisInput(bool isLocalPlayer)
    {
        this.isLocalPlayer = isLocalPlayer;
    }

    public void Advance(string inputName=null)
    {
        if (isLocalPlayer)
        {
            // It's fine if everyones processor does this differently because
            // this will be sent to other players.
            buffer.Enqueue((long)(Input.GetAxis(inputName) * (1 << FInt.FLOATING_BITS)));

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

        values.Push(buffer.Dequeue());
    }
}
