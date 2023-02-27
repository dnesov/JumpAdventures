extends AnimationPlayer


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

export var animation_name: String;


# Called when the node enters the scene tree for the first time.
func _ready():
	play(animation_name)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
