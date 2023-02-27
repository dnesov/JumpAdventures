extends Label


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


func _process(_delta):
	text = "FPS: " + str(Engine.get_frames_per_second())
	pass
