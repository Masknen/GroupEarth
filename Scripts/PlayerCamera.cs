using Godot;
using System;

public partial class PlayerCamera : Camera3D
{
	private Godot.Collections.Array<Player> playerList;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerList = PlayerManager.Instance().players;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (playerList.Count < 3){
			var cameraPosX = (playerList[0].Position.X + playerList[1].Position.X) / 2;
			var cameraPosZ = (playerList[0].Position.Z + playerList[1].Position.Z) / 2;
			var cameraposY = playerList[0].Position.DistanceTo(playerList[1].Position);

			this.Position = new Vector3(cameraPosX, cameraposY, cameraPosZ);
		}
		else{
			GD.Print("Players not found by Camera Check PlayerCameras.cs");
		}
	}
}
