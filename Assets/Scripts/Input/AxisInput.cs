﻿using UnityEngine;
using System.Collections.Generic;

public class AxisInput
{
    private bool isLocalPlayer = false;
    private int playerNum;
    private string axisName;
    private Stack<long> values = new Stack<long>();

    private Queue<long> buffer = new Queue<long>();
    
    public FInt value
    {
        get { return new FInt(values.Peek()); }
    }

    public AxisInput(bool isLocalPlayer, int playerNum, string axisName)
    {
        this.isLocalPlayer = isLocalPlayer;
        this.playerNum = playerNum;
        this.axisName = axisName;
    }

    // There is a special protocol to call input advances until the buffer is full
    // so that other advances can be skipped. Additionally during online play if
    // there isn't enough input then no advance will be called until there is.
    public long Advance()
    {
        long newValue = 0;
        if (isLocalPlayer)
        {
            // It's fine if everyones processor does this differently because
            // this will be sent to other players.
            float inputValue;
            if (Game.isOnline)
            {
                inputValue = Input.GetAxis(string.Format("p1axis{0}", axisName));
            }
            else
            {
                inputValue = Input.GetAxis(string.Format("p{0}axis{1}", playerNum + 1, axisName));
            }
            newValue = (long)(inputValue * (1 << FInt.FLOATING_BITS));
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
            return newValue;
        }

        values.Push(buffer.Dequeue());
        return newValue;
    }

    public void AddToBuffer(long x)
    {
        buffer.Enqueue(x);
    }
}
