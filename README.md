# Đồ Án Lập Trình Web

Dự án đồ án môn học Lập trình Web, được phát triển bằng ASP.NET Core MVC.

## Yêu Cầu Hệ Thống

- .NET 8.0 SDK (hoặc phiên bản tương thích)
- SQL Server (hoặc cơ sở dữ liệu được cấu hình trong `appsettings.json`)
- IDE: Visual Studio 2022 / VS Code / JetBrains Rider

## Hướng Dẫn Cài Đặt & Chạy Dự Án

1. Clone dự án về máy:
   ```bash
   git clone https://github.com/dthanhmong725/DOAN_LAPTRINH_WEB.git
   ```

2. Mở thư mục dự án.

3. Khôi phục các gói NuGet (dependencies):
   ```bash
   dotnet restore
   ```

4. Cấu hình cơ sở dữ liệu:
   - Mở file `appsettings.json`.
   - Chỉnh sửa chuỗi kết nối (`ConnectionStrings`) để trỏ đến SQL Server của bạn.

5. Chạy Entity Framework migrations để tạo/cập nhật database:
   ```bash
   dotnet ef database update
   ```

6. Khởi động ứng dụng:
   ```bash
   dotnet run
   ```
   Hoặc mở file `.sln` bằng Visual Studio và nhấn `F5` để chạy.

## Cấu Trúc Dự Án Chính

- `Controllers/`: Xử lý HTTP requests và điều phối logic.
- `Models/`: Các lớp đại diện cho cấu trúc dữ liệu (Entities).
- `Views/`: Giao diện người dùng được xây dựng bằng Razor.
- `wwwroot/`: Các tài nguyên tĩnh như CSS, JavaScript, Hình ảnh.
- `Data/`: Chứa DbContext và cấu hình truy cập cơ sở dữ liệu.
- `Services/` & `Interfaces/`: Chứa business logic, hỗ trợ Dependency Injection.
- `Middleware/`: Các thành phần tùy chỉnh trong pipeline xử lý request.
- `Hubs/`: Xử lý giao tiếp thời gian thực (SignalR).
