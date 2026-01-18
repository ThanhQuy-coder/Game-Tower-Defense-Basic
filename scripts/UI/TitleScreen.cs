using Godot;

public partial class TitleScreen : Control
{
	[Export] public AudioStream BackgroundMusic;

	public override void _Ready()
	{
		var music = GetNode<MusicManager>("/root/MusicManager");
		music.PlayMusic(BackgroundMusic);
	}
}
