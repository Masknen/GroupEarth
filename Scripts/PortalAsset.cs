using Godot;
using System;

public partial class PortalAsset : Node3D
{
    public static PortalAsset Instance {get; private set;}
    private GpuParticles3D portal;
    private OmniLight3D portalChargeLight;

    public float chargeAmount = 0;

    public override void _Ready()
    {
        portal = GetNode<GpuParticles3D>("PortalVFX");
        portalChargeLight = GetNode<OmniLight3D>("PortalChargeLight");
    }

    public override void _Process(double delta)
    {
        Instance = this;
        portal.Visible = false;

        openPortal();
        
    }

    public void chargePortal(){
        chargeAmount += 1;
    }

    private void openPortal(){
        chargeAmount = ArcadeSpawner.Instance.currentWave;
        portalChargeLight.OmniRange = chargeAmount;
        if(chargeAmount >= 5){
            portal.Visible = true;
        }
        

    }
}
