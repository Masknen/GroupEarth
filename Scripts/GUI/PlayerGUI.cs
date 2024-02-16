using Godot;
using System;

public partial class PlayerGUI : Control
{
    private ProgressBar[] healthBar = new ProgressBar[2];

    private float[] targetValue = new float[2];

    public override void _Process(double delta) {
        healthBar[0] = GetChild(0).GetChild<ProgressBar>(0);
        healthBar[1] = GetChild(1).GetChild<ProgressBar>(0);

        foreach (var player in PlayerManager.Instance().players) {
            if (player.ID >= 0) {
                targetValue[player.ID] = (player.stats.GetStat(Stat.StatType.CurrentHealth) / (float)(player.stats.GetStat(Stat.StatType.MaxHealth)) * 100.0f);

                healthBar[player.ID].Value = Mathf.Lerp(healthBar[player.ID].Value, targetValue[player.ID], delta*10);
            }
        }
    }
}
