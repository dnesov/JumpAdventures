extends CanvasLayer

signal on_closed

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func show():
	var player = get_node("AnimationPlayer") as AnimationPlayer
	player.play("show")
func hide():
	var player = get_node("AnimationPlayer") as AnimationPlayer
	player.play_backwards("show")
	emit_signal("on_closed")
