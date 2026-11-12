using Godot;


public partial class AutoViewport : Node
{
    /// <summary>
    /// Stretch mode to apply after resizing.
    /// Options: Disabled | CanvasItems | Viewport
    /// </summary>
    [Export]
    public Window.ContentScaleModeEnum StretchMode = Window.ContentScaleModeEnum.CanvasItems;

    /// <summary>
    /// Stretch aspect to apply.
    /// Options: Ignore | Keep | KeepWidth | KeepHeight | Expand
    /// </summary>
    [Export]
    public Window.ContentScaleAspectEnum StretchAspect = Window.ContentScaleAspectEnum.Expand;

    /// <summary>
    /// Optional minimum logical resolution. Set to Vector2.Zero to disable.
    /// </summary>
    [Export]
    public Vector2 MinimumSize = new Vector2(360, 640);

    /// <summary>
    /// Optional maximum logical resolution. Set to Vector2.Zero to disable.
    /// </summary>
    [Export]
    public Vector2 MaximumSize = Vector2.Zero;

    /// <summary>
    /// If true, re-applies on every orientation change (useful for mobile).
    /// </summary>
    [Export]
    public bool ListenForOrientationChange = true;


    public override void _Ready()
    {
        ApplyViewportSize();

        if (ListenForOrientationChange)
        {
            GetTree().Root.SizeChanged += OnWindowSizeChanged;
        }
    }


    // ─── Core Logic ───────────────────────────────────────────────────────────

    private void ApplyViewportSize()
    {
        Vector2 screenSize = GetScreenSize();
        Vector2 targetSize = ClampSize(screenSize);

        // Resize the OS window to match the screen
        DisplayServer.WindowSetSize(new Vector2I((int)targetSize.X, (int)targetSize.Y));

        // Apply stretch / scale settings
        ApplyStretchSettings(targetSize);

   
    }

    private Vector2 GetScreenSize()
    {
        // DisplayServer.ScreenGetSize() returns the physical pixel resolution
        // of the primary screen — works on desktop AND mobile (Android / iOS).
        Vector2I size = DisplayServer.ScreenGetSize();
        return new Vector2(size.X, size.Y);
    }

    private Vector2 ClampSize(Vector2 size)
    {
        Vector2 result = size;

        if (MinimumSize != Vector2.Zero)
        {
            result.X = Mathf.Max(result.X, MinimumSize.X);
            result.Y = Mathf.Max(result.Y, MinimumSize.Y);
        }

        if (MaximumSize != Vector2.Zero)
        {
            result.X = Mathf.Min(result.X, MaximumSize.X);
            result.Y = Mathf.Min(result.Y, MaximumSize.Y);
        }

        return result;
    }

    private void ApplyStretchSettings(Vector2 targetSize)
    {
        Window rootWindow = GetTree().Root;

        // Set the base content size to our target resolution
        rootWindow.ContentScaleSize = new Vector2I((int)targetSize.X, (int)targetSize.Y);

        rootWindow.ContentScaleMode = StretchMode;
        rootWindow.ContentScaleAspect = StretchAspect;
    }


    // ─── Orientation Change Handler ───────────────────────────────────────────

    private void OnWindowSizeChanged()
    {
        ApplyViewportSize();
    }
}