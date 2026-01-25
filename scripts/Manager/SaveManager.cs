using Godot;
using System;

/// <summary>
/// Class tĩnh chịu trách nhiệm Đọc và Ghi dữ liệu xuống ổ cứng
/// </summary>
public static class SaveManager
{
    // Đường dẫn "user://" là đường dẫn đặc biệt của Godot, trỏ tới AppData trên máy người chơi
    // Nó không phải là thư mục res:// chứa source code
    private const string SAVE_PATH = "user://savegame.cfg"; 
    private const string SECTION = "GameData";
    private const string KEY_UNLOCKED = "UnlockedLevel";

    /// <summary>
    /// Lưu level cao nhất đã mở khóa
    /// </summary>
    public static void SaveProgress(int levelToSave)
    {
        // Kiểm tra xem level mới có cao hơn level đã lưu không
        // (Tránh trường hợp chơi lại màn 1 xong game lại lưu đè là mới mở màn 1)
        int currentSavedLevel = LoadProgress();
        if (levelToSave <= currentSavedLevel) return;

        var config = new ConfigFile();
        config.SetValue(SECTION, KEY_UNLOCKED, levelToSave);
        config.Save(SAVE_PATH);
        
        GD.Print($"[SaveManager] Đã lưu tiến độ: Mở khóa màn {levelToSave}");
    }

    /// <summary>
    /// Đọc level đã lưu từ ổ cứng
    /// </summary>
    public static int LoadProgress()
    {
        var config = new ConfigFile();
        Error err = config.Load(SAVE_PATH);

        // Nếu file chưa tồn tại (lần đầu chơi), trả về 1
        if (err != Error.Ok)
        {
            return 1;
        }

        // Lấy giá trị ra, mặc định là 1 nếu lỗi
        return (int)config.GetValue(SECTION, KEY_UNLOCKED, 1);
    }
}