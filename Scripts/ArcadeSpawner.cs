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
        if (spawnTimer >= 5) {
            GD.Print("Instantiate tried");
            spawnTimer = 0;

            Node spawnedWitch = witch.Instantiate();

            float dirAngle = GD.Randf() * (2 * MathF.PI); 
            Transform3D lookTransfrom = new Transform3D(new Basis(Transform.Basis.Y, dirAngle), Position);
            (spawnedWitch as enemy1).Position = -lookTransfrom.Basis.Z * 11;
            //GD.Print(dirAngle);
            //GD.Print(2 * MathF.PI);

            GetTree().Root.AddChild(spawnedWitch);

            
            GD.Print("instantiate should have happened");
        }
    }
}
