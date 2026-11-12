

using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;





public partial class PlayManager : Node3D
{
    public static PlayManager Instance { get; private set; }
    public object UImanager { get; private set; }

    private List<Color> colorList = new List<Color>();
    [Export] private Node3D[] boxset = System.Array.Empty<Node3D>();
    private HashSet<Color> uniqueColor;
    private Dictionary data;
    [Export] private TwoByTwo twobytwo;
    private int currentLevel = 0;
    [Export] private Label timerText;
    private bool timerIsRunning=false;
    private double timer=0;
    public int gridSize=0;
    [Export] private Color star3Color;
    [Export] private Color star2Color;
    [Export] private Color star1Color;
    [Export] private TextureButton pauseButton;

    public override void _EnterTree()
    {
        Instance = this;
    }
    public override  void _Ready()
	{
        
        GameManager.Instance.GetConfigValue("Level", "CurrentLevel", 1, out Variant value);
        currentLevel = (int)value;

        LoadLevelData();
        ReadValues();
        CalculateUniqeColor();
        GD.Print(uniqueColor.Count);
        ColorChange(colorList);
        twobytwo.OnReady(colorList);
        pauseButton.Pressed += Onpaused;

    }

    private void CalculateUniqeColor()
    {
        uniqueColor = new HashSet<Color>(colorList);
    }
    private void LoadLevelData()
    {
        string levelPath = $"res://Level_data/Level_{currentLevel}.json";
        if (!FileAccess.FileExists(levelPath))
        {
            GD.Print("Config file not found!");
            return;
        }

        using var file = FileAccess.Open(levelPath, FileAccess.ModeFlags.Read);
        string jsonString = file.GetAsText();

        data = Json.ParseString(jsonString).AsGodotDictionary();
    }


    private void ReadValues()
    {
        colorList.Clear();
         gridSize = (int)(long)data["gridSize"];
        //float timer = (float)(double)data["timer"];
        float freezeTime = (float)(double)data["freezetime"];
        float showingTime = (float)(double)data["showingTime"];

        int star3Coin = (int)(long)data["star3coin"];

        GD.Print($"Grid: {gridSize}, Timer: {timer}");
        GD.Print($"Freeze: {freezeTime}, Show: {showingTime}");
        var colors = (Array)data["colors"];

        foreach (var color in colors)
        {
            colorList.Add(Color.FromString(color.ToString(), Colors.White));
        }
    }
    private void ColorChange(List<Color> boxColor)
    {
        for (int i = 0; i < boxset.Length; i++)
        {

            if(i >= uniqueColor.Count)
            {
                var parent = boxset[i].GetParent();

                if (parent is Node3D node3D)
                {
                    node3D.Visible = false;
                }

            }
            else
            {
                MeshInstance3D targetMesh = boxset[i].FindChild("Line001") as MeshInstance3D;
                StandardMaterial3D targetMaterial = (StandardMaterial3D)targetMesh.GetActiveMaterial(0).Duplicate();

                targetMesh.SetSurfaceOverrideMaterial(0, targetMaterial);
                targetMaterial.AlbedoColor = uniqueColor.ToList()[i];
            }
            

        }
    }

    public void CheckLevelCompletion()
    {
        timerIsRunning=false;
        GameManager.Instance.isInputAllowed = false;
        GameManager.Instance.MoveCount = 0;
        bool allMatched = true;
        int i = 0;
        foreach (Node3D box in twobytwo.boxset)
        {
            
            if (box.Visible)
            {
                MeshInstance3D targetMesh = box.FindChild("Line001") as MeshInstance3D;
                StandardMaterial3D targetMaterial = (StandardMaterial3D)targetMesh.GetSurfaceOverrideMaterial(0);
                if (targetMaterial.AlbedoColor != colorList[i++])
                {
                    allMatched = false;
                    GameOver();
                    break;
                }
            }
        }
        if (allMatched)
        {
            GameWin();
           
        }
    }



    private void GameOver()
    {
        GD.Print("Game Over!");
    }


    private void GameWin()
    {
        UIManager.Instance.ShowGameWinPanel();

        //
        GD.Print("You Win!");
    }
    public void Onpaused()
    {
        UIManager.Instance.TogglePause(true);
    }
    public void StartTimer()
    {

        timerIsRunning = true;

    }
    public override void _Process(double delta)
    {
        if (timerIsRunning)
        {
            timer += delta;
            float star3time = (float)(double)data["star3Time"];
            float star2time = (float)(double)data["star2Time"];
            int minutes = (int)(timer / 60);
            int seconds = (int)(timer % 60);
            timerText.Text = $"{minutes:00}:{seconds:00}";
            if (timer < star3time)
            {
                timerText.LabelSettings.FontColor=star3Color;
            }
            else if(timer< star2time)
            {
                timerText.LabelSettings.FontColor = star2Color;

            }
            else
            {
                timerText.LabelSettings.FontColor = star1Color;

            }
        }

    }

    public override void _ExitTree()
    {
        QueueFree();
    }
}
