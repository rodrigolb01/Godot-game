using Godot;
using System;
using System.Collections.Generic;

public class player2 : KinematicBody2D
{
	private float default_minimum_dist=150;
	private AnimationPlayer animation1;
	private Timer shoot_cooldown;
	private Timer timer2;
	private Timer timer3;
	private TouchScreenButton up;
	private TouchScreenButton down;
	private TouchScreenButton right;
	private TouchScreenButton left;
	private TouchScreenButton fire;
	private TouchScreenButton joystick_button;
	private KinematicBody2D crosshair;
	private joystick1 joystick;
	private Line2D laser_sight;
	private Line2D line_to_closest_target;
	private Line2D default_range;
	private Enemy closest_enemy;
	private Vector2 closest_pos;
	private float closest_distance=1500;
	private List<Enemy> inrange = new List<Enemy>();
	private Area2D autoaimrange;
	private Sprite autoaimrange_area_marker;
   
	private CollisionShape2D autoaimrange_area;
	private bool spawn_enemies;
	private bool spawn_meteors;
	private bool w=false;
	private bool d=false;
	private bool s=false;
	private bool a=false;
	private bool shoot=false;
	private bool aiming=false;
	private float health = 10;
	private targetLock lock1;

	PackedScene bulletScene;    
	PackedScene enemyScene;
	PackedScene asteroidScene;
	PackedScene targetScene;
	public override void _Ready()
	{
		spawn_enemies = true;
		spawn_meteors = true;
		closest_pos = new Vector2(0,0);
		autoaimrange = GetNode<Area2D>("../Player2/Area2D");
		autoaimrange_area_marker = GetNode<Sprite>("../Player2/Area2D/Sprite");
		autoaimrange_area = GetNode<CollisionShape2D>("../Player2/Area2D/range of auto");
		autoaimrange.Connect("area_entered", this, nameof(attackClosestEnemy));
		autoaimrange.Connect("area_exited", this, nameof(resetAutoAim));
		laser_sight = GetNode<Line2D>("../Player2/Line2D");
		laser_sight.Hide();
		line_to_closest_target = GetNode<Line2D>("../Player2/Line2D_target");
		line_to_closest_target.Hide();
		default_range = GetNode<Line2D>("../Player2/default range");
		default_range.Hide();

		lock1 = GetNode<targetLock>("../Player2/targetLock");
		lock1.Hide();

		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
		asteroidScene = GD.Load<PackedScene>("res://Debris.tscn");

		targetScene = GD.Load<PackedScene>("res://targetLock.tscn");

		joystick = GetNode<joystick1>("../MarginContainer/VBoxContainer/HBoxContainer/Control/joystick1");
		joystick_button = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control/joystick1/TouchScreenButton");
		
		joystick.Connect("shooted",this,nameof(onShootPressed));
		joystick.Connect("aiming", this, nameof(onAimHolding));
		joystick.Connect("notAiming",this, nameof(onAimRelease));
		animation1 = GetNode<AnimationPlayer>("AnimationPlayer");
		shoot_cooldown = GetNode<Timer>("shootCooldown_timer");
		shoot_cooldown.Connect("timeout",this,nameof(OnTimerTimeout));
		timer2 = GetNode<Timer>("enemySpawn_timer");
		timer2.Connect("timeout",this,nameof(enemy_spawner));
		timer2.Start(2);
		timer3 = GetNode<Timer>("meteorSpawn_timer");
		timer3.Connect("timeout",this,nameof(spawnMeteors));
		timer3.Start(3);
		
		up = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_up");
		down = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_down");
		right = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_right");
		left = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control3/touch_left");
		fire = GetNode<TouchScreenButton>("../MarginContainer/VBoxContainer/HBoxContainer/Control/touch_fire");
		crosshair = GetNode<KinematicBody2D>("../crosshair");
	}

