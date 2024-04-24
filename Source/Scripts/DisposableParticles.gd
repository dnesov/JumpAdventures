extends Particles2D

var timer: Timer

# Called when the node enters the scene tree for the first time.
func _ready():
	timer = Timer.new()
	timer.wait_time = lifetime
# warning-ignore:return_value_discarded
	timer.connect("timeout", self, "queue_free")
	add_child(timer)
	timer.start()
	
	restart()
	
	for particle in get_children():
		if particle is Particles2D:
			particle.restart()
		pass
	pass
