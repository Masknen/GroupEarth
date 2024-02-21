using Godot;
using System;
using System.Diagnostics;

public partial class MiddleNode : Node3D
{
	public static MiddleNode Instance { get; private set; }
	private Godot.Collections.Array<Player> playerList = new Godot.Collections.Array<Player>();
	public Camera3D camera { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Stopwatch sw = new Stopwatch();
        Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		playerList = PlayerManager.Instance().players;
		// if (playerList.Count < 3){
		//GD.Print(playerList);
		// }
		// else{
		// 	GD.Print("Players not found by Camera Check PlayerCameras.cs");
		// }


		if (playerList.Count == 2)
		{
			var nodePosX = (playerList[0].Position.X + playerList[1].Position.X) / 2;
			var nodePosZ = (playerList[0].Position.Z + playerList[1].Position.Z) / 2;

			this.Position = new Vector3(nodePosX, Position.Y, nodePosZ);
		}
		//var cameraposY = playerList[0].Position.DistanceTo(playerList[1].Position);


		// if (Input.IsActionJustPressed("spawnPlayers"))
		// {
		// 	PlayerManager.Instance().SpawnPlayers();

		// }

	}
}
