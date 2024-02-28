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

		//Max Need this
		camera = GetChild<Camera3D>(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		playerList = PlayerManager.Instance.players;
		float nodePosX = 0;
		float nodePosZ = 0;
		for (int i = 0; i < playerList.Count; i++) {
			nodePosX += playerList[i].Position.X;
            nodePosZ += playerList[i].Position.Z;
        }
		nodePosX /= playerList.Count;
		nodePosZ /= playerList.Count;

		this.Position = new Vector3(nodePosX, Position.Y, nodePosZ);
	}
}
