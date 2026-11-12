using Godot;

public partial class Jelly : StaticBody3D
{
    [Export] private RayCast3D rayCast;
    [Export] private MeshInstance3D meshInstance;
    private Vector3 originalLocalPos;
    private Vector3 originalRotation;
    private Vector3 offset;
    private float dragDepth;
    private bool isTouching = false;
    private Camera3D mainCamera;
    private StandardMaterial3D originalMaterial;

    public override void _Ready()
    {

       
        originalLocalPos = Position;
        originalRotation = RotationDegrees;
        CallDeferred(nameof(GetCamera));
    }

    private void GetCamera()
    {
        mainCamera = GetViewport().GetCamera3D();
    }

    public void _on_input_event(Camera3D camera, InputEvent inputEvent,
        Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (mainCamera == null) return;

        if (inputEvent is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
        {
            OnMouseDown();
        }
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (!isTouching) return;

        if (inputEvent is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            !mouseButton.Pressed)
        {
            OnMouseUp();
        }
    }

    public override void _Process(double delta)
    {
        if (isTouching)
            OnMouseDrag();
    }

    private void OnMouseDown()
    {
        if(GameManager.Instance.isInputAllowed==false) return;
        isTouching = true;
        RotationDegrees = new Vector3(5, 0, 0);

        Vector2 mouse2D = GetViewport().GetMousePosition();
        dragDepth = GlobalPosition.DistanceTo(mainCamera.GlobalPosition);
        dragDepth = Mathf.Max(dragDepth, 1.5f);

        offset = GlobalPosition - mainCamera.ProjectPosition(mouse2D, dragDepth);
        originalMaterial = (StandardMaterial3D)meshInstance.GetActiveMaterial(0).Duplicate();
        meshInstance.SetSurfaceOverrideMaterial(0, originalMaterial);




    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.isInputAllowed == false) return;

        isTouching = false;

        if (rayCast != null && rayCast.IsColliding())
        {
            var collider = rayCast.GetCollider();


            if((collider as Node).Name=="Tray")
            {
                Position = originalLocalPos;
                RotationDegrees = originalRotation;
                return;
            }

            if (collider is StaticBody3D staticBody)
            {

            
                var parent= staticBody.GetParent();
                if (parent.FindChild("Jelly") is Node3D child)
                {
                    if(child.Visible)
                    {
                        Position = originalLocalPos;
                        RotationDegrees = originalRotation;
                        return;
                    }
                    ;
                    child.Visible = true;
                    MeshInstance3D targetMesh = child.FindChild("Line001") as MeshInstance3D;
            
                    Tween tween = CreateTween();
                    targetMesh.Scale = Vector3.Zero;
                    tween.TweenProperty(targetMesh, "scale", Vector3.One, 0.5f).SetTrans(Tween.TransitionType.Bounce);
                    tween.TweenCallback(Callable.From(() =>
                    {
                        GameManager.Instance.MoveCount++;
                        if(GameManager.Instance.MoveCount == PlayManager.Instance.gridSize * PlayManager.Instance.gridSize)
                        {
                            PlayManager.Instance.CheckLevelCompletion();
                        }

                    }));
                    StandardMaterial3D targetMaterial = (StandardMaterial3D)targetMesh.GetActiveMaterial(0).Duplicate();

                    targetMesh.SetSurfaceOverrideMaterial(0, targetMaterial);
                    targetMaterial.AlbedoColor=originalMaterial.AlbedoColor;
                }
            }

        }
        
            Position = originalLocalPos;
            RotationDegrees = originalRotation;
        
    }

    private void OnMouseDrag()
    {
        if (GameManager.Instance.isInputAllowed == false) return;

        Vector2 mouse2D = GetViewport().GetMousePosition();
        GlobalPosition = mainCamera.ProjectPosition(mouse2D, dragDepth) + offset;

    }
}