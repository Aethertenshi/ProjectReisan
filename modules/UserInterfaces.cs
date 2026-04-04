using Raylib_cs;
using System;

namespace reien;
public partial class Engine
{
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
