using MainGame;
using Raylib_cs;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;

namespace reie;

public struct Button
{
    public Rectangle Rect;
    public string Text;
    public string Font;
    public Color BaseColor;
    public Color HoverColor;
    public Color TextColor;
    public int FontSize;

    public bool isHovering;
    public bool isClicked;

    public Button(Rectangle rect, string text, int? fontsize = null, string? font = null, Color? baseColor = null, Color? hoverColor = null, Color? textColor = null)
    {
        Rect = rect;
        Text = text;
        BaseColor = baseColor ?? Color.Gray;
        HoverColor = hoverColor ?? Color.LightGray;
        TextColor = textColor ?? Color.White;
        Font = font ?? "DefaultFont";
        FontSize = fontsize ?? 20;
    }
}

public class Engine {
    // Variables
    private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
    private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();
    private Dictionary<string, Music> _musics = new Dictionary<string, Music>();
    private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();

    // Importing Data
    public Texture2D UseTexture(string TextureName, string TexturePath)
    {
        Texture2D tex = Raylib.LoadTexture(TexturePath);

        if (!_textures.ContainsKey(TextureName))
        {
            _textures.Add(TextureName, tex);
        }

        return tex;
    }
    public Music UseMusic(string MusicName, string MusicPath)
    {
        Music msc = Raylib.LoadMusicStream(MusicPath);

        if (!_musics.ContainsKey(MusicName))
        {
            _musics.Add(MusicName, msc);
        }
        return msc;
    }
    public Font UseFont(string FontName, string FontPath, int FontSize = 14)
    {
        Font fnt = Raylib.LoadFontEx(FontPath, FontSize, null, 0);
        if (!_fonts.ContainsKey(FontName))
        {
            _fonts.Add(FontName, fnt);
        }
        return fnt;
    }

    // Drawing Functions
    public void DrawText(string text, int x, int y, int fontSize, Color color) => Raylib.DrawText(text, x, y, fontSize, color);
    public void ClearScreen(Color? color = null) => Raylib.ClearBackground(color ?? Color.Black);
    public void DrawCover(string textureName, Rectangle destRec, Color? tint = null)
    {
        if (!_textures.ContainsKey(textureName)) return;

        Color finalTint = tint ?? Color.White;
        Texture2D texture = _textures[textureName];

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
    public void DrawButton(ref Button btn)
    {
        Font font = _fonts.ContainsKey(btn.Font) ? _fonts[btn.Font] : Raylib.GetFontDefault();
        Vector2 mousePos = Raylib.GetMousePosition();
        bool isHovering = Raylib.CheckCollisionPointRec(mousePos, btn.Rect);
        bool isClicked = isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left);

        Debug.WriteLine(font);

        Color currentColor = isHovering ? btn.HoverColor : btn.BaseColor;

        Raylib.DrawRectangleRounded(btn.Rect, 0.2f, 8, currentColor);

        int fontSize = btn.FontSize;
        int textWidth = Raylib.MeasureText(btn.Text, fontSize);

        float textX = btn.Rect.X + (btn.Rect.Width - textWidth) / 2;
        float textY = btn.Rect.Y + (btn.Rect.Height - fontSize) / 2;

        //Raylib.DrawText(btn.Text, (int)textX, (int)textY, fontSize, btn.TextColor);
        Raylib.DrawTextEx(font, btn.Text, new Vector2(textX, textY), fontSize, 0, btn.TextColor);

        btn.isHovering = isHovering;
        btn.isClicked = isClicked;
    }

    // Main Game Functions
    public void Window(int width, int height, string windowtitle = "REIE", ConfigFlags flag = ConfigFlags.ResizableWindow)
    {
        Raylib.SetConfigFlags(flag);
        Raylib.InitWindow(width, height, windowtitle);
    }
    public void SetFPS(int fps) => Raylib.SetTargetFPS(fps);
    public void PlayMusic(string MusicName)
    {
        if (_musics.ContainsKey(MusicName))
        {
            Raylib.PlayMusicStream(_musics[MusicName]);
        }
    }

    // Helper Functions
    public void UpdateMusic()
    {
        foreach (var pair in _musics)
        {
            if (Raylib.IsMusicStreamPlaying(pair.Value))
            {
                Raylib.UpdateMusicStream(pair.Value);
            }
        }
    }
    public void UnloadAll()
    {
        foreach (var pair in _textures)
        {
            Raylib.UnloadTexture(pair.Value);
        }
        foreach (var pair in _musics)
        {
            Raylib.UnloadMusicStream(pair.Value);
        }
    }
}
public class Game {
    public static void Main()
    {
        // Main Functions
        Engine engine = new Engine();
        CoreGame game = new CoreGame();
        game.Init(engine);

        // Update Functions
        while (!Raylib.WindowShouldClose())
        {
            engine.UpdateMusic();
            game.Update(engine, Raylib.GetFrameTime());

            Raylib.BeginDrawing();
            game.Draw(engine);
            Raylib.EndDrawing();
        }

        engine.UnloadAll();
        Raylib.CloseWindow();
    }
}
