using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace reien;

// Interfaces and Structs
public interface IDrawable
{
    void Draw(Engine engine);
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
        }
    }
}

// Main Engine Class
public partial class Engine
{
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