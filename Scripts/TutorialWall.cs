using Godot;
using System;

public partial class TutorialWall : StaticBody3D, IDamagable {
    public bool Hit(Node hitter, int damage) {
        if(damage >= 4) {
            SoundManager.Instance.PlayBigObjectHit();
            QueueFree(); 
            return true;
        }
        return true;
    }
}
