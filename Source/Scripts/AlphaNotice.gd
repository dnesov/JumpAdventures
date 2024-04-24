extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	var data = get_node("root/GlobalData")
	print(data.get("_acceptedAlphaNotice"))
	pass # Replace with function body.

func hide():
	get_node("root/GlobalData").set("_acceptedAlphaNotice", true)
	visible = false
	pass
