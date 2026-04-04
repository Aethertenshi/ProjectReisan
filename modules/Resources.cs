using Raylib_cs;
using System;

namespace reien;
public partial class Engine
{
    // Caches
    private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
    private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();
    private Dictionary<string, Music> _musics = new Dictionary<string, Music>();
    private Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();

    // Functions
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
}
