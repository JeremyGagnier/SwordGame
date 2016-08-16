using System.Collections.Generic;

public class StabInput : ButtonInput
{
    public StabInput(bool isLocalPlayer, int playerNum) :
        base(isLocalPlayer, playerNum, "1")
    {
    }
}
