using Godot;
using System;
using Godot.Collections;

public partial class GameManager : Node2D
{

    public static GameManager Instance { get; private set; }

    public bool isInputAllowed = false;
    private string datPath = "res://Data/powerups.dat";

    private ConfigFile config = new ConfigFile();
    private string path = "res://Data/Data.cfg";
    public int undoCount = 5;
    public int coinCount = 5;
    public int freezeCount = 5;
    public int autoPlaceCount = 5;
    public int revealCount = 5;

    public int MoveCount = 0;
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
    public void SavePowerups()
    {
        using var file = FileAccess.Open(datPath, FileAccess.ModeFlags.Write);

        var data = new Dictionary
    {
        { "undo", undoCount },
        { "coin", coinCount },
        { "freeze", freezeCount },
        { "autoPlace", autoPlaceCount },
        { "reveal", revealCount }
    };

        file.StoreVar(data);
    }
    public override void _Ready()
    {
        LoadPowerups();
    }


    public void SetConfigValue(string section, string key, Variant value)
    {
        config.SetValue(section, key, value);
        config.Save(path);
    }

    public void GetConfigValue(string section, string key, Variant defaultValue, out Variant value)
    {
        config.Load(path);
        value = config.GetValue(section, key, defaultValue);

    }

    public void LoadPowerups()
    {
        if (!FileAccess.FileExists(datPath))
        {
            GD.Print("No save file found, using defaults.");
            SavePowerups();
             return;
        }

        using var file = FileAccess.Open(datPath, FileAccess.ModeFlags.Read);
        var data = (Dictionary)file.GetVar();

        undoCount = (int)(long)data["undo"];
        coinCount = (int)(long)data["coin"];
        freezeCount = (int)(long)data["freeze"];
        autoPlaceCount = (int)(long)data["autoPlace"];
        revealCount = (int)(long)data["reveal"];
        GD.Print(undoCount,coinCount, freezeCount, autoPlaceCount, revealCount);
    }

}
