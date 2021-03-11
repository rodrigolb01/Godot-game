extends KinematicBody2D

onready var joystick = get_parent().get_node("MarginContainer/VBoxContainer/HBoxContainer/Control/joystick1/TouchScreenButton")
signal pos;
func _process(delta):
	move_and_slide(joystick.get_value() *1000)
	
##if(position.x >= 2200 || position.y >=1080 || position.x <=0 || position.y <=0):
		##position -= joystick.get_value() * 50;
	
	
	

