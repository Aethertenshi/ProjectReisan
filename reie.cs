using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace reien;

// Interfaces and Structs
public interface IDrawable
{
    void Draw(Engine engine);
    void Update(Engine engine, float dt);
}
public interface IGameRunner
{
    void Init(Engine engine);
    void Update(Engine engine, float deltaTime);
    void Draw(Engine engine);
}

// Helper Classes
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
            element.Update(engine, Raylib.GetFrameTime());
        }
    }
}

// Main Engine Class
public partial class Engine
{
    // Cache
    private Stack<Vector2> _translationStack = new Stack<Vector2>();
    private Vector2 _currentTranslation = Vector2.Zero;
    private Rectangle? _currentScissor = null;

    // Scissor
    public void SetScissorRectangle(Rectangle rect)
    {
        // Raylib has native hardware clipping. It expects integers for screen coordinates.
        Raylib.BeginScissorMode((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
    }
    public void ClearScissorRectangle()
    {
        // Disables the clipping region
        Raylib.EndScissorMode();
    }
    public void PushTranslation(float x, float y)
    {
        _translationStack.Push(_currentTranslation);
        _currentTranslation += new Vector2(x, y);
    }

    public void PopTranslation()
    {
        if (_translationStack.Count > 0)
        {
            _currentTranslation = _translationStack.Pop();
        }
    }
    public void DrawScissorRectangle(float x, float y, float width, float height, Color color)
    {
        // 1. Add the current translation offset to the drawing coordinates
        float finalX = x + _currentTranslation.X;
        float finalY = y + _currentTranslation.Y;

        // 2. Pass the translated coordinates to Raylib
        Raylib.DrawRectangle((int)finalX, (int)finalY, (int)width, (int)height, color);
    }


    // Properties
    public int ScreenWidth => Raylib.GetScreenWidth();
    public int ScreenHeight => Raylib.GetScreenHeight();

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

public class ReiEngine
{
    private IGameRunner _runner;
    public ReiEngine(IGameRunner runner)
    {
        _runner = runner;
    }
    public void Run()
    {
        Engine engine = new Engine();
        _runner.Init(engine);
        while (!Raylib.WindowShouldClose())
        {
            engine.UpdateMusic();
            _runner.Update(engine, Raylib.GetFrameTime());
            Raylib.BeginDrawing();
            _runner.Draw(engine);
            Raylib.EndDrawing();
        }
        engine.UnloadAll();
        Raylib.CloseWindow();
    }
}