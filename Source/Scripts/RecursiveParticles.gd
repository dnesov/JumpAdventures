extends Particles2D

func _ready():
	restart()
	
	for particle in get_children():
		if particle is Particles2D:
			particle.restart()
		pass
	pass
