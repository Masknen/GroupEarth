using Godot;
using System;

public partial class PlayerManager : Node3D
{
    static private PlayerManager _instance;

    static public PlayerManager Instance() {
        if (_instance == null) {
            _instance = new PlayerManager();
        }
        return _instance;
    }

    public Godot.Collections.Array<Player> players = new Godot.Collections.Array<Player>();
    private Godot.Collections.Array<int> playersToCreateID = new Godot.Collections.Array<int>();
    private PackedScene player;

    private bool[] startJustPressed = new bool[2];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Input.JoyConnectionChanged += Input_JoyConnectionChanged;
        player = GD.Load<PackedScene>("res://Scenes/player.tscn");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        var ConnectedJoyPadIDs = Input.GetConnectedJoypads();
        int i = 0;
        foreach (var ID  in ConnectedJoyPadIDs) {
            if (Input.IsJoyButtonPressed(ID, JoyButton.Start) && startJustPressed[i]) {
                startJustPressed[i] = false;
                playersToCreateID.Add(ID);
                GD.Print(playersToCreateID);
            }
            if (Input.IsJoyButtonPressed(ID, JoyButton.Back)) {
                playersToCreateID.Remove(ID);
                GD.Print(playersToCreateID);
            }
            if (!Input.IsJoyButtonPressed(ID, JoyButton.Start) && !startJustPressed[i]) {
                startJustPressed[i] = true;
            }
            ++i;
        }
        if (playersToCreateID.Count >= 2) {
            playersToCreateID.RemoveAt(0);
        }
        if(Input.IsActionJustPressed("spawnPlayers")) {
            SpawnPlayers();
        }
	}

    public void SpawnPlayers() {
        int spawnOffsetX = 1;
        foreach (var ID in playersToCreateID) {
            var newPlayer = player.Instantiate();
            (newPlayer as Player).ID = ID;
            (newPlayer as Player).Position = new Vector3 (spawnOffsetX, 1, 0);
            AddChild(newPlayer);

            spawnOffsetX -= spawnOffsetX * 2;
            players.Add(newPlayer as Player);
        }
    }

    private void Input_JoyConnectionChanged(long device, bool connected) {
        
    }

}
