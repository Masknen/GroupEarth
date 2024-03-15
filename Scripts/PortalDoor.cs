using Godot;
using System;

public partial class PortalDoor : Area3D
{

    private bool playNextScene = false;
    public override void _Ready() {
        BodyEntered += playerEntersPortal;
    }

    public override void _Process(double delta)
    {
        //nextScene();
    }

    private void playerEntersPortal(Node3D body) {
        
        if(PortalAsset.Instance.portalOpen == true){
            GD.Print("open");
        if (body.IsInGroup("player") ) {
            GD.Print("should be invis");
            body.Visible = false;
            nextScene();
            //playersEnteredPortal += 1;

        }
        }
    }

    private void nextScene(){
        try {
            int i = 0;
            if (PlayerManager.Instance.players.Count == 2) {
                i = 1;
            }
            if (!PlayerManager.Instance.players[0].Visible && !PlayerManager.Instance.players[i].Visible) {
                
                if(!playNextScene){
                    GameManager.Instance.deathScreenInstance ??= GameManager.Instance.deathScreen.Instantiate();
                    GameManager.Instance.AddChild(GameManager.Instance.deathScreenInstance);
                    GameManager.Instance.deathScreenInstance.GetChild<Label>(1).Text = "Your time:\n" + Math.Truncate(GameManager.Instance.TimeSinceStart / 60) + " min | " + Math.Truncate(GameManager.Instance.TimeSinceStart % 60) + " sec";
                    playNextScene = true;
                }
               
            } 
        
        } catch (Exception) { }
    }
}
