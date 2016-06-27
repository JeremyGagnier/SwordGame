using System.Collections.Generic;

public class LeftRightInput : AxisInput
{
    public LeftRightInput(bool isLocalPlayer, int playerNum) :
        base(isLocalPlayer, playerNum, "x")
    {
    }
}
