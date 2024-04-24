extends LinkButton


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
# warning-ignore:return_value_discarded
	connect("pressed", self, "on_pressed")
	pass # Replace with function body.

func on_pressed():
# warning-ignore:return_value_discarded
	OS.shell_open(ProjectSettings.globalize_path("user://"))
