extends Node

var window_sizes = [
	Vector2(320, 180),
	Vector2(426, 240),
	Vector2(640, 360),
	Vector2(848, 480),
	Vector2(854, 480),
	Vector2(960, 540),
	Vector2(1024, 576),
	Vector2(1280, 720),
	Vector2(1366, 768),
	Vector2(1600, 900),
	Vector2(1920, 1080),
]

const default_window_size = 7

func _ready():
	update_resolution_button()
	pass # Replace with function body.
	
func update_resolution_button():
	var button = get_node("%ResolutionOptionButton") as OptionButton
	button.items.clear()
	
	for size in window_sizes:
		button.add_item(str(size.x) + "x" + str(size.y))
	
	button.select(default_window_size)

func toggle_fullscreen(toggled: bool):
	OS.window_fullscreen = toggled
	pass # Replace with function body.
	
func change_window_size(size: int):
	OS.window_size = window_sizes[size]
	pass


func toggle_vsync(toggled: bool):
	OS.vsync_enabled = toggled
	pass # Replace with function body.


func open_worldpack_folder():
	OS.shell_open(OS.get_user_data_dir() + "/worldpacks")
	pass # Replace with function body.
