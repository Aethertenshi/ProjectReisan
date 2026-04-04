using Raylib_cs;
using System;

namespace reien;
public partial class Engine
{
    // Functions
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
}
