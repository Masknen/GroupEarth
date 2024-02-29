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
        //--SoundQueues
        _soundQueuesByName.Add("MenuBackGroundSound", GetNode<SoundQue>("MenuMusic"));
        _soundQueuesByName.Add("MenuButtonPressedSound", GetNode<SoundQue>("MenuButtonPressed"));
        _soundQueuesByName.Add("DungeonAmbient", GetNode<SoundQue>("DungeonAmbient"));
        _soundQueuesByName.Add("DangerAreaMusic", GetNode<SoundQue>("DangerAreaMusic"));
        _soundQueuesByName.Add("DeflectSound", GetNode<SoundQue>("DeflectSound"));
        _soundQueuesByName.Add("BigObjectHit", GetNode<SoundQue>("BigObjectHit"));
        _soundQueuesByName.Add("PortalOpeningSound", GetNode<SoundQue>("PortalOpenSound"));
        _soundQueuesByName.Add("Dialog1", GetNode<SoundQue>("Dialog1"));
        _soundQueuesByName.Add("Dialog2", GetNode<SoundQue>("Dialog2"));
        _soundQueuesByName.Add("Dialog3", GetNode<SoundQue>("Dialog3"));
        //--SoundPools
        /*
        ---index---
        -FireBallHitSound
        -WalkingSound
        -FireBallSound
        --------------------
        */
        _soundPoolsByName.Add("WalkingSound",GetNode<SoundPool>("WalkingPool"));
        _soundPoolsByName.Add("FireBallSound",GetNode<SoundPool>("FireSoundPool"));
        _soundPoolsByName.Add("FireBallHitSound",GetNode<SoundPool>("FireHitSoundPool"));

        
    }
    //-- Dialog 
    public void PlayDialogOne(){
        _soundQueuesByName["Dialog1"].PlaySound();
    }
    public void PlayDialogTwo(){
        _soundQueuesByName["Dialog2"].PlaySound();
    }
    public void PlayDialogThree(){
        _soundQueuesByName["Dialog3"].PlaySound();
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
    public void PlayDangerAreaMusic(){
        _soundQueuesByName["DangerAreaMusic"].PlaySound();
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
        _soundPoolsByName["FireBallSound"].PlayRandomSound();
    }
    public void PlayFireBallHitSound(){
        _soundPoolsByName["FireBallHitSound"].PlayRandomSound();
    }
    public void PlayDeflectSound(){
        _soundQueuesByName["DeflectSound"].PlaySound();
    }
    public void PlayBigObjectHit(){
        _soundQueuesByName["BigObjectHit"].PlaySound();
    }
    public void PlayPortalOpeningSound(){
        _soundQueuesByName["PortalOpeningSound"].PlaySound();
    }
}
