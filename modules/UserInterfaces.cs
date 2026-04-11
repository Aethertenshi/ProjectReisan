using Raylib_cs;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Numerics;

namespace reien.UI
{
    public class ScrollContainer : IDrawable
    {
        private float visualScrollOffset = 0;
        private float targetScrollOffset = 0;
        private int currentContentHeight = 0;
        private int currentContentWidth = 0;
        private Vector2 lastPosition = Vector2.Zero;

        public Rectangle Rect { get; set; }
        public Color BackgroundColor;
        public List<IDrawable> Children;
        public float ScrollSpeed;
        public Vector2 Padding;
        public ScrollContainer(DrawBatch? batch, Rectangle rect, List<IDrawable>? children = null, Color? backgroundColor = null, float scrollSpeed = 20.0f, Vector2 padding = new Vector2())
        {
            Rect = rect;
            BackgroundColor = backgroundColor ?? Color.DarkGray;
            Children = children ?? new List<IDrawable>();
            ScrollSpeed = scrollSpeed;
            Padding = padding;
            if (batch != null)
            {
                batch.Add(this);
            }
        }

        public void Update(Engine engine, float dt)
        {
            bool isPressing = Raylib.IsMouseButtonDown(MouseButton.Left);
            float isHovering = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), Rect);
            float scrollDelta = Raylib.GetMouseWheelMove();
            
            if (isHovering > 0 && scrollDelta != 0) targetScrollOffset -= scrollDelta * ScrollSpeed;
            if (isPressing && isHovering > 0) targetScrollOffset += (Raylib.GetMousePosition().Y - lastPosition.Y);
            lastPosition = Raylib.GetMousePosition();

            // 0.1f is the 'tightness' - lower is smoother/slower, higher is snappier
            visualScrollOffset = Raymath.Lerp(visualScrollOffset, targetScrollOffset, 8f * dt);
            currentContentHeight = (int)Padding.Y + (int)Rect.Y;
            currentContentWidth = (int)Padding.X + (int)Rect.X;

            if (Math.Abs(visualScrollOffset - targetScrollOffset) < 0.1f) visualScrollOffset = targetScrollOffset;

            foreach (IDrawable child in Children)
            {
                if (child.Rect.X > 0)
                {
                    Rectangle modifiedRect = child.Rect;
                    Rectangle originalRect = child.Rect;

                    modifiedRect.Y = currentContentHeight + visualScrollOffset;
                    modifiedRect.X = currentContentWidth;
                    child.Rect = modifiedRect;
                    child.Draw(engine);
                    child.Update(engine, dt);
                    child.Rect = originalRect;

                    currentContentHeight += (int)(child.Rect.Height + Padding.Y);
                }
            }
        }
        public void Draw(Engine engine)
        {
            Raylib.DrawRectangle((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height, BackgroundColor);
        }
    }
    public class Button : IDrawable
    {
        public Rectangle Rect { get; set; }
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