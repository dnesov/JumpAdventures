extends RichTextLabel

export var translation_string = ""


# Called when the node enters the scene tree for the first time.
func _ready():
	bbcode_text = "[center]" + tr(translation_string)
	pass # Replace with function body.
