extends CanvasLayer


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	var os = OS.get_name()
	if os != "Android":
		visible = false
	pass # Replace with function body.
