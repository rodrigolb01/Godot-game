using Godot;
using System;


public class joystick1 : Sprite
{

	[Signal]
	public delegate void shooted();
	[Signal]
	public delegate void aiming();
	[Signal]
	public delegate void notAiming();
	
	TouchScreenButton j;
	
	public override void _Ready()
	{
		j =  GetNode<TouchScreenButton>("TouchScreenButton");

		j.Connect("shoot",this, nameof(onShootPressed));
		j.Connect("aim",this, nameof(onAimHold));
		j.Connect("aimRelease",this, nameof(onAimRelease));
	}
	
	public void onShootPressed()
	{
		EmitSignal("shooted");
	}
	public void onAimHold()
	{
		EmitSignal("aiming");
	}
	public void onAimRelease()
	{
		EmitSignal("notAiming");
	}
}
