using System.Collections.Generic;

public class UpDownInput : AxisInput
{
    public UpDownInput(bool isLocalPlayer, int playerNum) :
        base(isLocalPlayer, playerNum, "y")
    {
    }
}
