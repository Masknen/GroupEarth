using Godot;
using System;
using System.Collections.Generic;

public partial class SoundPool : Node
{
    private List<SoundQue> _sounds = new List<SoundQue>();
    private RandomNumberGenerator _random = new RandomNumberGenerator();

    private int _lastIndex = -1;
    public override void _Ready()
    {
        foreach(var child in GetChildren()){
            if(child is SoundQue soundQue){
                _sounds.Add(soundQue);
            }
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override string[] _GetConfigurationWarnings(){
        int numberOfSoundQueChildren = 0;
        foreach(var child in GetChildren()){
            if(child is SoundQue soundQueue){
                numberOfSoundQueChildren++;
            }
        }
        if(numberOfSoundQueChildren < 2){
            return new string[] {"Expect two or more children of type: SoundQue"};
        }
        return new string[0];

    }
    public void PlayRandomSound(){
        int index;
        do{
            index = _random.RandiRange(0, _sounds.Count- 1);
        }
        while(index == _lastIndex);

        _lastIndex = index;
        _sounds[index].PlaySound();
    }
}
