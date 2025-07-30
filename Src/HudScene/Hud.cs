using Godot;

namespace FlappyCat.Src.HudScene;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    public override void _Ready()
    {
        GetNode<TextureRect>("ScoreRect").Hide();
        GetNode<Label>("CurrentScoreLabel").Hide();
    }

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreRect/ScoreLabel").Text = score.ToString();
        GetNode<Label>("CurrentScoreLabel").Text = score.ToString();
    }

    public void UpdateHighestScore(int score)
    {
        GetNode<Label>("ScoreRect/HighestScoreLabel").Text = $"BEST: {score.ToString()}";
    }

    public void ShowRestartMenu(int score)
    {
        GetNode<Label>("CurrentScoreLabel").Hide();

        GetNode<Label>("ScoreRect/ScoreLabel").Text = $"SCORE: {score.ToString()}";
        GetNode<Label>("TitleLabel").Text = "GAME OVER";

        GetNode<Label>("TitleLabel").Show();
        GetNode<TextureRect>("ScoreRect").Show();
        GetNode<TextureButton>("StartButton").Show();
    }

    public void OnStartButtonDown()
    {
        GetNode<TextureButton>("StartButton").Position += new Vector2(0, 1);
    }

    public void OnStartButtonUp()
    {
        GetNode<TextureButton>("StartButton").Position += new Vector2(0, -1);
    }

    public void OnStartButtonPressed()
    {
        GetNode<TextureButton>("StartButton").Hide();
        GetNode<Label>("TitleLabel").Hide();
        GetNode<TextureRect>("ScoreRect").Hide();

        GetNode<Label>("CurrentScoreLabel").Show();

        EmitSignal(SignalName.StartGame);
    }
}