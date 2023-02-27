using Godot;
using System.Collections.Generic;
using Jump.Entities;

public class ColorScheme : Resource
{
    // A lame workaround for the lack of support for enums in dictionaries.
    [Export] public Godot.Collections.Dictionary<int, Color> Colors;
}