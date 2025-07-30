using FlappyCat.Src.BonusScoreItemScene;
using FlappyCat.Src.HudScene;
using FlappyCat.Src.PlayerScene;
using FlappyCat.Src.ScratcherScene;
using Godot;

namespace FlappyCat.Src;

public enum GameState
{
    Stopped,
    Running,
}

public partial class Main : Node
{
    [Export]
    public PackedScene ScratcherScene { get; set; }

    [Export]
    public PackedScene BonusScoreItemScene { get; set; }

    [Export]
    public float BonusScoreItemChance { get; set; } = 0.15f;

    public Player Player { get; set; }
    public Hud Hud { get; set; }

    private int _score;
    private int _highestScore;
    private GameState GameState = GameState.Stopped;

    private const float BonusScoreTimerDelay = 0.6f;

    public override void _Ready()
    {
        SetProcess(false);

        Hud = GetNode<Hud>("Hud");
        Player = GetNode<Player>("Player");

        Player.StartPosition = GetNode<Marker2D>("StartPosition").Position;
        Player.Init();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("jump") && GameState == GameState.Stopped)
        {
            GameState = GameState.Running;
            GetNode<Timer>("StartGameTimer").Start();
            Player.StartState(PlayerState.Flying);
        }
    }

    public void StartGame()
    {
        SetProcess(true);
        GetNode<AudioStreamPlayer2D>("Music").Play();

        _score = 0;

        Hud.UpdateScore(_score);
        Player.StartState(PlayerState.Idle);
    }

    public void OnStartGameTimerTimeout()
    {
        GetNode<Timer>("ScratcherTimer").Start();
        GetNode<Timer>("BonusScoreTimer").Start();
        GetNode<Timer>("BonusScoreTimer").WaitTime -= BonusScoreTimerDelay;
    }

    public void OnScratcherTimerTimeout()
    {
        Scratcher scratcher = ScratcherScene.Instantiate<Scratcher>();
        scratcher.Connect("OnHit", new Callable(this, MethodName.GameOver));
        scratcher.Connect("OnJumpSuccessful", new Callable(this, MethodName.ScorePoints));

        AddChild(scratcher);
    }

    public void OnBonusScoreTimerTimeout()
    {
        if (GD.Randf() <= BonusScoreItemChance)
        {
            BonusScoreItem bonusScoreItem = BonusScoreItemScene.Instantiate<BonusScoreItem>();
            bonusScoreItem.Connect("OnHit", new Callable(this, MethodName.ScorePoints));

            AddChild(bonusScoreItem);
        }
    }

    private void ScorePoints(int amount)
    {
        _score += amount;
        Hud.UpdateScore(_score);
    }

    public void GameOver()
    {
        if (_score > _highestScore) _highestScore = _score;

        SetProcess(false);

        GetTree().CallGroup("scratchers", Node.MethodName.QueueFree);
        GetTree().CallGroup("bonusScoreItems", Node.MethodName.QueueFree);

        GetNode<Timer>("StartGameTimer").Stop();
        GetNode<Timer>("ScratcherTimer").Stop();

        GetNode<Timer>("BonusScoreTimer").Stop();
        GetNode<Timer>("BonusScoreTimer").WaitTime += BonusScoreTimerDelay;

        Player.Init();
        Player.Hide();
        GameState = GameState.Stopped;

        Hud.UpdateScore(_score);
        Hud.UpdateHighestScore(_highestScore);
        Hud.ShowRestartMenu(_score);

        GetNode<AudioStreamPlayer2D>("Music").Stop();
        GetNode<AudioStreamPlayer2D>("GameOver").Play();
    }
}
