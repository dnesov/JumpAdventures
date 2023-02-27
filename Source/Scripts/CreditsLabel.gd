extends RichTextLabel

func _ready():
	connect("meta_clicked", self, "meta_clicked")
	var file = File.new()
	file.open("res://credits.txt", File.READ)
	bbcode_text = file.get_as_text()
	file.close()
	pass 

func meta_clicked(meta):
	OS.shell_open(str(meta))
