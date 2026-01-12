using Godot;
using System;

// [BASE CLASS]
// Chịu trách nhiệm: Di chuyển thẳng, xử lý va chạm cơ bản, tự hủy.
public abstract partial class BulletBase : Area2D
{
    [Export] public float Speed = 600.0f;
    [Export] public int Damage = 10;
    [Export] public float Lifetime = 3.0f;

    protected Vector2 Direction;
    protected bool IsInitialized = false;

    public override void _Ready()
    {
        // Tự hủy sau thời gian quy định
        GetTree().CreateTimer(Lifetime).Timeout += () => QueueFree();
        AreaEntered += OnHit;
    }

    public void Setup(Vector2 position, float rotation)
    {
        GlobalPosition = position;
        GlobalRotation = rotation;
        // Tính hướng bay dựa trên góc xoay
        Direction = Vector2.Right.Rotated(rotation);
        IsInitialized = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInitialized) return;
        // Bay thẳng tắp theo hướng đã định
        GlobalPosition += Direction * Speed * (float)delta;
    }

    // Template Method: Xử lý chung rồi gọi hàm riêng
    private void OnHit(Area2D area)
    {
        if (area is EnemyBase enemy)
        {
            ApplyEffect(enemy);
            CreateImpactEffect();
            QueueFree(); // Hủy đạn sau khi trúng
        }
    }

    // Các class con sẽ định nghĩa hiệu ứng cụ thể (Sát thương đơn, nổ lan, làm chậm...)
    protected abstract void ApplyEffect(EnemyBase enemy);

    protected virtual void CreateImpactEffect()
    {
        // TODO: Spawn hiệu ứng nổ/bụi tại GlobalPosition
    }
}