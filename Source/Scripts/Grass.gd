extends Sprite

export var follow_target_path: NodePath
export var distance_threshold: float = 20
export var move_speed_multiplier: float = 3.0
export var max_target_velocity: float = 100

var follow_target: Node2D
var grass_material: ShaderMaterial = material as ShaderMaterial
var prev_target_pos: Vector2

# Called when the node enters the scene tree for the first time.
func _ready():
#	follow_target = get_node(follow_target_path) as Node2D
	pass # Replace with function body.

func _physics_process(delta):
#	var dist = global_position.distance_to(follow_target.global_position)
#
#	if(dist < distance_threshold):
#		apply_sway()
#		pass
#
#	prev_target_pos = follow_target.global_position
	pass

func apply_sway():
	print("sway")
	grass_material.set_shader_param("interval", 0.1)
	print(grass_material.get_shader_param("interval"))
	pass


func screen_entered():
	visible = true
	pass # Replace with function body.


func screen_exited():
	visible = false
	pass # Replace with function body.
