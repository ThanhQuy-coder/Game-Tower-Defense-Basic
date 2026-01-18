using Godot;

public partial class Level1 : Node2D
{
	[Export] public AudioStream BackgroundMusic;

	public override void _Ready()
	{
		var music = GetNode<MusicManager>("/root/MusicManager");
		music.PlayMusic(BackgroundMusic);
	}
}
