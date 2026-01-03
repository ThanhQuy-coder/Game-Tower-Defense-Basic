using Godot;

public partial class Cow : Node2D
{
	// * Tốc độ di chuyển của bò
	[Export] // * Giúp có thể thay đổi ngay trên inspector
	public float Speed = 10f;

	// * Khoảng cách bò đi được tính từ vị trí ban đầu
	[Export]
	public float MoveDistance = 15f;

	// * Thời gian bò đứng yên trước khi đổi hướng
	[Export]
	public float WaitTime = 10.0f;

	/*
	*    _startX: lưu vị trí X ban đầu để biết bò đi xa bao nhiêu
	*    _direction: hướng di chuyển (1 = phải, -1 = trái)
	*    _isMoving: cờ kiểm tra bò đang đi hay đứng yên
	*    _sprite: node hoạt hình (AnimatedSprite2D)
	*    _waitTimer: đồng hồ đếm ngược khi bò đứng yên
	*/
	private float _startX;
	private int _direction = 1;
	private bool _isMoving = false;
	private AnimatedSprite2D _sprite;
	private double _waitTimer = 0;

	// * Hàm khởi tạo của Godot, chạy 1 lần duy nhất
	// * Sử dụng để: lấy vị trí, tìm node con, bất animation mặc định,...
	public override void _Ready()
	{
		/*
		*	Lấy vị trí X ban đầu.
		*	Lấy node AnimatedSprite2D.
		*	Cho bò bắt đầu với animation "idle".
		*/

		_startX = GlobalPosition.X;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("idle");
	}

	// * Hàm cập nhật liên tục, tham số `delta` là thời gian trôi qua giữa 2 frame
	// * delta ~ fps
	// * Sử dụng để: viết logic xảy ra mỗi frame (di chuyển, kiểm tra va chạm,...)
	public override void _Process(double delta)
	{
		if (_isMoving)
		{
			// * Giúp bò di chuyển sang trái và phải
			Position += new Vector2(Speed * _direction * (float)delta, 0);

			// * Kiểm tra bò đã vượt ngoài phạm vi cho phép chưa
			if (Position.X >= _startX + MoveDistance || Position.X <= _startX - MoveDistance)
			{
				_isMoving = false;
				_waitTimer = WaitTime;
				_sprite.Play("idle");
			}
		}
		else
		{
			_waitTimer -= delta;
			if (_waitTimer <= 0)
			{
				_direction *= -1;
				_sprite.FlipH = _direction == -1;
				_sprite.Play("walk");
				_isMoving = true;
			}
		}
	}
}
