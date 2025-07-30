using FlappyCat.Src.Common;
using Godot;

namespace FlappyCat.Src.ScratcherScene;

public partial class Scratcher : Scrollable
{
    [Signal]
    public delegate void OnHitEventHandler();

    [Signal]
    public delegate void OnJumpSuccessfulEventHandler();

    private bool _hasBeenJumped = false;
    private const int ScoreValue = 1;
    private const string ScoreAudioName = "ScoreSound";
    private const int ViewportMargin = 100;
    private const int JumpMargin = 75;

    public override void _ScrollableReady()
    {
        float positionOffset = GD.RandRange(ViewportMargin * -1, ViewportMargin);
        Position = new Vector2(StartPosition.X, StartPosition.Y + positionOffset);
    }

    public override void _ScrollableProcess(double delta)
    {
        if (Position.X < (ScreenSize.X / 2 - JumpMargin) && !_hasBeenJumped)
        {
            _hasBeenJumped = true;
            EmitSignal(SignalName.OnJumpSuccessful, ScoreValue);
            GetNode<AudioStreamPlayer2D>(ScoreAudioName).Play();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        EmitSignal(SignalName.OnHit);
        QueueFree();
    }
}
