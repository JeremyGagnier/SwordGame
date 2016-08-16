using System.Collections.Generic;

public class DodgeInput : ButtonInput
{
    public DodgeInput(bool isLocalPlayer, int playerNum) :
        base(isLocalPlayer, playerNum, "4")
    {
    }
}
