using Raylib_cs;
using System;
using System.Diagnostics;
using System.Numerics;

namespace reien.UI
{
    public class ScrollingFrame : IDrawable
    {
        public Rectangle Rect;
        public Color BackgroundColor;
        public Color ScrollbarColor;
        public float ScrollPosition;
        public float ScrollSpeed;

        public List<IDrawable> Children { get; private set; }
        public float ContentHeight { get; set; } = 0f;

        public ScrollingFrame(DrawBatch batch, Rectangle rect, Color? backgroundColor = null, Color? scrollbarColor = null, float scrollSpeed = 20f, List<IDrawable>? children = null)
        {
            Rect = rect;
            BackgroundColor = backgroundColor ?? Color.DarkGray;
            ScrollbarColor = scrollbarColor ?? Color.LightGray;
            ScrollSpeed = scrollSpeed;
            Children = children ?? new List<IDrawable>();

            batch.Add(this);
        }

        public void Update(Engine engine, float dt)
        {
            float mouseScrollDelta = Raylib.GetMouseWheelMove();
            Vector2 mouse = Raylib.GetMousePosition();
            // Only scroll if mouse is over the frame
            if (mouse.X >= Rect.X && mouse.X <= Rect.X + Rect.Width && mouse.Y >= Rect.Y && mouse.Y <= Rect.Y + Rect.Height)
            {
                if (mouseScrollDelta != 0)
                {
                    ScrollPosition -= mouseScrollDelta * ScrollSpeed;
                    float maxScroll = Math.Max(0, ContentHeight - Rect.Height);
                    ScrollPosition = Math.Clamp(ScrollPosition, 0, maxScroll);
                }
            }
            // Propagate Update to children, but adjust mouse position for hit detection
            foreach (var child in Children)
            {
                // If child is a Button, update its hitbox for offset
                if (child is Button btn)
                {
                    // Save original rect
                    var origRect = btn.Rect;
                    btn.Rect = new Rectangle(
                        Rect.X + btn.Rect.X,
                        Rect.Y + btn.Rect.Y - ScrollPosition,
                        btn.Rect.Width,
                        btn.Rect.Height
                    );
                    btn.Update(engine, dt);
                    btn.Draw(engine);
                    btn.Rect = origRect; // Restore
                }
                else
                {
                    child.Update(engine, dt);
                    child.Draw(engine);
                }
            }
        }

        // Correctly implements IDrawable.Draw
        public void Draw(Engine engine)
        {
            engine.DrawRectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height, BackgroundColor);

            engine.SetScissorRectangle(Rect);
            engine.PushTranslation(Rect.X, Rect.Y - ScrollPosition);

            foreach (var child in Children)
            {
                child.Draw(engine);
            }

            engine.PopTranslation();
            engine.ClearScissorRectangle();

            DrawScrollbar(engine);
        }

        private void DrawScrollbar(Engine engine)
        {
            if (ContentHeight <= Rect.Height) return;

            float scrollbarWidth = 10f;
            float viewableRatio = Rect.Height / ContentHeight;
            float scrollbarThumbHeight = Rect.Height * viewableRatio;

            float scrollTrackSpace = ContentHeight - Rect.Height;
            float scrollProgress = ScrollPosition / scrollTrackSpace;
            float scrollbarY = Rect.Y + (scrollProgress * (Rect.Height - scrollbarThumbHeight));

            engine.DrawRectangle(
                Rect.X + Rect.Width - scrollbarWidth,
                scrollbarY,
                scrollbarWidth,
                scrollbarThumbHeight,
                ScrollbarColor
            );
        }
    }
    public class Button : IDrawable
    {
        public Rectangle Rect;
        public string Text;
        public string Font;
        public Color BaseColor;
        public Color HoverColor;
        public Color TextColor;
        public int FontSize;

        public bool isClicked;
        public bool isHovered;
        public Action<Button>? isAction;

        public Button(DrawBatch? batch, Action<Button>? onAction, Rectangle rect, string text, int? fontsize = null, string? font = null, Color? baseColor = null, Color? hoverColor = null, Color? textColor = null)
        {
            Rect = rect;
            Text = text;
            BaseColor = baseColor ?? Color.Gray;
            HoverColor = hoverColor ?? Color.LightGray;
            TextColor = textColor ?? Color.White;
            Font = font ?? "DefaultFont";
            FontSize = fontsize ?? 20;
            isAction = onAction;

            if (batch != null)
            {
                batch.Add(this);
            }
        }

        public void Update(Engine engine, float dt)
        {
            if (isHovered && isAction != null)
            {
                isAction?.Invoke(this);
            }
        }
        public void Draw(Engine engine)
        {
            engine.DrawButton(this);
        }
    }
}