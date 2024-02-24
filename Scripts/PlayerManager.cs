using Godot;
using System;


/*
 * Spawn players by adding your controller to the active list(playersToCreateID)
 * then start debug with f3
 * spawn players with f1
 */
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

    private bool[] startJustPressed = { true, true};
    public bool debugBoolean = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _instance = this;
        Input.JoyConnectionChanged += Input_JoyConnectionChanged;


        playersToCreateID = Input.GetConnectedJoypads();
        while (playersToCreateID.Count > 2) {
            playersToCreateID.RemoveAt(0);
        }
        SpawnPlayers();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        var ConnectedJoyPadIDs = Input.GetConnectedJoypads();
        int i = 0;
        foreach (var ID  in ConnectedJoyPadIDs) {
            if (Input.IsJoyButtonPressed(ID, JoyButton.Start) && !playersToCreateID.Contains(ID)) {
                playersToCreateID.Add(ID);
                //GD.Print("Added: " + ID);
            }
            if (Input.IsJoyButtonPressed(ID, JoyButton.Back) && playersToCreateID.Contains(ID)) {
                playersToCreateID.Remove(ID);
                //GD.Print("Removed: " + ID);
            }
            ++i;
        }
        if (playersToCreateID.Count > 2) {
            playersToCreateID.RemoveAt(0);
        }
        if(Input.IsActionJustPressed("spawnPlayers") && debugBoolean) {
            //GD.Print("Trying to spawn Players: " + playersToCreateID);
            SpawnPlayers();
        }
        if (Input.IsActionJustPressed("EnableDebug")) {
            debugBoolean = !debugBoolean;
            //GD.Print("Debug: " + debugBoolean);
        }
	}

    public void SpawnPlayers() {
        int spawnOffsetX = 2;
        foreach (var ID in playersToCreateID) {
            var newPlayer = GameManager.Instance.player.Instantiate();
            (newPlayer as Player).setID(ID);
            (newPlayer as Player).Position = new Vector3 (spawnOffsetX, 1, 0);
            players.Add(newPlayer as Player);
            AddChild(newPlayer);

            spawnOffsetX -= spawnOffsetX * 2;
        }
        if (players.Count == 1) {
            var newPlayer = GameManager.Instance.player.Instantiate();
            (newPlayer as Player).ID = -1;
            (newPlayer as Player).Position = new Vector3(spawnOffsetX, 1, 0);
            players.Add(newPlayer as Player);
            AddChild(newPlayer);
        }


        if (players.Count > 2) {
            foreach (var player in players) {
                player.QueueFree();
            }
            players.Clear();
            SpawnPlayers();
        }
        //players[0].otherPlayer = players[1];
        //players[1].otherPlayer = players[0];
    }

    private void Input_JoyConnectionChanged(long device, bool connected) {
        
    }

}
