extends CanvasItem

var rope_segment = preload("res://Prefabs/Decoration/Rope/RopeSegment.tscn")
var segment_length: int = 5
var rope_segments = []
var rope_points: PoolVector2Array = []

export var rope_tolerance = 4.0
export var hanging: bool = false
export var rope_color: Color = Color.black
export var vertex_texture: Texture
export var thickness: float = 3.0

onready var start_anchor = $RopeStartAnchor
onready var end_anchor = $RopeEndAnchor

onready var start_anchor_joint = $RopeStartAnchor/Joint
onready var end_anchor_joint = $RopeEndAnchor/Joint

func _ready():
	populate(start_anchor.position, end_anchor.position)
	get_rope_points()
	pass
	
func _process(_delta):
	get_rope_points()
	if rope_points.size() != 0:
		update()

func create(amount: int, parent: Object, end: Vector2, spawn_angle: float):
	for i in amount:
		parent = add_segment(parent, spawn_angle)
		parent.set_name("segment_" + str(i))
		rope_segments.append(parent)
		
		var joint_pos = parent.get_node("Joint").global_position
		if joint_pos.distance_to(end) < rope_tolerance:
			break
	
	if !hanging:
		end_anchor.get_node("Joint").node_a = end_anchor.get_path()
		end_anchor.get_node("Joint").node_b = rope_segments[-1].get_path()

func populate(start: Vector2, end: Vector2):
	var d = start.distance_to(end)
	var segment_amount = round(d / segment_length)
	var spawn_angle = (end - start).angle() - PI / 2
	
	create(segment_amount, start_anchor, end, spawn_angle)
	pass
	
func add_segment(parent: Node, spawn_angle: float) -> Node2D:
	var j: PinJoint2D = parent.get_node("Joint") as PinJoint2D
	
	var segment: Node2D = rope_segment.instance() as Node2D
	segment.global_position = j.global_position
	segment.rotation = spawn_angle
	segment.rope_parent = self
	add_child(segment)
	
	j.node_a = parent.get_path()
	j.node_b = segment.get_path()
	
	return segment

func get_rope_points():
	rope_points = []
	rope_points.append(start_anchor.global_position)
	for i in rope_segments:
		rope_points.append(i.global_position)
	
	if !hanging:
		rope_points.append(end_anchor_joint.global_position)

func _draw():
	draw_polyline(rope_points, rope_color, thickness, true)
	pass
