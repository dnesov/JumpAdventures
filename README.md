![Jump Adventures Header](./media/repo_header.png)

# Jump Adventures
This is a repository containing source code and other scripts used in [Jump Adventures](https://store.steampowered.com/app/2221130/Jump_Adventures/), excluding art assets and binaries that cannot be licensed under BSD-3.

## Purpose
- Learning;
- Reference for reverse engineers/decompilation (modloaders, hacks, you name it);
- Datamining;
- Anything else as long as it follows the [license](LICENSE).

## Building
At the present time, the project heavily depends on native Steam and FMOD binaries. I'll be moving the implementations for these to separate ones in the future, as well as documenting the process of building the code for this repo.

For now, its more useful as a reference for someone wanting to see how games work, helper for decompilers/reverse engineers or anything as long as it follows BSD-3.

## Libraries used
- [YamlDotNet](https://github.com/aaubry/YamlDotNet)
- [ImGui.NET](https://github.com/mellinoe/ImGui.NET)
- [imgui-godot](https://github.com/pkdawson/imgui-godot)
- [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET)