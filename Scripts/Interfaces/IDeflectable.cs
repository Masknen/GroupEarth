using Godot;

public interface IDeflectable
{  
    public void Deflect(float yRotation, Node3D target);
    public void FriendDeflect(float yRotation);
    public void ArcDeflect(float yRotation);
    public void Hold();
}
