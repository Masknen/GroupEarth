using Godot;
using System;
using System.Diagnostics;

public partial class GameManager : Node {
    public double TimeSinceStart {  get; private set; }
    public float LastPositionUpdate { get; private set; }
    public float playersDeadTime = 0;
    public static GameManager Instance { get; private set; }

    //Preload scenes(player,fireball,etc)
    public PackedScene player { get; private set; }
    public PackedScene skeleton { get; private set; }
    public PackedScene rogue { get; private set; }
    public PackedScene interactableBox { get; private set; }
    public PackedScene mage { get; private set; }

    // Scenes
    private PackedScene startMenu;
    private PackedScene world;
    private PackedScene middleNode;
    private PackedScene playerManager;
    private PackedScene gameGUI;
    private PackedScene inputManager;

    private PackedScene soundManager;

    private Node startMenuInstance;
    private Node worldInstance;
    private Node middleNodeInstance;
    private Node playerManagerInstance;
    private Node gameGUIInstance;
    private Node inputManagerInstance;

    private Node soundManagerInstace;

    Stopwatch sw = new Stopwatch();

    public override void _Ready() {
        Instance = this;
        player = GD.Load<PackedScene>("res://Scenes/player.tscn");
        skeleton = GD.Load<PackedScene>("res://Scenes/enemy_1.tscn");
        rogue = GD.Load<PackedScene>("res://Scenes/enemy_2.tscn");
        mage = GD.Load<PackedScene>("res://Scenes/enemy_3.tscn");
        interactableBox = GD.Load<PackedScene>("res://Scenes/interactable_box.tscn");

        startMenu = GD.Load<PackedScene>("res://Scenes/GUI Scenes/menu.tscn");
        world = GD.Load<PackedScene>("res://Scenes/full_session_map.tscn");
        middleNode = GD.Load<PackedScene>("res://Scenes/middle_node.tscn");
        gameGUI = GD.Load<PackedScene>("res://Scenes/GUI Scenes/game_gui.tscn");
        playerManager = GD.Load<PackedScene>("res://Scenes/player_manager.tscn");
        inputManager = GD.Load<PackedScene>("res://Scenes/input_manager.tscn");
        soundManager = GD.Load<PackedScene>("res://Scenes/SFX Scenes/sound_manager.tscn");


        //Instantiate Start Menu
        CreateInputManager();
        CreateStartMenu();
    }

    public override void _Process(double delta) {
        LastPositionUpdate += (float)delta;
        if (LastPositionUpdate >= 0.5f) {
            LastPositionUpdate = 0;
        }
        TimeSinceStart += delta;

        try {
            int i = 0;
            if (PlayerManager.Instance.players.Count == 2) {
                i = 1;
            }
            if (PlayerManager.Instance.players[0].isDead && PlayerManager.Instance.players[i].isDead) {
                playersDeadTime += (float)delta;
            } else {
                playersDeadTime = 0;
            }
            if (playersDeadTime >= 5) {
                playersDeadTime = 0;
                (startMenuInstance as Control).Visible = true;
                DestroyGame();
            }
        } catch (Exception e) { }
        
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
        sw.Restart();

        CreateSoundManager();
        GD.Print(sw.ElapsedMilliseconds + " | SoundManager");
        sw.Stop();

        GD.Print(GetTreeStringPretty());
    }
    public void DestroyGame() {
        GD.Print(GetTreeStringPretty());
        playerManagerInstance.QueueFree();
        worldInstance.QueueFree();
        gameGUIInstance.QueueFree();
        middleNodeInstance.QueueFree();
        soundManagerInstace.QueueFree();
        GD.Print(GetTreeStringPretty());
    }
    public void CreatePlayerManager() {
        playerManagerInstance = playerManager.Instantiate();
        AddChild(playerManagerInstance);
    }
    public void CreateGameGUI() {
        gameGUIInstance = gameGUI.Instantiate();
        AddChild(gameGUIInstance);
    }

    public void CreateSoundManager(){
        soundManagerInstace = soundManager.Instantiate();
        AddChild(soundManagerInstace);
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
