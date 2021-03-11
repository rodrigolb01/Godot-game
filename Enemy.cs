using Godot;
using System;

public class Enemy : Node2D
{
	private AnimationPlayer animation1;
	private bool a=false;
	private bool d=false;
	private bool s=false;
	private bool w=false;
	private Timer timer1;
	private int health = 5;

	public void Damage(int amount)
	{
		health -= amount;
		if(health <= 0) 
			QueueFree();
	}
	public override void _Ready()
	{
		animation1 = GetNode<AnimationPlayer>("AnimationPlayer");

		var area = GetNode<Area2D>("Area2D");

		area.Connect("area_entered",this,nameof(OnCollision));
		area.Connect("area_exited",this,nameof(OnCollisionNoMore));

		timer1 = GetNode<Timer>("Attack_timer");
		timer1.Connect("timeout",this,nameof(OnTimerTimeout));
	}
	
	public override void _Process(float delta)
	{
		var player = GetNode<player2>("../Player2");
		float speed = 50;
		float moveAmount = speed * delta;
		Vector2 moveDirection = (player.Position - Position).Normalized();
		Position += moveDirection * moveAmount;

		var r = (player.Position - GlobalPosition).Angle() * 180/3.14159;

		if (r > -135 && r <= -45)
		{
			if(w==false)
			{
				w=true;
				d=false;
				s=false;
				a=false;
				animation_changeAngle();
			}
		}
		else if (r > -45 && r <=  45)
		{
			if(d==false)
			{
				d=true;
				w=false;
				s=false;
				a=false;
				animation_changeAngle();
			}
		}
		else if (r > 45 && r <= 135)
		{
			if(s==false)
			{
				s=true;
				d=false;
				a=false;
				w=false;
				animation_changeAngle();
			}
		}
		else
		{
			if(a==false)
			{
				a=true;
				d=false;
				w=false;
				s=false;
				animation_changeAngle();
			}
		}
	}

	private void OnCollision(Area2D with)
	{
		GD.Print("enemy detects colision!");
		if(with.GetParent() is Player player)
		{
			timer1.Start(1);
		}
	}

	private void OnCollisionNoMore(Area2D with)
	{
		if(with.GetParent() is Player player)
		{
			timer1.Stop();
		}
	}

	private void OnTimerTimeout()
	{
		var player = GetNode<Player>("../Player");
		if (player != null)
			player.Health -=2;
			GD.Print("enemy hurt player!");
	}
	void animation_changeAngle()
	{
		if (w)
		animation1.Play("Normal_W");
		if (d)
		animation1.Play("Normal_D");
		if (s)
		animation1.Play("Normal_S");
		if (a)
		animation1.Play("Normal_A");	
	}
}

