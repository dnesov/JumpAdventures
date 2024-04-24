extends Particles2D

onready var notifier: VisibilityNotifier2D = $VisibilityNotifier2D


# Called when the node enters the scene tree for the first time.
func _ready():
	notifier.connect("screen_exited", self, "screen_exited")
	pass # Replace with function body.


func screen_exited():
	queue_free()
