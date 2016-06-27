using UnityEngine;
using System.Collections.Generic;

public class Player : Character
{
    public static FInt PRADIUS = new FInt(100.0f);
    
    private InputModule input;
    private FInt dmgLeftovers = FInt.Zero();
    private List<FVector> lastFacing;
    private FInt facing {
        get
        {
            FInt dx = FInt.Zero();
            FInt dy = FInt.Zero();
            foreach (FVector vec in lastFacing)
            {
                dx += vec.x;
                dy += vec.y;
            }
            dx /= lastFacing.Count;
            dy /= lastFacing.Count;
            return FInt.Atan(dx, dy);
        }
    }

    public Sword sword;
    public string playerName;

    [SerializeField] private GameObject characterImg;
    [SerializeField] private GameObject characterMask;

    void Awake()
    {
        radius = PRADIUS;
    }

    void Start()
    {
        lastFacing = new List<FVector>();
        for (int i = 0; i < 3; ++i)
        {
            lastFacing.Add(new FVector(FInt.Zero(), FInt.Zero()));
        }
    }

    public void Setup(InputModule input, FInt startx, FInt starty, int team, string name)
    {
        this.input = input;
        position.x = startx;
        position.y = starty;
        this.team = team;
        playerName = name;

        transform.position = new Vector3(startx.ToFloat(), starty.ToFloat());

        sword.Setup(this);
    }

    public override void Advance()
    {
        FVector dpos = new FVector(input.xAxis, -input.yAxis);
        dpos = dpos.Normalize();
        if (input.button1 && input.button1Changed)
        {
            // Swing counter-clockwise
            sword.Swing(Sword.SwingState.CCWISE, facing);
        }
        else if (input.button2 && input.button2Changed)
        {
            // Stab
            sword.Swing(Sword.SwingState.STAB, facing);
        }
        else if (input.button3 && input.button3Changed)
        {
            // Swing clockwise
            sword.Swing(Sword.SwingState.CWISE, facing);
        }

        if (dpos.x.rawValue != 0 || dpos.y.rawValue != 0)
        {
            position.x += dpos.x * CalculateSpeed() * Game.TIMESTEP;
            position.y += dpos.y * CalculateSpeed() * Game.TIMESTEP;
            lastFacing.RemoveAt(0);
            lastFacing.Add(dpos);
        }

        FInt fdx = FInt.Zero();
        FInt fdy = FInt.Zero();
        foreach (FVector vec in lastFacing)
        {
            fdx += vec.x;
            fdy += vec.y;
        }

        // TODO: Clean up this hack with animations
        if (fdx.rawValue < 0)
        {
            characterImg.transform.localPosition = new Vector3(30, 118, 0);
            characterImg.transform.localScale = new Vector3(1, 1, 1);
            characterMask.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            characterImg.transform.localPosition = new Vector3(-30, 118, 0);
            characterImg.transform.localScale = new Vector3(-1, 1, 1);
            characterMask.transform.localScale = new Vector3(-1, 1, 1);
        }

        sword.Advance();
        base.Advance();
    }

    public override void Damage(FInt damage)
    {
        // Remove sword parts
        dmgLeftovers += damage;
        while (dmgLeftovers >= FInt.One())
        {
            dmgLeftovers -= FInt.One();
            sword.RemovePart();
        }
        invincibility += new FInt(0.4f);
    }

    public FInt CalculateSpeed()
    {
        return 10 * FInt.Max(new FInt(100.0f) - sword.weight, new FInt(10.0f));
    }
}
