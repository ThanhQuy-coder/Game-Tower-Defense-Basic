# Ghi chú cho thuyết trình

## SRP (Single Responsibility Principle - Đơn trách nhiệm)

- folder `WaveManager` ra thành các folder `LevelFactory` và `WaveData`
  - `LevelFactory`: có chức năng duy nhất Khởi tạo cấu hình cho từng level
  - `WaveData`: có chức năng Lưu trữ thông tin cho một wave

## Mô hình Component-Based

- Hiện tại WaveManager vẫn đang làm 2 việc lớn: <strong>Quản lý logic dòng thời gian (Manager)</strong> và <strong>Thực thi việc tạo Node (Spawner)</strong>.
  - `PathSpawner`: chịu trách nhiệm gắn quái vào path
  - `WaveTimeline`: chịu trách nhiệm lưu trữ thông tin wave

> `Engine` là khái niệm chỉ “hệ thống nền tảng xử lý các chức năng cơ bản”.
>
> `Orchestrator` là khái niệm dùng để chỉ một thành phần có nhiệm vụ “điều phối, kết nối và quản lý các thành phần khác”
