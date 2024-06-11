extends PanelContainer

var changelog_loaded: bool = false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.
	
func _input(event):
	if event.is_action_pressed("ui_cancel") and visible:
		hide()

func show():
	visible = true
	if changelog_loaded: return
	get_node("%ChangelogLabel").bbcode_text = load_changelog_file()


func load_changelog_file() -> String:
	var file = File.new()
	file.open("res://changelog.txt", File.READ)
	var text = file.get_as_text()
	file.close()
	changelog_loaded = true
	return text

func hide():
	visible = false
