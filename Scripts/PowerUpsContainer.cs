using Godot;
using System;

public partial class PowerUpsContainer : MarginContainer
{
    [Export] private Label freezeLabel;
	[Export] private Label revealLabel;
	[Export] private Label autoPlaceLabel;
	[Export] private Label undoLabel;




    public override void _Ready()     
	{
		UpdatePowerUps();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void UpdatePowerUps() { 
		undoLabel.Text = GameManager.Instance.undoCount.ToString();
		autoPlaceLabel.Text = GameManager.Instance.autoPlaceCount.ToString();
		revealLabel.Text = GameManager.Instance.revealCount.ToString();
		freezeLabel.Text = GameManager.Instance.freezeCount.ToString();
    }
}
