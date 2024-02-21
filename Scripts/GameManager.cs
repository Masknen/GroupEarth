using Godot;
using System;
using System.Diagnostics;

public partial class GameManager : Node {
    public static GameManager Instance { get; private set; }

    private PackedScene Menu;
    private PackedScene World;
    private PackedScene middleNode;
    private PackedScene playerManager;
    private PackedScene gameGUI;
    private PackedScene inputManager;

    Stopwatch sw = new Stopwatch();
    public override void _Ready() {
        Instance = this;

        // Menu = Gd.Load<PackedScene>("");
        World = GD.Load<PackedScene>("res://map_0_2.tscn");
        middleNode = GD.Load<PackedScene>("res://Scenes/middle_node.tscn");
        gameGUI = GD.Load<PackedScene>("res://Scenes/GUI Scenes/game_gui.tscn");
        playerManager = GD.Load<PackedScene>("res://Scenes/player_manager.tscn");
        inputManager = GD.Load<PackedScene>("res://Scenes/input_manager.tscn");

        CreateInputManager();
        //Instantiate Start Menu
    }

    public override void _Process(double delta) {
        if (InputManager.Instance().IsJustPressedButton(0, JoyButton.A)) {
            StartGame();
        }
    }

    public void StartGame() {
        sw.Start();

        CreateWorld();
        GD.Print(sw.ElapsedMilliseconds + " | World");
        sw.Restart();

        CreatePlayerManager();
        GD.Print(sw.ElapsedMilliseconds + " | PlayerManager");
        sw.Restart();

        CreateGameGUI();
        GD.Print(sw.ElapsedMilliseconds + " | GameGUI");
        sw.Restart();

        CreateMiddleNode();
        GD.Print(sw.ElapsedMilliseconds + " | MiddleNode");
        sw.Stop();
    }
    public void CreatePlayerManager() {
        var playerManagerInstance = playerManager.Instantiate();
        AddChild(playerManagerInstance);
    }

    public void CreateGameGUI() {
        var gameGUIInstance = gameGUI.Instantiate();
        AddChild(gameGUIInstance);
    }

    public void CreateMiddleNode() {
        var middleNodeInstance = middleNode.Instantiate();
        AddChild(middleNodeInstance);
    }

    public void CreateWorld() {
        var worldInstance = World.Instantiate();
        AddChild(worldInstance);
    }

    public void CreateInputManager() {
        var inputManagerInstance = inputManager.Instantiate();
        AddChild(inputManagerInstance);
    }


}
