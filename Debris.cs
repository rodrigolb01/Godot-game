using Godot;
using System;


public class Debris : RigidBody2D
{
	private AnimationPlayer animation1;
	private Timer moveTimer;
	public override void _Ready()
	{
		animation1 = GetNode<AnimationPlayer>("AnimationPlayer");
		animation1.Play("Moving");
		
	}

	public override void _Process(float delta)
	{
		if(Position.x >= 2000 || Position.y >= 2000)
		{
			QueueFree();
		}
	}

	

}
