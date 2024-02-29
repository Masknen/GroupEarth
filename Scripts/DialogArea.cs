using Godot;
using System;

public partial class DialogArea : Area3D
{
    
    private bool runOnce = true;
    public override void _Ready() {
        BodyEntered += SpawnerActivater_BodyEntered;
    }

    private void SpawnerActivater_BodyEntered(Node3D body) {
        GD.Print(body);
        if ((body as Player) != null) {
            if(runOnce){
                SoundManager.Instance.PlayDialogOne();
                runOnce = false;
            }
            
            ArcadeSpawner.Instance.mobsShouldSpawn = true;
        }
    }
}
