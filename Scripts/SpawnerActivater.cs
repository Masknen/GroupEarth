using Godot;
using System;

public partial class SpawnerActivater : Area3D
{
    public override void _Ready() {
        BodyEntered += SpawnerActivater_BodyEntered;
    }

    private void SpawnerActivater_BodyEntered(Node3D body) {
        GD.Print(body);
        if ((body as Player) != null) {
            ArcadeSpawner.Instance.mobsShouldSpawn = true;
        }
    }
}
