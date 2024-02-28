using Godot;
using System;

public partial class PlayerCamera : Camera3D
{
	private Godot.Collections.Array<Player> playerList = new Godot.Collections.Array<Player>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		playerList = PlayerManager.Instance.players;
		// if (playerList.Count < 3){
		//GD.Print(playerList);
		// }
		// else{
		// 	GD.Print("Players not found by Camera Check PlayerCameras.cs");
		// }


		if(playerList.Count == 2){
		var cameraPosX = (playerList[0].Position.X + playerList[1].Position.X) / 2;
		var cameraPosZ = ((playerList[0].Position.Z + playerList[1].Position.Z) / 2) + 8;

		this.Position = new Vector3(cameraPosX, Position.Y , cameraPosZ);
		}
		//var cameraposY = playerList[0].Position.DistanceTo(playerList[1].Position);


		// if (Input.IsActionJustPressed("spawnPlayers"))
		// {
		// 	PlayerManager.Instance().SpawnPlayers();

		// }



	}
}
