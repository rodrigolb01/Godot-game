extends KinematicBody2D

onready var joystick = get_parent().get_node("MarginContainer/VBoxContainer/HBoxContainer/Control/joystick1/TouchScreenButton")
signal pos();
func _process(delta):
	emit_signal("pos");
	move_and_slide(joystick.get_value() * 1000)
	
	if(position.x <= 2000):
		position.x -= joystick.get_value() * 1000;
	if(position.y <=1080):
		position.y -= joystick.get_value() * 1000;
	

