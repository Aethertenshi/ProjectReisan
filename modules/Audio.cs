using Raylib_cs;
using System;

namespace reien;
public partial class Engine
{
    // Caches
    private List<AudioCallback<float>> _audioCallbacks = new List<AudioCallback<float>>();

    // Functions
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

    public unsafe void AttachAudioProcessor(string musicName, delegate* unmanaged[Cdecl]<void*, uint, void> callback)
    {
        if (_musics.TryGetValue(musicName, out Music music))
        {
            Raylib.AttachAudioStreamProcessor(music.Stream, callback);
        }
    }
}
