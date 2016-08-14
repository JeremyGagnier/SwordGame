using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct InputSegment {
    public long xAxis, yAxis;
    public bool button1, button2, button3, button4;
}

public class InputTracker {
    private List<List<InputSegment>> frames = new List<List<InputSegment>>();
    private List<Dictionary<int, InputSegment>> futureFrames = new List<Dictionary<int, InputSegment>>();

    public void Setup(int numPlayers)
    {
        for (int i = 0; i < numPlayers; ++i)
        {
            frames.Add(new List<InputSegment>());
            futureFrames.Add(new Dictionary<int, InputSegment>());
        }
    }

    public void AddInput(byte[] message)
    {
        int frame = message[0] * 65536 + message[1] * 256 + message[2];
        int pnum = (int)message[3] % 4;
        
        InputSegment segment;
        segment.button1 = (message[3] & (byte)4) != 0;
        segment.button2 = (message[3] & (byte)8) != 0;
        segment.button3 = (message[3] & (byte)16) != 0;
        segment.button4 = (message[3] & (byte)32) != 0;
        segment.xAxis = (long)message[4] * 72057594037927936 +
            (long)message[5] * 281474976710656 +
            (long)message[6] * 1099511627776 +
            (long)message[7] * 4294967296 +
            (long)message[8] * 16777216 +
            (long)message[9] * 65536 +
            (long)message[10] * 256 +
            (long)message[11];
        segment.yAxis = (long)message[12] * 72057594037927936 +
            (long)message[13] * 281474976710656 +
            (long)message[14] * 1099511627776 +
            (long)message[15] * 4294967296 +
            (long)message[16] * 16777216 +
            (long)message[17] * 65536 +
            (long)message[18] * 256 +
            (long)message[19];

        if (frames[pnum].Count == frame)
        {
            frames[pnum].Add(segment);
            int futureFrame = frame + 1;
            while (futureFrames[pnum].ContainsKey(futureFrame))
            {
                frames[pnum].Add(futureFrames[pnum][futureFrame]);
                futureFrames[pnum].Remove(futureFrame);
                futureFrame += 1;
            }
        }
        else if (frames[pnum].Count < frame)
        {
            futureFrames[pnum].Add(frame, segment);
        }
    }

    /// <summary>
    /// This should only be used by the local player.
    /// It assumes this input is for the latest frame.
    /// </summary>
    /// <param name="pnum">Player index</param>
    /// <param name="segment">Input data</param>
    /// <returns></returns>
    public int SaveInput(int pnum, InputSegment segment)
    {
        int frame = frames[pnum].Count;
        frames[pnum].Add(segment);
        return frame;
    }

    public InputSegment GetInput(int pnum, int frame)
    {
        return frames[pnum][frame];
    }

    public byte[] GetData(int pnum, int frame)
    {
        InputSegment segment = frames[pnum][frame];
        byte[] message = new byte[20];
        message[0] = (byte)(frame / 65536);
        message[1] = (byte)(frame / 255);
        message[2] = (byte)frame;
        message[3] = (byte)(pnum +
            (segment.button1 ? 1 : 0) * 4 +
            (segment.button2 ? 1 : 0) * 8 +
            (segment.button3 ? 1 : 0) * 16 +
            (segment.button4 ? 1 : 0) * 32);
        message[4] = (byte)(segment.xAxis / 72057594037927936);
        message[5] = (byte)(segment.xAxis / 281474976710656);
        message[6] = (byte)(segment.xAxis / 1099511627776);
        message[7] = (byte)(segment.xAxis / 4294967296);
        message[8] = (byte)(segment.xAxis / 16777216);
        message[8] = (byte)(segment.xAxis / 65536);
        message[10] = (byte)(segment.xAxis / 256);
        message[11] = (byte)segment.xAxis;
        message[12] = (byte)(segment.yAxis / 72057594037927936);
        message[13] = (byte)(segment.yAxis / 281474976710656);
        message[14] = (byte)(segment.yAxis / 1099511627776);
        message[15] = (byte)(segment.yAxis / 4294967296);
        message[16] = (byte)(segment.yAxis / 16777216);
        message[17] = (byte)(segment.yAxis / 65536);
        message[18] = (byte)(segment.yAxis / 256);
        message[19] = (byte)segment.yAxis;

        return message;
    }

    /// <summary>
    /// Checks if this frame exists for one player
    /// </summary>
    /// <param name="pnum">Player index</param>
    /// <param name="frame">Frame index</param>
    /// <returns></returns>
    public bool HasFrame(int pnum, int frame)
    {
        return frames[pnum].Count > frame;
    }
}
