using Godot;
using System;
using System.Collections.Generic;
[Tool]

public partial class SoundQue : Node
{
    private int _next = 0;
    private List<AudioStreamPlayer> _audioStreamPlayers = new List<AudioStreamPlayer>();

    [Export]
    public int count { get; set;} = 1;

    public override void _Ready()
    {
        if(GetChildCount() == 0)
        {
            GD.Print("error - No audioplayer found");
            return;
        }
        var child = GetChild(0);
        if (child is AudioStreamPlayer audioStreamPlayer){
            _audioStreamPlayers.Add(audioStreamPlayer);

            for(int i = 0; i < count; i++){

                AudioStreamPlayer duplicate = audioStreamPlayer.Duplicate() as AudioStreamPlayer;
                AddChild(duplicate);
                _audioStreamPlayers.Add(duplicate);
            }
        }
    }

    public override string[] _GetConfigurationWarnings(){
        if(GetChildCount() == 0)
        {
            return new string[] {"Excpect one AudioStreamPlayer"};
        }
        if(GetChild(0) is not AudioStreamPlayer){

            return new string[] {"Excpect first child to be a AudioStreamPlayer"};
        }
        return base._GetConfigurationWarnings();
    }

    public void PlaySound(){
        if(!_audioStreamPlayers[_next].Playing)
        {
            _audioStreamPlayers[_next++].Play();
            _next %= _audioStreamPlayers.Count;
        }
    }

}
