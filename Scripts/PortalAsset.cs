using Godot;
using System;

public partial class PortalAsset : Node3D
{
    public static PortalAsset Instance {get; private set;}
    private GpuParticles3D portal;
    private OmniLight3D portalChargeLight;

    private Node3D _ExplosionPos;

    private PackedScene portalOpeningExplosion;

    private Area3D portalDoor;

    public float chargeAmount = 0;

    public bool portalOpen = false;

    

    public override void _Ready()
    {
        
        Instance = this;

        portal                 = GetNode<GpuParticles3D>("PortalVFX");
        portal.Visible = false;

        portalChargeLight      = GetNode<OmniLight3D>("PortalChargeLight");
        _ExplosionPos          = GetNode<Node3D>("ExplosionPos");
        portalOpeningExplosion = GD.Load<PackedScene>("res://AnimationScenes/greenExplosion.tscn"); 
        portalDoor             = GetNode<Area3D>("PortalDoor");
    }

    public override void _Process(double delta)
    {
        openPortal(); 
        
    }

    private void greenPortalExplosion() {
        var new_Explosion = portalOpeningExplosion.Instantiate();
        (new_Explosion as greenExplosion).Position = _ExplosionPos.GlobalPosition;
        GetParent().AddChild(new_Explosion);
    }

    public void chargePortal(){
        chargeAmount += 1;
    }

    private void openPortal(){
        chargeAmount = ArcadeSpawner.Instance.currentWave;
        portalChargeLight.OmniRange = chargeAmount;
        if(chargeAmount >= 2){
            if(!portalOpen){
            greenPortalExplosion();
           portalOpen = true;
            } 
            portal.Visible = true;
            portalChargeLight.Visible = false;
        }
        

    }
    private void playerEnterPortal(Node3D body){


    }
}
