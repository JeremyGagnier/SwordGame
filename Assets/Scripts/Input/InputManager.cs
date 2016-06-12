public class InputManager
{
    private int localPlayerInput;

    private AxisInput _xAxis;
    private AxisInput _yAxis;
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

    public InputManager(bool isLocalPlayer, int localPlayerInput)
    {
        this.localPlayerInput = localPlayerInput;

        _xAxis = new AxisInput(isLocalPlayer);
        _yAxis = new AxisInput(isLocalPlayer);
        _button1 = new ButtonInput(isLocalPlayer);
        _button2 = new ButtonInput(isLocalPlayer);
        _button3 = new ButtonInput(isLocalPlayer);
        _button4 = new ButtonInput(isLocalPlayer);
    }

    public void Advance()
    {
        _xAxis.Advance(string.Format("p{0}axisx", localPlayerInput));
        _yAxis.Advance(string.Format("p{0}axisy", localPlayerInput));
        _button1.Advance(string.Format("p{0}button1", localPlayerInput));
        _button2.Advance(string.Format("p{0}button2", localPlayerInput));
        _button3.Advance(string.Format("p{0}button3", localPlayerInput));
        _button4.Advance(string.Format("p{0}button4", localPlayerInput));
    }
}
