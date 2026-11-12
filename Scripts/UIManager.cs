using Godot;
using System;

public partial class UIManager : CanvasLayer
{
    public static UIManager Instance { get; private set; }
    [Export] private Texture2D startBg;
    [Export] private Texture2D levelBg;
    [Export] private TextureRect bg;
    [Export] private Panel startPanel;
    [Export] private Panel levelPanel;
    [Export] private TextureButton startButton;
    [Export] private TextureButton backButton;
    [Export] private Panel levelGrid;
    [Export] private Panel gamewinPanel;
    [Export] private Panel pausePanel;
    [Export] private TextureButton resumeButton;
    [Export] private TextureButton homeButton;



    private GridContainer levelGridContainer;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }
    public override void _Ready()
    {

        bg.Texture = startBg;
        startButton.Connect("pressed", new Callable(this, nameof(OnStartButtonPressed)));
        backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
        
        levelGridContainer = levelGrid.GetChild(0).GetChild(0) as GridContainer;
        resumeButton.Pressed += () => TogglePause(false);
        homeButton.Pressed +=OnHomeButtonPressed;
        RefreshLevelGrid();
    }

    private void OnStartButtonPressed()
    {
        bg.Texture = levelBg;
        startPanel.Visible = false;
        levelPanel.Visible = true;
        


    }
    private void OnBackButtonPressed()
    {
        bg.Texture = startBg;
        startPanel.Visible = true;
        levelPanel.Visible = false;
    }


    private void RefreshLevelGrid()
    {

        for (int i = 0; i < levelGridContainer.GetChildCount(); i++)
        {
            if (i < levelGridContainer.GetChildCount()-5)
            {
                (levelGridContainer.GetChild(i).GetChild(0) as Control).Visible = true;
                ((levelGridContainer.GetChild(i).GetChild(0) as Control).GetChild(0) as Label).Text = $"{i + 1}";
                (levelGridContainer.GetChild(i).GetChild(2) as TextureButton).Visible = true;
                
                var level = i + 1;
                Action handler = () => OnLevelPressed(level);

                
                (levelGridContainer.GetChild(i).GetChild(2) as TextureButton).Pressed += handler;

                (levelGridContainer.GetChild(i).GetChild(1) as TextureRect).Visible = false;
            }
            else
            {
                (levelGridContainer.GetChild(i).GetChild(2) as TextureButton).Visible = false;

                (levelGridContainer.GetChild(i).GetChild(0) as Control).Visible = false;
                (levelGridContainer.GetChild(i).GetChild(1) as TextureRect).Visible = true;
            }

        }

    }



    private void OnLevelPressed(int level)
    {
        GameManager.Instance.SetConfigValue("Level", "CurrentLevel", level);
        levelPanel.Visible = false;
        bg.Visible = false;
        var error = GetTree().ChangeSceneToFile("res://Scene/gameplay.tscn");
        GD.Print("Scene change result: " + error);

    }

    public override void _Process(double delta)
    {


    }
    public void ShowGameWinPanel()
    {
        gamewinPanel.Visible = true;
    }
   public void TogglePause(bool value)
    { 
        pausePanel.Visible = value;
        Engine.TimeScale = !value ? 1 : 0;
    }
   public void OnHomeButtonPressed()
    {
        gamewinPanel.Visible= false;
        startPanel.Visible = true;
        bg.Visible = true;
        bg.Texture = startBg;
        GetTree().ChangeSceneToFile("res://Scene/main_menu.tscn");
       

    }
   



}
