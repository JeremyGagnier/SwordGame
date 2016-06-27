using System;

public class InputModule
{
    private bool isLocalPlayer;
    private int localPlayerInput;

    private LeftRightInput _xAxis;
    private UpDownInput _yAxis;
    private ButtonInput _button1;
    private ButtonInput _button2;
    private ButtonInput _button3;
    private ButtonInput _button4;

    public FInt xAxis { get { return _xAxis.value; } }
    public FInt yAxis { get { return _yAxis.value; } }
    public bool button1 { get { return _button1.pressed; } }
    public bool button2 { get { return _button2.pressed; } }
    public bool button3 { get { return _button3.pressed; } }
    public bool button4 { get { return _button4.pressed; } }
    public bool button1Changed { get { return _button1.justChanged; } }
    public bool button2Changed { get { return _button2.justChanged; } }
    public bool button3Changed { get { return _button3.justChanged; } }
    public bool button4Changed { get { return _button4.justChanged; } }

    public InputModule(bool isLocalPlayer, int localPlayerInput)
    {
        this.isLocalPlayer = isLocalPlayer;
        this.localPlayerInput = localPlayerInput;

        _xAxis = new LeftRightInput(isLocalPlayer, localPlayerInput);
        _yAxis = new UpDownInput(isLocalPlayer, localPlayerInput);
        _button1 = new ButtonInput(isLocalPlayer);
        _button2 = new ButtonInput(isLocalPlayer);
        _button3 = new ButtonInput(isLocalPlayer);
        _button4 = new ButtonInput(isLocalPlayer);
    }

    public void Advance()
    {
        long xAxis = _xAxis.Advance();
        long yAxis = _yAxis.Advance();
        bool b1 = _button1.Advance(string.Format("p{0}button1", localPlayerInput));
        bool b2 = _button2.Advance(string.Format("p{0}button2", localPlayerInput));
        bool b3 = _button3.Advance(string.Format("p{0}button3", localPlayerInput));
        bool b4 = _button4.Advance(string.Format("p{0}button4", localPlayerInput));

        // TODO: Also test to make sure that the game is online
        if (isLocalPlayer)
        {
            string output = string.Format("{0} {1} {2} {3} {4} {5}",
                xAxis,
                yAxis,
                b1 ? "1" : "0",
                b2 ? "1" : "0",
                b3 ? "1" : "0",
                b4 ? "1" : "0");

            NetworkingManager.SendGameMessage(output);
        }
    }

    public void Input(string inputs)
    {
        string[] data = inputs.Split(' ');
        _xAxis.AddToBuffer(Convert.ToInt32(data[0]));
        _yAxis.AddToBuffer(Convert.ToInt32(data[1]));
        _button1.AddToBuffer(data[2] == "1");
        _button2.AddToBuffer(data[3] == "1");
        _button3.AddToBuffer(data[4] == "1");
        _button4.AddToBuffer(data[5] == "1");
    }
}
