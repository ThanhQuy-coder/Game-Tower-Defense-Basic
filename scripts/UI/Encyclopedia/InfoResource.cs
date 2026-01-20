using Godot;
using System;

// [SOLID: Single Responsibility] - Chỉ chứa dữ liệu, không chứa logic hiển thị
public partial class InfoResource : Resource
{
	[Export] public string Name;
	[Export] public Texture2D Icon;
	[Export(PropertyHint.MultilineText)] public string Description;

	// Các chỉ số như: Máu, Sát thương, Tốc độ...
	[Export] public string Health;
	[Export] public int Damage;
	[Export] public string Speed;
}
