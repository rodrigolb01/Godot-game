extends Node2D
onready var joystick = get_parent().get_node("MarginContainer/VBoxContainer/HBoxContainer/Control/joystick1/TouchScreenButton")

func _process(delta):
	move_and_slide(joystick.get_value() * 800)
