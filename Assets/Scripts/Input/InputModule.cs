using System;

public class InputModule
{
    private OnlineNetwork network;
    private bool isLocalPlayer;
    private int playerNum;

    private LeftRightInput _xAxis;
    private UpDownInput _yAxis;
    private StabInput _stab;
    private SwingLeftInput _swingLeft;
    private SwingRightInput _swingRight;
    private DodgeInput _dodge;

    public FInt xAxis { get { return _xAxis.value; } }
    public FInt yAxis { get { return _yAxis.value; } }
    public bool stab { get { return _stab.pressed; } }
    public bool swingLeft { get { return _swingLeft.pressed; } }
    public bool swingRight { get { return _swingRight.pressed; } }
    public bool dodge { get { return _dodge.pressed; } }
    public bool stabChanged { get { return _stab.justChanged; } }
    public bool swingLeftChanged { get { return _swingLeft.justChanged; } }
    public bool swingRightChanged { get { return _swingRight.justChanged; } }
    public bool dodgeChanged { get { return _dodge.justChanged; } }

    public InputModule(OnlineNetwork network, bool isLocalPlayer, int playerNum)
    {
        this.network = network;
        this.isLocalPlayer = isLocalPlayer;
        this.playerNum = playerNum;

        _xAxis = new LeftRightInput(isLocalPlayer, playerNum);
        _yAxis = new UpDownInput(isLocalPlayer, playerNum);
        _stab = new StabInput(isLocalPlayer, playerNum);
        _swingLeft = new SwingLeftInput(isLocalPlayer, playerNum);
        _swingRight = new SwingRightInput(isLocalPlayer, playerNum);
        _dodge = new DodgeInput(isLocalPlayer, playerNum);
    }

    public void Advance(int frame)
    {
        if (Game.isOnline && !isLocalPlayer)
        {
            InputSegment segment = network.GetInput(playerNum, frame);
            _xAxis.AddToBuffer(segment.xAxis);
            _yAxis.AddToBuffer(segment.yAxis);
            _stab.AddToBuffer(segment.button1);
            _swingLeft.AddToBuffer(segment.button2);
            _swingRight.AddToBuffer(segment.button3);
            _dodge.AddToBuffer(segment.button4);
        }

        long xAxis = _xAxis.Advance();
        long yAxis = _yAxis.Advance();
        bool b1 = _stab.Advance();
        bool b2 = _swingLeft.Advance();
        bool b3 = _swingRight.Advance();
        bool b4 = _dodge.Advance();

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
