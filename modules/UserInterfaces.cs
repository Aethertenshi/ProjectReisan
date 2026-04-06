using Raylib_cs;
using System;

namespace reien;
public partial class Engine
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
            // 1. Fetch the input directly from Raylib
            float mouseScrollDelta = Raylib.GetMouseWheelMove();

            // Optional check: You may want to add logic here to ensure the mouse 
            // is actually hovering over 'Rect' before allowing it to scroll!
            if (mouseScrollDelta == 0) return;

            // 2. Apply the scroll
            ScrollPosition -= mouseScrollDelta * ScrollSpeed;

            // 3. Clamp the scroll position
            float maxScroll = Math.Max(0, ContentHeight - Rect.Height);
            ScrollPosition = Math.Clamp(ScrollPosition, 0, maxScroll);

            // 4. Propagate the Update call to children (if your children also need to update over time)
            foreach (var child in Children)
            {
                child.Update(engine, dt);
            }
        }

        // Correctly implements IDrawable.Draw
        public void Draw(Engine engine)
        {
            engine.DrawRectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height, BackgroundColor);

            engine.SetScissorRectangle(Rect);
            engine.PushTranslation(0, -ScrollPosition);

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

        public Button(DrawBatch batch, Action<Button>? onAction, Rectangle rect, string text, int? fontsize = null, string? font = null, Color? baseColor = null, Color? hoverColor = null, Color? textColor = null)
        {
            Rect = rect;
            Text = text;
            BaseColor = baseColor ?? Color.Gray;
            HoverColor = hoverColor ?? Color.LightGray;
            TextColor = textColor ?? Color.White;
            Font = font ?? "DefaultFont";
            FontSize = fontsize ?? 20;
            isAction = onAction;

            batch.Add(this);
        }

        public void Update(Engine engine, float dt)
        {
        }
        public void Draw(Engine engine)
        {
            engine.DrawButton(this);
            if (isHovered && isAction != null && isClicked)
            {
                isAction?.Invoke(this);
            }
        }
    }
}