	public override void _Process(float delta)
	{
		foreach(Enemy e in inrange)
		{
			var dist = e.GlobalPosition.DistanceTo(GlobalPosition);
			if(dist < closest_distance)
			{
				closest_distance = dist;
				closest_pos = e.Position - Position;
				if(closest_enemy != e)
				{
					closest_enemy = e;
					if(!lock1.Visible)
						lock1.Show();
					lock1.GetNode<AnimationPlayer>("AnimationPlayer").Play("New Anim");
				}
			}
			else
			{
				if(closest_enemy != null)
					closest_pos = closest_enemy.Position - Position;
			}
		}

		if(closest_enemy!=null)
			lock1.Position = closest_pos;
		
		line_to_closest_target.RemovePoint(1);
		line_to_closest_target.AddPoint(closest_pos);

		if(inrange.Count>0 && fire.IsPressed() && closest_distance != default_minimum_dist)
		{
			if(shoot_cooldown.IsStopped())
			{
				Bullet bullet = (Bullet)bulletScene.Instance();
				bullet.Position = Position;
				bullet.Rotation = (closest_pos).Angle();
				shoot_cooldown.Start((float)0.4);
				shoot = false;	
				GetParent().AddChild(bullet);
				GetTree().SetInputAsHandled();
			}
		}
		if(fire.IsPressed())
		{
			if(!autoaimrange_area_marker.Visible)
				autoaimrange_area_marker.Show();
		}
		else
		{
			if(autoaimrange_area_marker.Visible)
				autoaimrange_area_marker.Hide();
		}
		if(!joystick_button.IsPressed())
		{
			if(crosshair.Visible)
				crosshair.Hide();
			if(crosshair.Position != Position)
				crosshair.Position = Position;
		}
		else
		{
			if(!crosshair.Visible)
				crosshair.Show();
		}
		
		
		//move in response to keys
		float speed = 500;
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

		MoveAndSlide(moveVector);
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
		if(aiming)
		{
			_Draw();
			Update();
			if(!laser_sight.Visible)
				laser_sight.Show();
		}
		else
		{
			if(laser_sight.Visible)
				laser_sight.Hide();
		}
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

	private void OnTimerTimeout()
	{
		shoot_cooldown.Stop();
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

	private void spawnMeteors()
	{
		if(spawn_meteors == true)
		{
			RigidBody2D a = (RigidBody2D)asteroidScene.Instance();
			uint randy = GD.Randi()%1000+1;
			Vector2 vector = new Vector2(-100,randy);
			a.Position = vector;
			Vector2 moveVector = new Vector2(0,0);
			moveVector += Transform.x.Normalized() * 80;
			a.ApplyCentralImpulse(moveVector);

			GetParent().AddChild(a);
		}
	}

	void onShootPressed()
	{
		if(shoot_cooldown.IsStopped())
		{
			Bullet bullet = (Bullet)bulletScene.Instance();
			bullet.Position = Position;
			bullet.Rotation = (crosshair.Position - GlobalPosition).Angle();
			GetParent().AddChild(bullet);
			GetTree().SetInputAsHandled();
			shoot_cooldown.Start((float)0.2);//start cooldown timer
			shoot = false;	
		}
		else
			return;	
	}

	void onAimHolding()
	{
		if(aiming == false)
			aiming = true;
		_Draw();
		Update();
	}
	void onAimRelease()
	{
		if(aiming == true)
		{
			aiming = false;
			if(!laser_sight.Visible)
				laser_sight.Hide();
		}
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

	private void attackClosestEnemy(Area2D with)
	{
		
		if(with.GetParent() is Enemy enemy)
		{
			Enemy target = (Enemy)with.GetParent();
			inrange.Add(target);
		}
	}

	private void resetAutoAim(Area2D with)
	{
		if(with.GetParent() is Enemy enemy)
		{ 
			if((Enemy)with.GetParent() == closest_enemy)
			{
				closest_enemy = null;
				if(lock1.Visible)
					lock1.Hide();
			}
			inrange.Remove((Enemy)with.GetParent());
			closest_distance = 1500;
		}
	}

	public override void _Draw()
	{ 
		Vector2 v = ToLocal(GetNode<CollisionShape2D>("../crosshair/CollisionShape2D").Position);
		laser_sight.RemovePoint(1);
		laser_sight.AddPoint((crosshair.Position-Position));
	}
}
