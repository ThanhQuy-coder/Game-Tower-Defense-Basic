// using Godot;
// using System;

// // [Người 3] - Logic màn hình kết thúc
// public partial class GameOverUI : Control
// {
//     public override void _Ready()
//     {
//         Visible = false; // Ẩn lúc đầu
        
//         if (Global.Instance != null)
//         {
//             Global.Instance.OnGameOver += ShowGameOver;
//         }
//     }

//     private void ShowGameOver()
//     {
//         Visible = true;
//         GetTree().Paused = true; // Dừng toàn bộ game
//     }

//     public void _on_RestartButton_pressed()
//     {
//         GetTree().Paused = false;
        
//         // Reset chỉ số Global
//         if (Global.Instance != null)
//         {
//             Global.Instance.Health = 20;
//             Global.Instance.Gold = 100;
//             Global.Instance.Wave = 1;
//         }

//         // Load lại màn chơi hiện tại
//         GetTree().ReloadCurrentScene();
//     }
// }