using Godot;
using System;

public partial class RayCast3d : RayCast3D
{
    public void CheckCollision()
    {
        if (IsColliding())
        {
            GD.Print("Colliding with: " + GetCollider());
        }
    }
}
