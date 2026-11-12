using Godot;

public partial class SafeArea : Control
{
    public override void _Ready()
    {
        ApplySafeArea();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMSizeChanged ||
            what == NotificationWMWindowFocusIn)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        // Physical screen safe area (in raw pixels)
        Rect2 safeArea = DisplayServer.GetDisplaySafeArea();

        // Physical screen full rect (in raw pixels)
        Rect2I screenRect = DisplayServer.ScreenGetUsableRect();

        // Guard: avoid divide-by-zero on startup
        if (screenRect.Size.X == 0 || screenRect.Size.Y == 0)
            return;

        // ── The Fix ──────────────────────────────────────────────────────────
        // DisplayServer returns RAW pixel values, but Godot anchors work in
        // VIEWPORT space (after stretch/scale is applied).
        // We must map the safe area into viewport space first.

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;

        float scaleX = viewportSize.X / screenRect.Size.X;
        float scaleY = viewportSize.Y / screenRect.Size.Y;

        // Safe area in viewport space
        float safeLeft = (safeArea.Position.X - screenRect.Position.X) * scaleX;
        float safeTop = (safeArea.Position.Y - screenRect.Position.Y) * scaleY;
        float safeRight = viewportSize.X - (safeArea.End.X - screenRect.Position.X) * scaleX;
        float safeBottom = viewportSize.Y - (safeArea.End.Y - screenRect.Position.Y) * scaleY;

        // Convert to 0–1 anchor ratios
        AnchorLeft = safeLeft / viewportSize.X;
        AnchorTop = safeTop / viewportSize.Y;
        AnchorRight = 1.0f - (safeRight / viewportSize.X);
        AnchorBottom = 1.0f - (safeBottom / viewportSize.Y);

        // Clear any pixel offsets
        OffsetLeft = 0;
        OffsetTop = 0;
        OffsetRight = 0;
        OffsetBottom = 0;

       
    }
}