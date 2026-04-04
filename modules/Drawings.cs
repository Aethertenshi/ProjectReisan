using Raylib_cs;
using System;
using System.Numerics;

namespace reien;
public partial class Engine
{
    // Functions
    public void DrawText(string text, int x, int y, int fontSize, Color color) => Raylib.DrawText(text, x, y, fontSize, color);

    public void DrawTextPro(string fontName, string text, Vector2 position, Vector2 origin, float rotation, float fontSize, float spacing, Color tint)
    {
        Font font = _fonts.ContainsKey(fontName) ? _fonts[fontName] : Raylib.GetFontDefault();
        Raylib.DrawTextPro(font, text, position, origin, rotation, fontSize, spacing, tint);
    }

    public Vector2 MeasureTextEx(string fontName, string text, float fontSize, float spacing)
    {
        Font font = _fonts.ContainsKey(fontName) ? _fonts[fontName] : Raylib.GetFontDefault();
        return Raylib.MeasureTextEx(font, text, fontSize, spacing);
    }

    public void ClearScreen(Color? color = null) => Raylib.ClearBackground(color ?? Color.Black);

    public void DrawRectangle(float x, float y, float width, float height, Color color) => Raylib.DrawRectangle((int)x, (int)y, (int)width, (int)height, color);

    public void DrawRing(Vector2 center, float innerRadius, float outerRadius, int startAngle, int endAngle, int segments, Color color) =>
        Raylib.DrawRing(center, innerRadius, outerRadius, startAngle, endAngle, segments, color);

    public void DrawTexturePro(string textureName, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color tint)
    {
        if (_textures.TryGetValue(textureName, out Texture2D texture))
            Raylib.DrawTexturePro(texture, source, dest, origin, rotation, tint);
    }

    public void DrawCover(string textureName, Rectangle destRec, Color? tint = null)
    {
        if (!_textures.TryGetValue(textureName, out Texture2D texture)) return;

        Color finalTint = tint ?? Color.White;
        float targetAspect = destRec.Width / destRec.Height;
        float imageAspect = (float)texture.Width / (float)texture.Height;

        Rectangle sourceRec = new Rectangle(0.0f, 0.0f, (float)texture.Width, (float)texture.Height);

        if (imageAspect > targetAspect)
        {
            sourceRec.Width = texture.Height * targetAspect;
            sourceRec.X = (texture.Width - sourceRec.Width) / 2.0f;
        }
        else
        {
            sourceRec.Height = texture.Width / targetAspect;
            sourceRec.Y = (texture.Height - sourceRec.Height) / 2.0f;
        }

        Vector2 origin = new Vector2(destRec.Width / 2.0f, destRec.Height / 2.0f);
        destRec.X += origin.X;
        destRec.Y += origin.Y;

        Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, 0.0f, finalTint);
    }

    public void DrawButton(Button btn)
    {
        Font font = _fonts.ContainsKey(btn.Font) ? _fonts[btn.Font] : Raylib.GetFontDefault();
        Vector2 mousePos = Raylib.GetMousePosition();
        bool isHovering = Raylib.CheckCollisionPointRec(mousePos, btn.Rect);
        bool isClicked = Raylib.IsMouseButtonPressed(MouseButton.Left) && isHovering;

        Color currentColor = isHovering ? btn.HoverColor : btn.BaseColor;
        Raylib.DrawRectangleRounded(btn.Rect, 0.2f, 8, currentColor);

        int textWidth = (int)Raylib.MeasureTextEx(font, btn.Text, btn.FontSize, 1).X;
        float textX = btn.Rect.X + (btn.Rect.Width - textWidth) / 2;
        float textY = btn.Rect.Y + (btn.Rect.Height - btn.FontSize) / 2;

        Raylib.DrawTextEx(font, btn.Text, new Vector2(textX, textY), btn.FontSize, 1, btn.TextColor);

        btn.isClicked = isClicked;
        btn.isHovered = isHovering;
    }
}
