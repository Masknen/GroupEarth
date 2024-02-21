using Godot;
using System;
using System.Diagnostics;

public partial class ArcadeSpawner : Node3D {

    private double spawnTimer = 0; //Global timer sometime maybe
    PackedScene enemy1;
    PackedScene enemy2;
    PackedScene enemy3;
    int enemyCost = 1;
    int currentTokens = 0;
    int maxTokens = 7;

    private bool mobsShouldSpawn = false;


    public override void _Ready() {
        enemy1 = GD.Load<PackedScene>("res://Scenes/enemy_1.tscn");
        enemy2 = GD.Load<PackedScene>("res://Scenes/enemy_2.tscn");
        enemy3 = GD.Load<PackedScene>("res://Scenes/enemy_3.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        spawnTimer += delta;
        uint chosenEnemy = GD.Randi() % 3;
        //GD.Print(chosenEnemy);

        //debug option F4----
        var enemies = GetTree().GetNodesInGroup("enemies");
        if (Input.IsActionJustPressed("DespawnMobs") && PlayerManager.Instance().debugBoolean) {
            mobsShouldSpawn = false;
            foreach (var enemy in enemies) {
                ((CharacterBody3D)enemy).QueueFree();

            }
        }
        //debug option F2
        if (Input.IsActionJustPressed("SpawnMobs") && PlayerManager.Instance().debugBoolean) {
            mobsShouldSpawn = true;
        }

        if (mobsShouldSpawn) {
            if (spawnTimer >= 4) {

                var spawnedEnemy = enemy1.Instantiate(); //It needs a default
                currentTokens += 1;

                if (chosenEnemy == 1) {
                    spawnedEnemy = enemy2.Instantiate();
                    currentTokens += 2;
                }
                if (chosenEnemy == 2) {
                    spawnedEnemy = enemy3.Instantiate();
                    currentTokens += 4;
                }

                GetTree().Root.AddChild(spawnedEnemy);
                float dirAngle = GD.Randf() * (2 * MathF.PI);
                Transform3D lookTransfrom = new Transform3D(new Basis(Transform.Basis.Y, dirAngle), new Vector3(0, 2, 0));

                (spawnedEnemy as CharacterBody3D).Transform = GetParentNode3D().Transform;
                (spawnedEnemy as CharacterBody3D).Position += lookTransfrom.Basis.Y;
                (spawnedEnemy as CharacterBody3D).Position += -lookTransfrom.Basis.Z * 17;


                if (currentTokens >= maxTokens) {
                    spawnTimer = 0;
                    currentTokens = 0;
                }
            }

        }
    }

}
