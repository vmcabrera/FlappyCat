using FlappyCat.Src.Common;
using Godot;

namespace FlappyCat.Src.BonusScoreItemScene;

public partial class BonusScoreItem : Scrollable
{
    [Signal]
    public delegate void OnHitEventHandler();

    private const int ScoreValue = 2;
    private const string ScoreAudioName = "ScoreSound";
    private const int ViewportMargin = 45;

    public override void _ScrollableReady()
    {
        int bottomMargin = (int)(GetViewportRect().Size.Y - ViewportMargin);

        float positionOffset = GD.RandRange(ViewportMargin, bottomMargin);
        Position = new Vector2(StartPosition.X, StartPosition.Y + positionOffset);
    }

    private void OnBodyEntered(Node2D body)
    {
        EmitSignal(SignalName.OnHit, ScoreValue);
        GetNode<AudioStreamPlayer2D>(ScoreAudioName).Play();

        GetNode<Sprite2D>("Sprite2D").Hide();
        GetNode<CollisionShape2D>("CollisionShape2D").Hide();
    }

    private void OnScoreSoundFinished()
    {
        QueueFree();
    }
}