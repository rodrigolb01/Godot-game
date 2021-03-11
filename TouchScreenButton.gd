extends TouchScreenButton

# Gonkee's joystick script for Godot 3 - full tutorial https://youtu.be/uGyEP2LUFPg
# If you use this script, I would prefer if you gave credit to me and my channel

# Change these based on the size of your button and outer sprite
signal shoot;
signal aim;
signal aimRelease;
var radius = Vector2(72, 72)
var boundary = 144
var boundary_fire = 120;
var ongoing_drag = -1

var return_accel = 20

var threshold = 10

onready var shoot_area = get_node("../shoot_area");	

func _ready():
	shoot_area.hide();
func _process(delta):
	if ongoing_drag == -1:
		var pos_difference = (Vector2(0, 0) - radius) - position
		position += pos_difference * return_accel * delta

func get_button_pos():
	return position + radius

func _input(event):
	if event is InputEventScreenDrag or (event is InputEventScreenTouch and event.is_pressed()):
		var event_dist_from_centre = (event.position - get_parent().global_position).length()

		if event_dist_from_centre <= boundary * global_scale.x or event.get_index() == ongoing_drag:
			set_global_position(event.position - radius * global_scale)

			if get_button_pos().length() > boundary:
				set_position( get_button_pos().normalized() * boundary - radius)
			if get_button_pos().length() >= threshold:
				emit_signal("aim");
			if get_button_pos().length() < boundary_fire:
				emit_signal("aimRelease");

			if event_dist_from_centre >= boundary_fire:
				emit_signal("shoot");
				if(!shoot_area.visible):
					shoot_area.show();
			else:
				if(shoot_area.visible):
					shoot_area.hide();
			ongoing_drag = event.get_index()
	if get_button_pos().length() >= threshold:
				emit_signal("aim");
	if get_button_pos().length() < boundary_fire:
				emit_signal("aimRelease");
		
	
	if event is InputEventScreenTouch and !event.is_pressed() and event.get_index() == ongoing_drag:
		ongoing_drag = -1;
		shoot_area.hide();
		

		
func get_value():
	if get_button_pos().length() > threshold:
		return get_button_pos().normalized()
	return Vector2(0, 0)
