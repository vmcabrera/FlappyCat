using Godot;

namespace FlappyCat.Src.Common;

public abstract partial class Scrollable : Area2D
{
    [Export]
    public int Speed { get; set; } = 250;

    public Vector2 StartPosition { get; set; }
    public Vector2 ScreenSize { get; set; }
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        StartPosition = GetNode<Marker2D>("StartPosition").Position;

        _ScrollableReady();
    }

    public virtual void _ScrollableReady() { }

    public override void _Process(double delta)
    {
        _velocity = new Vector2(-1, 0);
        _velocity = _velocity.Normalized() * Speed;

        Position += _velocity * (float)delta;

        _ScrollableProcess(delta);
    }

    public virtual void _ScrollableProcess(double delta) { }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}
