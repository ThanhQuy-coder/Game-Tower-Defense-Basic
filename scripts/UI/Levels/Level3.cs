using Godot;
using System;

public partial class Level3 : Node2D
{
	[Export] public AudioStream BackgroundMusic;

	public override void _Ready()
	{
		var music = GetNode<MusicManager>("/root/MusicManager");
		music.PlayMusic(BackgroundMusic);
	}
}
