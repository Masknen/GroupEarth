using Godot;
using Godot.Collections;
using System;

public partial class InputManager : Node
{
    static private InputManager _instance;

    static public InputManager Instance() {
        if (_instance == null) {
            _instance = new InputManager();
        }
        return _instance;
    }



    private Array<Dictionary<JoyButton, int>> playersPressed = new Array<Dictionary<JoyButton, int>>();
    //private Array<Dictionary<JoyButton, int>> playersReleased = new Array<Dictionary<JoyButton, int>>();
    public override void _Ready()
	{
        _instance = this;
        playersPressed.Add(new Dictionary<JoyButton, int>());
        playersPressed.Add(new Dictionary<JoyButton, int>());
        //playersReleased.Add(new Dictionary<JoyButton, int>());
        //playersReleased.Add(new Dictionary<JoyButton, int>());
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        for (int ID = 0; ID < 2; ID++) {
            for (int i = 0; i < (int)JoyButton.SdlMax; i++) {
                if (Input.IsJoyButtonPressed(ID, (JoyButton)i)) {
                    playersPressed[ID][(JoyButton)i]++;
                }
                if (!Input.IsJoyButtonPressed(ID, (JoyButton)i)) {
                    playersPressed[ID][(JoyButton)i] = 0;
                }
            }
        }
	}

    public bool IsJustPressed(int ID, JoyButton joyButton) {
        return playersPressed[ID][joyButton] == 1;
    }

}
