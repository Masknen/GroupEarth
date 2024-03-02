using Godot;
using System;

public partial class DialogArea2 : Area3D
{
    private bool runOnce = true;
    private Label dialog;
    private Timer timer;
    public override void _Ready() {
        dialog = GetNode<Label>("Label1");
        timer = GetNode<Timer>("DialogTimer");
        BodyEntered += SpawnerActivater_BodyEntered;
        timer.Timeout += dialogVisibleSwitch;
    }
    private void dialogVisibleSwitch(){
        if(dialog.Visible){
            dialog.Visible = false;
        }   
    }

    private void SpawnerActivater_BodyEntered(Node3D body) {
        GD.Print(body);
        if ((body as Player) != null) {
            if(runOnce){
                dialog.Visible = true;
                timer.Start();
                SoundManager.Instance.PlayDialogTwo();
                runOnce = false;
            }
            
            
        }
    }
}
