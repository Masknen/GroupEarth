using Godot;
using System;

public partial class PlayerRessEffect : Node3D
{
    private float MaxTime = 1f;
	private float timeTick = 0;
    
    public override void _Ready(){
        

    }
    public override void _Process(double delta)
    {
        
        timeTick += (float)delta;
				if (timeTick > MaxTime) {
					QueueFree();	
				}
                
        
    }
}