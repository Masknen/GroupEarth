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
        _soundQueuesByName.Add("MenuBackGroundSound", GetNode<SoundQue>("MenuMusic"));
        _soundQueuesByName.Add("MenuButtonPressedSound", GetNode<SoundQue>("MenuButtonPressed"));
        _soundQueuesByName.Add("DungeonAmbient", GetNode<SoundQue>("DungeonAmbient"));
        _soundPoolsByName.Add("WalkingSound",GetNode<SoundPool>("WalkingPool"));
        
    }
    //-- UI SFX - menu Music
    public void PlayMenuMusic(){
        _soundQueuesByName["MenuBackGroundSound"].PlaySound();
    }
    public void StopMenuMusic(){
        _soundQueuesByName["MenuBackGroundSound"].QueueFree();
    }
    
    public void MenuButtonSound(){
        _soundQueuesByName["MenuButtonPressedSound"].PlaySound();
    }
    //-- in-Game music/ambient
    public void DungeonAmbientSound(){
        _soundQueuesByName["DungeonAmbient"].PlaySound();
    }
    //-- MOVEMENT SFX
    public void PlayWalkingSound(){
        _soundPoolsByName["WalkingSound"].PlayRandomSound();
    }
     public void StopWalkingSound(){
        _soundPoolsByName["WalkingSound"].QueueFree();
    }

    //-- SPELL SFX
    public void PlayFireBallSound(){
        _soundQueuesByName["FireBallSound"].PlaySound();
    }
}
