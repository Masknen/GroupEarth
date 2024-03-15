using Godot;
using System;

public partial class DialogArea : Area3D
{
    
    private bool runOnce = true;

    private Label dialog;
    private Label hint;
    private Timer timer;
    private Timer hintTimer;

    public override void _Ready() {
        BodyEntered += SpawnerActivater_BodyEntered;
        dialog = GetNode<Label>("Label1");
        timer = GetNode<Timer>("DialogTimer");
        hintTimer = GetNode<Timer>("HintTimer");
        hint = GetNode<Label>("Label2");
        timer.Timeout += dialogVisibleSwitch;
        hintTimer.Timeout += hintVisibleSwitch;
    }

    private void dialogVisibleSwitch(){
        if(dialog.Visible){
            dialog.Visible = false;
        }
        
    }
    private void hintVisibleSwitch(){
        if(hint.Visible){
            hint.Visible = false;
        }
        
    }

    private void SpawnerActivater_BodyEntered(Node3D body) {
        if ((body as Player) != null) {
            if(runOnce){
                dialog.Visible = true;
                hint.Visible = true;
                hintTimer.Start();
                timer.Start();
                SoundManager.Instance.PlayDialogOne();
                runOnce = false;
            }
            
        }
    }
}
