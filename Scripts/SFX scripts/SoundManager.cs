using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class SoundManager : Node
{
    public static SoundManager Instance {get; private set;}

    private SoundQue fireBallSound;

    private Godot.Collections.Dictionary<string, SoundQue> _soundQueuesByName = 
    new Godot.Collections.Dictionary<string, SoundQue>();

    private Godot.Collections.Dictionary<string, SoundPool> _soundPoolsByName = 
    new Godot.Collections.Dictionary<string, SoundPool>();

    public override void _Ready()
    {
        
        Instance = this;
        
        _soundQueuesByName.Add("FireBallSound", GetNode<SoundQue>("FireSound"));
    }

    public void PlayFireBallSound(){
        //fireBallSound.PlaySound();
        _soundQueuesByName["FireBallSound"].PlaySound();
    }
}
