extends RichTextLabel

func _ready():
# warning-ignore:return_value_discarded
	connect("meta_clicked", self, "meta_clicked")
	var file = File.new()
	file.open("res://credits.txt", File.READ)
	bbcode_text = file.get_as_text()
	file.close()
	pass 

func meta_clicked(meta):
# warning-ignore:return_value_discarded
	OS.shell_open(str(meta))
