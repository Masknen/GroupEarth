using Godot;
using System;
using System.Collections.Generic;

public class Stat
{
    public enum StatType {
        MaxHealth,
        CurrentHealth,
        MovementSpeed,
        RotationSpeed,
        DodgeStrength,
        DeflectMultiplier,
        Damage,
        Size,
    }

    private Dictionary<StatType, int> myStats;

    public Stat() {
        myStats = new Dictionary<StatType, int>();
    }

    public Stat AddStat(StatType statType, int value) {
        if (!myStats.TryAdd(statType, value)) {
            GD.PushWarning("Stat Already Added");
        }
        return this;
    }

    public Stat RemoveStat(StatType statType) {
        if (!myStats.Remove(statType)) {
            GD.PushWarning("Stat not present/removed");
        }
        return this;
    }

    public void ModifyStat(StatType statType, int newValue) {
        if (myStats.TryGetValue(statType, out int value)) {
            myStats[statType] = newValue;
        } else {
            GD.PushWarning("Stat not present");
        }
    }

    public int GetStat(StatType statType) {
        if (!myStats.TryGetValue(statType, out int value)) {
            GD.PushError("Stat does not exist");
        }
        return value;
    }

}
