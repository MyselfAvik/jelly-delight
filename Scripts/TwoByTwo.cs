using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class TwoByTwo : Node3D
{
    [Export] public Node3D[] boxset = Array.Empty<Node3D>();
    


    public  async void OnReady(List<Color> boxColor)
    {
        ColorChange(boxColor);
        await ShowOnce(boxset);
        PlayManager.Instance.StartTimer();
    }

    private void ColorChange(List<Color> boxColor)
    {
        for (int i = 0; i < boxset.Length; i++)
        {
            MeshInstance3D targetMesh = boxset[i].FindChild("Line001") as MeshInstance3D;
            StandardMaterial3D targetMaterial = (StandardMaterial3D)targetMesh.GetActiveMaterial(0).Duplicate();

            targetMesh.SetSurfaceOverrideMaterial(0, targetMaterial);
            targetMaterial.AlbedoColor = boxColor[i];

        }
    }

    private async Task ShowOnce(Node3D[] boxSet)
    {
        await ToSignal(GetTree().CreateTimer(1f), "timeout");

        for (int i = 0; i < boxSet.Length; i++)
        {
            Node3D jellyNode = boxSet[i];
            if (jellyNode == null) continue;

            jellyNode.Scale = Vector3.Zero;
            jellyNode.Visible = true;
            PopAnimation(jellyNode, 0.1f, 1.16f, 0.2f);
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }

        await ToSignal(GetTree().CreateTimer(2f), "timeout");

        for (int i = 0; i < boxSet.Length; i++)
        {
            Node3D jellyNode = boxSet[i];
            if (jellyNode == null) continue;

            Tween tween = PopAnimation(jellyNode, 0.1f, 0f, 0.2f);
            await ToSignal(tween, "finished");
            jellyNode.Visible = false;
            jellyNode.Scale = new Vector3(1.16f, 1.16f, 1.16f);
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }


       GameManager.Instance.isInputAllowed = true;
    }

    public static Tween PopAnimation(Node3D node, float extraScale, float finalScale, float duration)
    {
        Vector3 currentScale = node.Scale;
        Vector3 endScale = new Vector3(finalScale, finalScale, finalScale);
        Vector3 peakScale = currentScale + endScale + Vector3.One * extraScale;

        Tween tween = node.CreateTween();
        tween.TweenProperty(node, "scale", peakScale, duration / 2f)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(node, "scale", endScale, duration / 2f)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.In);

        return tween;
    }
}