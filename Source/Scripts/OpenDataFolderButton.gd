extends LinkButton


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	connect("pressed", self, "on_pressed")
	pass # Replace with function body.

func on_pressed():
	OS.shell_open(ProjectSettings.globalize_path("user://"))
