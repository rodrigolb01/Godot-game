using Godot;
using System;

public class Player : Node2D
{
	private AnimationPlayer animation1;
	private Timer timer1;
	private Timer timer2;
	private TouchScreenButton up;
	private TouchScreenButton down;
	private TouchScreenButton right;
	private TouchScreenButton left;
	private TouchScreenButton fire;
	private KinematicBody2D crosshair;
	private joystick1 joystick;
	private bool spawn_enemies;
	private bool w=false;
	private bool d=false;
	private bool s=false;
	private bool a=false;
	private bool t=false;
	private bool shoot=false;
	private float health = 10;
	PackedScene bulletScene;    
	PackedScene enemyScene;
	public override void _Ready()
	{
		spawn_enemies = false;
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
		
		
		joystick = GetNode<joystick1>("../MarginContainer/VBoxContainer/HBoxContainer/Control/joystick1");
		
		//GD.Print(j.GetSignalList());
		joystick.Connect("shooted",this,nameof(onShootPressed));
		
		animation1 = GetNode<AnimationPlayer>("AnimationPlayer");
		timer1 = GetNode<Timer>("shootCooldown_timer");
		timer1.Connect("timeout",this,nameof(OnTimerTimeout));
		timer2 = GetNode<Timer>("enemySpawn_timer");
		timer2.Connect("timeout",this,nameof(enemy_spawner));
		timer2.Start(3);
		
		up = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_up");
		down = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_down");
		right = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_right");
		left = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_left");
		fire = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control/touch_fire");
		crosshair = GetNode<KinematicBody2D>("../crosshair");
	}
	public override void _Process(float delta)
	{
		//move in response to keys
		float speed = 200;
		float moveAmount = speed * delta;
		
		Vector2 moveVector = new Vector2(0,0);
		if(up.IsPressed())
			moveVector.y -= moveAmount;
		if(down.IsPressed())
		   moveVector.y += moveAmount;
		if(right.IsPressed())
		   moveVector.x += moveAmount;
		if(left.IsPressed())
		   moveVector.x -= moveAmount;
		if(fire.IsPressed())
		{
			if(timer1.IsStopped())
				shoot = true;
		}
		Position += moveVector.Normalized() *moveAmount;
		
		// get the angle the player is aiming 
		double r = (crosshair.Position - GlobalPosition).Angle() * 180/3.14159;
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
		
		//shoot where the mouse is aiming at
		if(shoot)
		{
			onShootPressed();
		}
	}

	private void OnTimerTimeout()
	{
		timer1.Stop();
	}

	private void enemy_spawner()
	{
		if(spawn_enemies == true)
		{
			uint randy = GD.Randi()%800+1;
			Enemy enemy = (Enemy)enemyScene.Instance();
			Vector2 vector = new Vector2(0,randy);
			enemy.Position = vector;
			GetParent().AddChild(enemy);
			GetTree().SetInputAsHandled();
		}
	}

	void animation_changeAngle()
	{
		if(t == true)
		{
			if(w)
				animation1.Play("Move_W (waverider)");
			if(d)
				animation1.Play("Move_D (waverider");
			if(a)
				animation1.Play("Move_A (waverider");
			if(s)
				animation1.Play("Move_S (waverider");
		}
		else
		{
			if (w)
			animation1.Play("Move_W");
			if (d)
			animation1.Play("Move_D");
			if (s)
			animation1.Play("Move_S");
			if (a)
			animation1.Play("Move_A");
		}
	}

	void onShootPressed()
	{
		if(timer1.IsStopped())
		{
			GD.Print("emited!");
			Bullet bullet = (Bullet)bulletScene.Instance();
			bullet.Position = Position;
			bullet.Rotation = (crosshair.Position - GlobalPosition).Angle();
			GetParent().AddChild(bullet);
			GetTree().SetInputAsHandled();
			timer1.Start((float)0.5);//start cooldown timer
			shoot = false;	
		}
		else
			return;	
	}
	
	public float Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
			if(health <= 0)
			   QueueFree();
		}
	}
}
