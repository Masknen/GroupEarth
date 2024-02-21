using Godot;
using System;
using System.Diagnostics;

public partial class GameManager : Node {
    public double TimeSinceStart {  get; private set; }
    public static GameManager Instance { get; private set; }

    private PackedScene startMenu;
    private PackedScene world;
    private PackedScene middleNode;
    private PackedScene playerManager;
    private PackedScene gameGUI;
    private PackedScene inputManager;

    private Node startMenuInstance;
    private Node worldInstance;
    private Node middleNodeInstance;
    private Node playerManagerInstance;
    private Node gameGUIInstance;
    private Node inputManagerInstance;

    Stopwatch sw = new Stopwatch();

    public override void _Ready() {
        Instance = this;

        startMenu = GD.Load<PackedScene>("res://Scenes/GUI Scenes/menu.tscn");
        world = GD.Load<PackedScene>("res://map_0_2.tscn");
        middleNode = GD.Load<PackedScene>("res://Scenes/middle_node.tscn");
        gameGUI = GD.Load<PackedScene>("res://Scenes/GUI Scenes/game_gui.tscn");
        playerManager = GD.Load<PackedScene>("res://Scenes/player_manager.tscn");
        inputManager = GD.Load<PackedScene>("res://Scenes/input_manager.tscn");


        //Instantiate Start Menu
        CreateInputManager();
        CreateStartMenu();
    }

    public override void _Process(double delta) {
        TimeSinceStart += delta;
    }

    public void StartGame() {
        GD.Print(GetTreeStringPretty());
        (startMenuInstance as Control).Visible = false;

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
        playerManagerInstance = playerManager.Instantiate();
        AddChild(playerManagerInstance);
    }
    public void CreateGameGUI() {
        gameGUIInstance = gameGUI.Instantiate();
        AddChild(gameGUIInstance);
    }
    public void CreateMiddleNode() {
        middleNodeInstance = middleNode.Instantiate();
        AddChild(middleNodeInstance);
    }
    public void CreateWorld() {
        worldInstance = world.Instantiate();
        AddChild(worldInstance);
    }
    public void CreateInputManager() {
        inputManagerInstance = inputManager.Instantiate();
        AddChild(inputManagerInstance);
    }
    public void CreateStartMenu() {
        startMenuInstance = startMenu.Instantiate();
        AddChild(startMenuInstance);
        (startMenuInstance as StartMenu).StartPressedEvent += StartButtonPressed;
    }

    private void StartButtonPressed(object sender, EventArgs e) {
        StartGame();
    }
}
