using Godot;
using System;
using System.Collections.Generic;
using ImGuiNET;

public class ImGuiNode : Node2D
{
    [Export]
    DynamicFont Font = null;

    [Signal]
    public delegate void IGLayout();

    public virtual void Init(ImGuiIOPtr io)
    {
#if DEBUG
        if (Font is null)
        {
            io.Fonts.AddFontDefault();
        }
        else
        {
            ImGuiGD.AddFont(Font);
        }
        ImGuiGD.RebuildFontAtlas();
#endif
    }

    public virtual void Layout()
    {
#if DEBUG
        EmitSignal(nameof(IGLayout));
#endif
    }

    public override void _EnterTree()
    {
#if DEBUG
        ImGuiGD.Init(GetViewport());
        Init(ImGui.GetIO());
#endif
    }

    public override void _Process(float delta)
    {
#if DEBUG
        if (Visible)
        {
            ImGuiGD.Update(delta, GetViewport());
            Layout();
            ImGuiGD.Render(GetCanvasItem());
        }
#endif
    }

    public override void _Input(InputEvent evt)
    {
#if DEBUG
        if (Visible && ImGuiGD.ProcessInput(evt))
        {
            GetTree().SetInputAsHandled();
        }
#endif
    }

    public override void _ExitTree()
    {
#if DEBUG
        ImGuiGD.Shutdown();
#endif
    }
}
