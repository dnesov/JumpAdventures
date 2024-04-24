tool
extends VisualScriptCustomNode

func _get_caption():
    return "My custom node"

func _get_category():
    return "Jump Adventures"

func _get_text():
    return ""

func _get_output_sequence_port_count():
    return 1


func _get_output_value_port_count():
    return 1