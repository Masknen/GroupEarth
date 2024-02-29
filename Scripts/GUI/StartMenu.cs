using Godot;
using System;

public partial class StartMenu : Control
{
	private BaseButton startButton; 
	private BaseButton quitButton; 

	public event EventHandler StartPressedEvent; 
	public event EventHandler QuitPressedEvent; 



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startButton = 	GetChild(0).GetChild(1).GetChild<BaseButton>(0);
		quitButton = 	GetChild(0).GetChild(1).GetChild<BaseButton>(2);
		startButton.Pressed += StartPressed; 
		quitButton.Pressed += QuitPressed; 

		startButton.GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (InputManager.Instance().IsJustReleasedButton(0, JoyButton.A)) {
			if(startButton.HasFocus()) {
				StartPressed();
			}
            if (quitButton.HasFocus()) {
                QuitPressed();
            }
        }
	}
	public void StartPressed()
	{
		StartPressedEvent?.Invoke(this,EventArgs.Empty); 

	}
	public void QuitPressed()
	{
		QuitPressedEvent?.Invoke(this,EventArgs.Empty);
		GetTree().Quit();

	}
	
}
