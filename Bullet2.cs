using Godot;
using System;

public class Bullet2 : RigidBody2D
{
	private float range = 2000;
	private float distance_travelled = 0;
	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		float speed = 800;
		float move_amount = speed * delta;
		Vector2 moveVector = new Vector2(0,0);
		moveVector += Transform.x.Normalized() * move_amount;
		distance_travelled +=move_amount;
		
		ApplyCentralImpulse(moveVector);
	}
}
