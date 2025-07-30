using Godot;

namespace FlappyCat.Src.PlayerScene;

public enum PlayerState
{
    Idle,
    Flying,
    Dead,
}

public partial class Player : CharacterBody2D
{
    [Signal]
    public delegate void TouchGroundEventHandler();

    public float FlapSpeed { get; } = 950f;
    public float MaxSpeed { get; } = 800f;
    public float Gravity { get; } = 25f;

    public PlayerState CurrentPlayerState { get; set; }
    public Vector2 StartPosition { get; set; }

    private AnimatedSprite2D _playerAnimatedSprite;
    private const string IdleAnimationName = "idle";
    private const string JumpingAnimationName = "jumping";
    private const float IdleVerticalVelocity = 2f;
    private const float IdleVerticalPosition = 5;

    public override void _Ready()
    {
        Init();
        _playerAnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {
        if (CurrentPlayerState == PlayerState.Idle)
        {
            IdleMovement();
        }
        else if (CurrentPlayerState == PlayerState.Dead)
        {
            Velocity = Vector2.Zero;
        }
        else
        {
            FlyingMovement();
        }

        CapsuleShape2D shape = (CapsuleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;

        if (Position.Y < shape.Height * -1) Position = new Vector2(Position.X, shape.Height * -1);
        if (Position.Y > GetViewportRect().Size.Y) EmitSignal(SignalName.TouchGround);
    }

    public void Init()
    {
        Position = StartPosition;
        Velocity = Vector2.Zero;
        CurrentPlayerState = PlayerState.Idle;
    }

    public void StartState(PlayerState state)
    {
        Show();
        CurrentPlayerState = state;
    }

    private void IdleMovement()
    {
        float newVerticalVelocity = Position.Y > StartPosition.Y + IdleVerticalPosition ? Velocity.Y - IdleVerticalVelocity : Velocity.Y + IdleVerticalVelocity;
        Velocity = new Vector2(0, newVerticalVelocity);

        _playerAnimatedSprite.Animation = IdleAnimationName;

        MoveAndSlide();
    }

    private void FlyingMovement()
    {
        float currentVerticalVelocity = Input.IsActionJustPressed("jump") ? -FlapSpeed : Velocity.Y;
        float newVerticalVelocity = (currentVerticalVelocity + Gravity) < MaxSpeed ? currentVerticalVelocity + Gravity : MaxSpeed;
        Velocity = new Vector2(0, newVerticalVelocity);

        _playerAnimatedSprite.Animation = newVerticalVelocity < 0 ? JumpingAnimationName : IdleAnimationName;

        bool hasCollided = MoveAndSlide();
        if (hasCollided) CurrentPlayerState = PlayerState.Dead;
    }
}
