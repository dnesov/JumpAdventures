extends Control

export var gradient: Gradient

onready var fps_label: Label = get_node("%FPS")
onready var frametime_label: Label = get_node("%FrameTime")

var optimal_fps: int = 60

var enabled: bool = false

func _ready():
	visible = enabled
	set_process(enabled)
	
func _input(event):
	if Input.is_action_just_pressed("debug_fps_toggle"):
		enabled = !enabled
		set_process(enabled)
		visible = enabled

func _process(_delta):
	var frametime = Performance.get_monitor(Performance.TIME_PROCESS) * 1000
	var fps = Engine.get_frames_per_second()
	
	fps_label.text = "%2.0f FPS" % fps
	
	frametime_label.text = "%2.2f ms" % frametime
	
	modulate = gradient.interpolate(fps / optimal_fps)
