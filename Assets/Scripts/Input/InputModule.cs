using System;

public class InputModule
{
    private OnlineNetwork network;
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

    public InputModule(OnlineNetwork network, bool isLocalPlayer, int localPlayerInput)
    {
        this.network = network;
        this.isLocalPlayer = isLocalPlayer;
        this.localPlayerInput = localPlayerInput;

        _xAxis = new LeftRightInput(isLocalPlayer, localPlayerInput);
        _yAxis = new UpDownInput(isLocalPlayer, localPlayerInput);
        _button1 = new ButtonInput(isLocalPlayer);
        _button2 = new ButtonInput(isLocalPlayer);
        _button3 = new ButtonInput(isLocalPlayer);
        _button4 = new ButtonInput(isLocalPlayer);
    }

    public void Advance(int frame)
    {
        if (Game.isOnline && !isLocalPlayer)
        {
            InputSegment segment = network.GetInput(localPlayerInput, frame);
            _xAxis.AddToBuffer(segment.xAxis);
            _yAxis.AddToBuffer(segment.yAxis);
            _button1.AddToBuffer(segment.button1);
            _button2.AddToBuffer(segment.button2);
            _button3.AddToBuffer(segment.button3);
            _button4.AddToBuffer(segment.button4);
        }

        long xAxis = _xAxis.Advance();
        long yAxis = _yAxis.Advance();
        bool b1 = _button1.Advance(string.Format("p{0}button1", localPlayerInput + 1));
        bool b2 = _button2.Advance(string.Format("p{0}button2", localPlayerInput + 1));
        bool b3 = _button3.Advance(string.Format("p{0}button3", localPlayerInput + 1));
        bool b4 = _button4.Advance(string.Format("p{0}button4", localPlayerInput + 1));

        if (Game.isOnline && isLocalPlayer)
        {
            InputSegment segment;
            segment.xAxis = xAxis;
            segment.yAxis = yAxis;
            segment.button1 = b1;
            segment.button2 = b2;
            segment.button3 = b3;
            segment.button4 = b4;
            network.SendGameMessage(segment);
        }
    }
}
