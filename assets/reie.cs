using MainGame;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace reie;

// Interfaces and Structs
public interface IDrawable
{
    void Draw(Engine engine);
}

// Helper Classes
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
        Rect = rect = rect;
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

public class DrawBatch
{
    public List<IDrawable> Elements;

    public DrawBatch()
    {
        Elements = new List<IDrawable>();
    }
    public void Add(IDrawable drawable)
    {
        Elements.Add(drawable);
    }
    public void DrawAll(Engine engine)
    {
        foreach (var element in Elements)
        {
            element.Draw(engine);
        }
    }
}

// Main Engine Class
public class Engine
{
    // Caches
    private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
    private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();
    private Dictionary<string, Music> _musics = new Dictionary<string, Music>();
    private Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();

    // Keep references to delegates so they aren't garbage collected
    private List<AudioCallback> _audioCallbacks = new List<AudioCallback>();

    // Properties
    public int ScreenWidth => Raylib.GetScreenWidth();
    public int ScreenHeight => Raylib.GetScreenHeight();

    // --- Resources ---
    public Texture2D UseTexture(string TextureName, string TexturePath)
    {
        if (!_textures.ContainsKey(TextureName))
            _textures.Add(TextureName, Raylib.LoadTexture(TexturePath));
        return _textures[TextureName];
    }

    public Music UseMusic(string MusicName, string MusicPath)
    {
        if (!_musics.ContainsKey(MusicName))
            _musics.Add(MusicName, Raylib.LoadMusicStream(MusicPath));
        return _musics[MusicName];
    }

    public Font UseFont(string FontName, string FontPath, int FontSize = 14)
    {
        if (!_fonts.ContainsKey(FontName))
            _fonts.Add(FontName, Raylib.LoadFontEx(FontPath, FontSize, null, 0));
        return _fonts[FontName];
    }

    public Shader UseShader(string ShaderName, string vsPath, string fsPath)
    {
        if (!_shaders.ContainsKey(ShaderName))
            _shaders.Add(ShaderName, Raylib.LoadShader(vsPath, fsPath));
        return _shaders[ShaderName];
    }

    // --- Drawing ---
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

    // --- Shaders ---
    public void BeginShader(string shaderName)
    {
        if (_shaders.TryGetValue(shaderName, out Shader shader))
            Raylib.BeginShaderMode(shader);
    }
    public void EndShader() => Raylib.EndShaderMode();

    public int GetShaderLocation(string shaderName, string uniformName)
    {
        return _shaders.TryGetValue(shaderName, out Shader shader)
            ? Raylib.GetShaderLocation(shader, uniformName) : -1;
    }

    public unsafe void SetShaderValue<T>(string shaderName, int loc, T value, ShaderUniformDataType type) where T : unmanaged
    {
        if (_shaders.TryGetValue(shaderName, out Shader shader))
            Raylib.SetShaderValue(shader, loc, value, type);
    }

    // --- Audio ---
    public void PlayMusic(string MusicName)
    {
        if (_musics.ContainsKey(MusicName))
            Raylib.PlayMusicStream(_musics[MusicName]);
    }

    public void SetMusicVolume(string MusicName, float volume)
    {
        if (_musics.TryGetValue(MusicName, out Music music))
            Raylib.SetMusicVolume(music, volume);
    }

    public float GetMusicTimePlayed(string MusicName)
    {
        return _musics.TryGetValue(MusicName, out Music music) ? Raylib.GetMusicTimePlayed(music) : 0f;
    }

    public void SeekMusic(string MusicName, float position)
    {
        if (_musics.TryGetValue(MusicName, out Music music))
            Raylib.SeekMusicStream(music, position);
    }

    public unsafe void AttachAudioProcessor(string musicName, AudioCallback callback)
    {
        if (_musics.TryGetValue(musicName, out Music music))
        {
            _audioCallbacks.Add(callback); // Prevent GC
            Raylib.AttachAudioStreamProcessor(music.Stream, callback);
        }
    }

    // --- Core Window & Input ---
    public void Window(int width, int height, string windowtitle = "REIE", ConfigFlags flag = ConfigFlags.ResizableWindow)
    {
        Raylib.SetConfigFlags(flag);
        Raylib.InitWindow(width, height, windowtitle);
        Raylib.InitAudioDevice();
    }
    public void SetFPS(int fps) => Raylib.SetTargetFPS(fps);
    public bool IsKeyPressed(KeyboardKey key) => Raylib.IsKeyPressed(key);
    public void HideCursor() => Raylib.HideCursor();

    public void UpdateMusic()
    {
        foreach (var pair in _musics)
        {
            if (Raylib.IsMusicStreamPlaying(pair.Value))
                Raylib.UpdateMusicStream(pair.Value);
        }
    }

    public void UnloadAll()
    {
        foreach (var pair in _textures) Raylib.UnloadTexture(pair.Value);
        foreach (var pair in _musics) Raylib.UnloadMusicStream(pair.Value);
        foreach (var pair in _shaders) Raylib.UnloadShader(pair.Value);
        foreach (var pair in _fonts) Raylib.UnloadFont(pair.Value);
        Raylib.CloseAudioDevice();
    }
}

public class Game
{
    public static void Main()
    {
        Engine engine = new Engine();
        CoreGame game = new CoreGame();

        // Let CoreGame initialize window dimensions/fullscreen
        game.Init(engine);

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