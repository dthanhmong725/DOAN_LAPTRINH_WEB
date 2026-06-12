# Tổng hợp các tính năng mới đã được cập nhật

Dự án **CyberForum** vừa trải qua một đợt nâng cấp lớn về cả chức năng (Backend/Frontend) lẫn giao diện người dùng. Dưới đây là danh sách chi tiết toàn bộ các tính năng mới đã được thêm vào:

## 1. Tích hợp Swagger (API Documentation)
- Đã cài đặt thư viện `Swashbuckle.AspNetCore`.
- Cấu hình Swagger UI có hỗ trợ xác thực qua **JWT Bearer Token**. 
- Dễ dàng test các API bảo mật trực tiếp trên trình duyệt tại đường dẫn `/swagger`.

## 2. Quản lý Tài khoản & Bảo mật (Authentication)
- **Đăng xuất:** Tích hợp tính năng xóa Cookie và Token để đăng xuất an toàn.
- **Quên mật khẩu & Đổi mật khẩu:** Cập nhật Controller và UI cho phép người dùng thay đổi hoặc khôi phục mật khẩu.
- **Kiểm tra độ mạnh mật khẩu:** Cải tiến UI (sử dụng `password-strength.js`) để đánh giá độ mạnh của mật khẩu theo thời gian thực (độ dài, ký tự đặc biệt, số, chữ hoa/thường).
- **Xử lý lỗi Validation:** Sửa lỗi hiển thị form đăng ký/đăng nhập để ánh xạ chuẩn xác các thông báo lỗi (ValidationProblemDetails) từ ASP.NET Core trả về giao diện.

## 3. Trung tâm Hồ sơ Cá nhân (Profile Management)
- Thêm thuộc tính `CoverPhotoUrl` vào Entity `User` trong Database để hỗ trợ ảnh bìa.
- Hoàn thiện trang `profile.html` với các tính năng:
  - Xem và thay đổi thông tin cá nhân, tiểu sử (Giới thiệu).
  - Cập nhật ảnh đại diện (Avatar) và ảnh bìa (Cover Photo).
  - Xem thống kê hoạt động và danh sách các bài viết đã đăng của người dùng.

## 4. Trung tâm Quản lý Upload (Upload Center)
- Xây dựng Entity mới `UserUpload` và các file `UploadController`, `UploadService` ở Backend.
- Hoàn thiện trang `upload.html` cho phép người dùng:
  - Tải lên tài liệu (hỗ trợ các định dạng: PDF, Hình ảnh, Source Code, File Log).
  - Quản lý danh sách các file đã tải lên (xem, xóa, lấy link chia sẻ).

## 5. Nâng cấp Giao diện toàn diện (Premium Cyber & Bootstrap 5)
- **Tích hợp Bootstrap 5:** Chèn hệ thống lưới (Grid), Typography và Component của Bootstrap vào toàn bộ 22 file HTML (`wwwroot/*.html`).
- **Nâng cấp `cyber.css`:** Viết lại thư viện CSS theo phong cách **Premium Cyber** cao cấp, đóng vai trò như một Custom Theme đè lên Bootstrap.
  - **Glassmorphism:** Hiệu ứng kính mờ (blur) trong suốt sang trọng cho thanh điều hướng (Navbar) và thanh bên (Sidebar).
  - **Gradient & Glow Effect:** Hiệu ứng dải màu và đổ bóng phát sáng cho các Nút bấm, Thẻ bài viết (Card), và Danh mục.
  - **Typography:** Nâng cấp phông chữ mang đậm phong cách công nghệ số (sử dụng bộ font `Outfit` và `JetBrains Mono`).
- **Sửa lỗi mã hóa:** Xử lý triệt để lỗi hiển thị font tiếng Việt (Mojibake) do sai sót trong quá trình đọc/ghi file UTF-8.

---
> **Lưu ý:** Toàn bộ tính năng Frontend (HTML/JS/CSS) được thiết kế theo chuẩn tách biệt (Decoupled), gọi API trực tiếp từ Backend giúp dễ dàng bảo trì và mở rộng trong tương lai.
