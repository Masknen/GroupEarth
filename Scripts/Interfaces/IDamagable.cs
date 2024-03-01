using Godot;
using System;

public interface IDamagable
{
    public bool Hit(Node hitter, int damage);
}
