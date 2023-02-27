using System.Collections.Generic;
using Godot;

public class MusicResource : Resource
{
    [Export] public List<AudioStream> Tracks;
}