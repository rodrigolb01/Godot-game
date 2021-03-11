using Godot;
using System;

public class Bullet : Node2D
{
	AnimationPlayer animation1;
   private float range = 1000;
   private float distanceTravelled = 0;
	public override void _Ready()
	{
		animation1 = GetNode<AnimationPlayer>("AnimationPlayer");
		animation1.Play("shooting");
		var area = GetNode<Area2D>("Area2D");
		area.Connect("area_entered",this,"OnCollision");
		area.Connect("body_entered",this,"OnCollision");
	}
	public override void _Process(float delta)
	{
		float speed = 800;
		float moveAmount = delta * speed;
		Position += Transform.x.Normalized() *moveAmount;
		distanceTravelled += moveAmount;
		if (distanceTravelled > range)
			QueueFree();
	}

	private void OnCollision(Node with)
	{
			 GD.Print("Bullet collided!");
			 with.GetParent<Enemy>().Damage(1);
			 QueueFree();
	}
}
