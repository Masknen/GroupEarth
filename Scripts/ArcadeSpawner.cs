using Godot;
using System;

public partial class ArcadeSpawner : Node3D {
    private double spawnTimer = 0; //Global timer sometime maybe
    PackedScene witch;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        witch = GD.Load<PackedScene>("res://Scenes/enemy_1.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        spawnTimer += delta;
        if (spawnTimer >= 1) {
            GD.Print("Instantiate tried");
            spawnTimer = 0;

            var spawnedWitch = witch.Instantiate();
            GetTree().Root.AddChild(spawnedWitch);
            //(spawnedWitch as enemy1).Position = new Vector3(0, 5, 0);
            GD.Print("instantiate should have happened");
        }
    }
}
