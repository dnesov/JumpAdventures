extends Node

var max_length: int = 8
var input = ""

var capy = load("res://Prefabs/Capy.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func _input(event):
	if (event is InputEventKey and event.is_pressed()):
		input += OS.get_scancode_string(event.scancode)
		if (input.length() >= max_length):
			input = ""
		check_cheat(input)
	pass


func check_cheat(cheat: String):
	match cheat:
		"CAPY":
			spawn_capybara()
	pass

func spawn_capybara():
	input = ""
	var capy_scene = capy.instance()
	var screen_size = get_tree().root.get_viewport().size
	capy_scene.global_position = Vector2(rand_range(0, screen_size.x), rand_range(0, screen_size.y))
	add_child(capy_scene)
	pass
