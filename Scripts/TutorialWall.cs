using Godot;
using System;

public partial class TutorialWall : StaticBody3D, IDamagable {
    public bool Hit(int damage) {
        if(damage >= 4) {
            QueueFree(); 
            return true;
        }
        return true;
    }
}
