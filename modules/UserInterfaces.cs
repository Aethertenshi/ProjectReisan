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
        public bool ClipChildren;
        public ScrollContainer(
            DrawBatch? batch, 
            Rectangle rect, 
            List<IDrawable>? children = null, 
            Color? backgroundColor = null, 
            float scrollSpeed = 20.0f, 
            Vector2 padding = new Vector2(), 
            bool clipChildren = false)
        {
            Rect = rect;
            BackgroundColor = backgroundColor ?? Color.DarkGray;
            Children = children ?? new List<IDrawable>();
            ScrollSpeed = scrollSpeed;
            Padding = padding;
            ClipChildren = clipChildren;
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

                    if (ClipChildren) Raylib.BeginScissorMode((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height);

                    modifiedRect.Y = currentContentHeight + visualScrollOffset;
                    modifiedRect.X = currentContentWidth;
                    child.Rect = modifiedRect;
                    child.Draw(engine);
                    child.Update(engine, dt);
                    child.Rect = originalRect;

                    if (ClipChildren) Raylib.EndScissorMode();

                    currentContentHeight += (int)(child.Rect.Height + Padding.Y);
                }
            }
        }
        public void Draw(Engine engine)
        {
            Raylib.DrawRectangle((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height, BackgroundColor);
        }
    }
    public class ScrollCarrousel : IDrawable
    {
        public Rectangle Rect { get; set; }
        public List<IDrawable> Children { get; set; }
        public Color BackgroundColor { get; set; }
        public Vector2 Padding { get; set; }
        public float ScrollSpeed { get; set; }
        public float CurveMagnitude { get; set; }
        public bool ClipChildren { get; set; }

        private float _scroll;
        private float _targetScroll;
        private Vector2 _lastMouse;

        public ScrollCarrousel(
            DrawBatch? batch,
            Rectangle rect,
            List<IDrawable>? children = null,
            Color? backgroundColor = null,
            Vector2 padding = new(),
            bool clipChildren = false,
            float scrollSpeed = 20f,
            float curveMagnitude = 200f)
        {
            Rect = rect;
            Children = children ?? [];
            BackgroundColor = backgroundColor ?? Color.DarkGray;
            Padding = padding;
            ClipChildren = clipChildren;
            ScrollSpeed = scrollSpeed;
            CurveMagnitude = curveMagnitude;

            batch?.Add(this);
        }

        public void Update(Engine engine, float dt)
        {
            HandleInput();

            _scroll = Raymath.Lerp(_scroll, _targetScroll, 8f * dt);
            if (MathF.Abs(_scroll - _targetScroll) < 0.1f) _scroll = _targetScroll;

            Layout();

            foreach (var child in Children)
                child.Update(engine, dt);
        }

        public void Draw(Engine engine)
        {
            Raylib.DrawRectangle((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height, BackgroundColor);

            if (ClipChildren) Raylib.BeginScissorMode((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height);

            foreach (var child in Children)
                child.Draw(engine);

            if (ClipChildren) Raylib.EndScissorMode();
        }

        // -------------------------------------------------------------------------

        private void HandleInput()
        {
            var mouse = Raylib.GetMousePosition();

            if (Raylib.CheckCollisionPointRec(mouse, Rect))
            {
                _targetScroll += Raylib.GetMouseWheelMove() * ScrollSpeed;

                if (Raylib.IsMouseButtonDown(MouseButton.Left))
                    _targetScroll += mouse.Y - _lastMouse.Y;
            }

            _lastMouse = mouse;
        }

        private void Layout()
        {
            float centerY = Rect.Y + Rect.Height * 0.5f;
            float cursorY = Rect.Y + Padding.Y + _scroll;
            float screenW = Raylib.GetScreenWidth();

            foreach (var child in Children)
            {
                var r = child.Rect;
                r.Y = cursorY;

                // Normalised distance from centre [0 = centre, 1 = carousel edge, >1 = beyond].
                float t = MathF.Abs((r.Y + r.Height * 0.5f) - centerY) / (Rect.Height * 0.5f);

                // Quadratic falloff: 1 at centre, 0 at edge.
                float curve = MathF.Pow(Math.Clamp(1f - t, 0f, 1f), 2f);

                // Centre button sits at Rect.X (left edge of the carousel).
                // Farther buttons slide right by however much curve they have lost.
                r.X = ((Rect.X + Rect.Width) - r.Width) + CurveMagnitude * (1f - curve);

                // Never push past the right edge of the screen.
                //r.X = Math.Min(r.X, screenW - r.Width);

                child.Rect = r;
                cursorY += r.Height + Padding.Y;
            }
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

        public Button(
            DrawBatch? batch, 
            Action<Button>? onAction, 
            Rectangle rect, 
            string text, 
            int? fontsize = null, 
            string? font = null, 
            Color? baseColor = null, 
            Color? hoverColor = null, 
            Color? textColor = null)
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
    public class ImageButton : IDrawable
    {
        public Rectangle Rect { get; set; }
        public string TextureName;
        public Color BaseTint;
        public Color HoverTint;

        public bool isClicked;
        public bool isHovered;
        public Action<ImageButton>? isAction;

        public ImageButton(
            DrawBatch? batch,
            Action<ImageButton>? onAction,
            Rectangle rect,
            string textureName,
            Color? baseTint = null,
            Color? hoverTint = null)
        {
            Rect = rect;
            TextureName = textureName;
            BaseTint = baseTint ?? Color.Gray;
            HoverTint = hoverTint ?? Color.White;
            isAction = onAction;

            if (batch != null)
            {
                batch.Add(this);
            }
        }

        public void Update(Engine engine, float dt)
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            isHovered = Raylib.CheckCollisionPointRec(mousePos, Rect);
            isClicked = Raylib.IsMouseButtonPressed(MouseButton.Left) && isHovered;

            if (isClicked && isAction != null)
            {
                isAction?.Invoke(this);
            }
        }

        public void Draw(Engine engine)
        {
            Color currentTint = isHovered ? HoverTint : BaseTint;

            // 1. Background Image (Cropped)
            // Your existing DrawCover method already implements the precise DrawTexturePro math
            // needed to preserve the width and crop the excess height based on aspect ratios!
            engine.DrawCover(TextureName, Rect, currentTint);

            // 2. Dim Overlay Gradient (From Left to Right)
            // This creates the black fading shadow on the left side, mimicking the beatmap button.
            int dimWidth = (int)(Rect.Width * 0.7f); // Dim extends 70% across the button
            Raylib.DrawRectangleGradientH(
                (int)Rect.X,
                (int)Rect.Y,
                dimWidth,
                (int)Rect.Height,
                new Color(0, 0, 0, 220), // Dark shadow on the left edge
                new Color(0, 0, 0, 0)    // Fades completely to transparent
            );

            // 3. Left Edge Accent (The purple/blue bar indicating status/difficulty)
            int accentWidth = 26;
            Color accentColor = new Color(104, 114, 224, 255); // Blue-purple color from your image
            Raylib.DrawRectangle((int)Rect.X, (int)Rect.Y, accentWidth, (int)Rect.Height, accentColor);

            // 4. Circle indicator on the accent bar
            int circleRadius = 6;
            Vector2 circlePos = new Vector2(Rect.X + (accentWidth / 2f), Rect.Y + (Rect.Height / 2f));

            Raylib.DrawCircleV(circlePos, circleRadius, new Color(255, 240, 100, 255)); // Yellow inner circle
            Raylib.DrawCircleLines((int)circlePos.X, (int)circlePos.Y, circleRadius, Color.White); // White outline

            // 5. Hover Overlay
            // Adds a subtle white flash over the entire button when hovered
            if (isHovered)
            {
                Raylib.DrawRectangle((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height, new Color(255, 255, 255, 30));
            }
        }
    }
}