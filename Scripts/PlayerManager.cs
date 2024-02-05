using Godot;
using System;

public partial class PlayerManager : Node3D
{
    public PlayerManager Instance { get; private set; }

    public Godot.Collections.Array<Player> players {  get; private set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Input.JoyConnectionChanged += Input_JoyConnectionChanged;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}

    public void SpawnPlayers() {

    }

    private void Input_JoyConnectionChanged(long device, bool connected) {
        
    }

}
