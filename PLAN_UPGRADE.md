# KẾ HOẠCH NÂNG CẤP CYBERFORUM

## 📋 Tổng quan yêu cầu

| # | Tính năng | Mức độ | Trạng thái |
|---|-----------|--------|------------|
| 1 | Hệ thống điểm uy tín (Reputation System) | Cao | ✅ Hoàn thành |
| 2 | Chat realtime (Real-time Chat) | Cao | ✅ Hoàn thành |
| 3 | Fix phông chữ & màu chữ | Trung bình | ✅ Hoàn thành |
| 4 | Đổi giao diện sáng/tối (Theme Switcher) | Trung bình | ✅ Hoàn thành |
| 5 | Thu gọn taskbar (Sidebar collapse) | Thấp | ✅ Hoàn thành |
| 6 | Đưa đăng xuất xuống quản trị (Move logout to admin) | Thấp | ✅ Hoàn thành |

---

## 🏗️ Phân tích hiện trạng codebase

### Cấu trúc dự án
- **Backend**: ASP.NET Core 8.0 Web API + MVC (hybrid)
- **Frontend**: HTML tĩnh + Vanilla JS + Bootstrap 5 + Tabler Icons
- **Database**: SQL Server + Entity Framework Core
- **Realtime**: SignalR (ChatHub đã có sẵn)
- **Auth**: JWT + Cookie

### Entity liên quan
- `User.ReputationPoints` ✅ (đã có)
- `User.Rank` ✅ (đã có với 6 cấp: Newbie → Elite)
- `Badge` ✅ (đã có hệ thống huy hiệu)
- `ActivityLog` ✅ (đã ghi log)
- `ChatRoom`, `ChatMessage`, `ChatRoomMember` ✅ (realtime chat backend OK)
- `Notification` ✅ (đã có)

### Tồn tại sẵn
- ✅ `PostService.VoteAsync` đã cộng/trừ `ReputationPoints` (dòng 428, 445, 468)
- ✅ `PostService.UpdateReputation` đã cập nhật `Rank` tự động
- ✅ ChatHub với `OnConnected`, `JoinRoom`, `SendMessage`, `EditMessage`, `DeleteMessage`
- ✅ `chat.html` đã có giao diện nhưng còn sơ sài (thiếu typing, online status, read receipt, group members modal)
- ✅ cyber.css đã dark theme hoàn chỉnh (Glassmorphism, Glow, Gradient)
- ❌ Chưa có light theme
- ❌ Chưa có theme switcher
- ❌ Sidebar chưa có nút thu gọn (chỉ responsive trên mobile)
- ❌ Logout có ở nhiều nơi (navbar, sidebar, user menu), cần dồn về admin

---

## 🎯 Kế hoạch thực hiện (chi tiết)

### 1️⃣ Hệ thống điểm uy tín (Reputation System)

**Backend:**
- [ ] Tạo `ReputationService` mới trong `Services/`
  - Quy tắc cộng/trừ điểm:
    - Đăng bài viết: **+5**
    - Bài viết được upvote: **+10** mỗi vote
    - Bài viết bị downvote: **-2** mỗi vote
    - Bình luận: **+2**
    - Bình luận được upvote: **+5** mỗi vote
    - Bình luận bị downvote: **-1** mỗi vote
    - Nhận downvote: **-5** (thay vì -2)
    - Bài viết bị xóa: **-15**
    - Bình luận bị xóa: **-5**
- [ ] Tạo `ReputationHistory` entity mới + migration
- [ ] Tạo `ReputationController` với endpoints:
  - `GET /api/reputation/me` - Lịch sử của tôi
  - `GET /api/reputation/user/{id}` - Điểm uy tín user
  - `GET /api/reputation/leaderboard` - Top users
- [ ] Cập nhật `PostService` và `CommentService` để dùng `ReputationService`
- [ ] Cập nhật `BadgeService.CheckAndAwardBadgesAsync` với logic mới

**Frontend:**
- [ ] Trang `reputation.html` (hoặc tab trong `profile.html`): Lịch sử điểm uy tín
- [ ] Widget hiển thị điểm uy tín trên bài viết/bình luận
- [ ] Cập nhật `leaderboard.html` với filter theo khoảng thời gian
- [ ] Tooltip giải thích cách tích điểm

---

### 2️⃣ Chat realtime cải thiện UX

**Backend (đã có, cần tinh chỉnh):**
- [ ] Thêm method `GetRoomMembers` trong `ChatService`
- [ ] Thêm method `MarkAsRead` để update `LastReadAt`
- [ ] Bổ sung broadcast `MessageRead` qua SignalR
- [ ] Lưu trạng thái online vào bộ nhớ (in-memory dictionary)

**Frontend (cải thiện đáng kể):**
- [ ] Hiển thị danh sách thành viên phòng (modal)
- [ ] Hiển thị trạng thái online/offline (chấm xanh/xám)
- [ ] Tạo phòng nhóm có chọn thành viên (modal search users)
- [ ] Hiển thị "đang nhập..." (typing indicator - đã có, cần tinh chỉnh)
- [ ] Hiển thị "đã xem" (read receipt)
- [ ] Phân biệt tin nhắn của mình/người khác rõ ràng
- [ ] Thêm emoji picker (optional)
- [ ] Cuộn xuống tin nhắn mới tự động
- [ ] Thông báo tin nhắn mới (Notification API)
- [ ] Số lượng tin nhắn chưa đọc trên sidebar (badge)

---

### 3️⃣ Fix phông chữ & màu chữ

**Phông chữ:**
- [ ] Đảm bảo tất cả HTML load đúng font `Outfit` + `JetBrains Mono`
- [ ] Thêm fallback font hợp lý
- [ ] Fix font cho tiếng Việt (UTF-8 encoding đã OK)
- [ ] Thêm các trọng lượng font cần thiết (300, 400, 500, 600, 700)

**Màu chữ:**
- [ ] Sửa các chỗ dùng màu cứng (`#fff`, `#000`) thay bằng biến CSS
- [ ] Đảm bảo contrast ratio WCAG AA
- [ ] Sửa màu text trong:
  - `.btn-ghost` (màu mặc định `var(--text-muted)` - tốt)
  - `.toast` (text primary - tốt)
  - `.form-input` (text primary - tốt)
  - Một số chỗ inline style `color: #fff` trong `notification-bell.js` (sửa thành var)
  - Trong `admin.html`: sửa các màu cứng trong chart (`#8b949e`, `#30363d33`)

---

### 4️⃣ Đổi giao diện Sáng/Tối (Theme Switcher)

**Backend:**
- Không cần thay đổi (frontend only)

**Frontend:**
- [ ] Tạo file `light.css` (theme sáng, bổ sung vào sau `cyber.css`)
- [ ] Override các biến CSS trong `:root[data-theme="light"]`:
  - `--bg-primary: #f5f7fa`
  - `--bg-surface: rgba(255, 255, 255, 0.85)`
  - `--bg-surface-solid: #ffffff`
  - `--text-primary: #0d1117`
  - `--text-secondary: #1f2937`
  - `--text-muted: #6b7280`
  - `--border: rgba(0, 0, 0, 0.1)`
  - `--accent: #00b87a` (giữ xanh nhưng đậm hơn để nổi trên nền sáng)
- [ ] Tạo module `ThemeManager` trong JS
- [ ] Lưu lựa chọn vào `localStorage`
- [ ] Tự động áp dụng theo `prefers-color-scheme` lần đầu
- [ ] Nút toggle trên navbar (icon mặt trời/mặt trăng)
- [ ] Sửa một số màu cứng (chart, code highlight) cho phù hợp light theme
- [ ] Sửa highlight.js theme cho cả 2 mode

---

### 5️⃣ Thu gọn taskbar (Sidebar collapse)

**Frontend:**
- [ ] Thêm nút toggle trên navbar (icon `ti ti-layout-sidebar`)
- [ ] CSS class `body.sb-sidenav-toggled` đã có sẵn concept từ SB Admin
- [ ] Lưu trạng thái vào `localStorage`
- [ ] Trên mobile: thêm nút hamburger
- [ ] Hiệu ứng transition mượt mà
- [ ] Thu gọn thành icon-only với tooltip

---

### 6️⃣ Đưa "Đăng xuất" xuống mục Quản trị

**Hiện trạng:**
- Có nút đăng xuất ở 4 chỗ: navbar, sidebar, user menu dropdown, post.html navbar
- Mục "Quản trị" trong sidebar đang có ở `requires-admin` (chỉ Admin mới thấy)

**Cách giải quyết:**
- [ ] Tạo menu "Tài khoản" trong sidebar thuộc `requires-auth`
- [ ] Trong menu này có: Hồ sơ, Cài đặt, Đăng xuất (chuyển xuống dưới cùng)
- [ ] Ẩn nút logout trên navbar (icon-only)
- [ ] Ẩn nút logout trong dropdown user-menu
- [ ] Giữ lại ở trang `admin.html` (vì user đã ở khu vực quản trị)
- [ ] Nếu user là admin/mod: thêm mục "Bảng quản trị" trong menu Tài khoản

---

## 📂 File sẽ tạo mới

| File | Mục đích |
|------|----------|
| `Services/ReputationService.cs` | Logic tính điểm uy tín |
| `Controllers/ReputationController.cs` | API điểm uy tín |
| `Models/Entities/ReputationHistory.cs` | Lịch sử điểm |
| `Migrations/...AddReputationHistory.cs` | EF Migration |
| `wwwroot/css/themes/light.css` | Theme sáng |
| `wwwroot/js/theme-manager.js` | Quản lý theme sáng/tối |
| `wwwroot/js/sidebar-toggle.js` | Thu gọn sidebar |
| `wwwroot/reputation.html` | Trang lịch sử điểm uy tín |
| `wwwroot/messages.html` (rename chat.html) | Chat cải thiện UX |

## ✏️ File sẽ sửa

| File | Sửa gì |
|------|--------|
| `Services/PostService.cs` | Tích hợp `ReputationService` |
| `Services/CommentService.cs` | Tích hợp `ReputationService` |
| `Services/BadgeService.cs` | Cập nhật logic với `ReputationHistory` |
| `Services/ChatService.cs` | Thêm method cho read receipt, online status |
| `Program.cs` | Đăng ký `IReputationService` |
| `Interfaces/IReputationService.cs` | Interface cho service mới |
| `wwwroot/index.html` | Thêm nút theme toggle + sidebar toggle |
| `wwwroot/chat.html` | Cải thiện UX chat |
| `wwwroot/css/themes/cyber.css` | Sửa màu cứng, bổ sung CSS sidebar collapse |
| `wwwroot/js/notification-bell.js` | Sửa màu cứng |
| `wwwroot/js/site.js` | Khởi tạo `ThemeManager` + `SidebarToggle` |
| Tất cả HTML pages | Ẩn nút logout trên navbar, thêm vào sidebar menu |

---

## 🚀 Lộ trình thực hiện (thứ tự ưu tiên)

1. **Phase 1 - Nền tảng (Cao nhất)**
   - Fix phông chữ & màu chữ (nhanh, ít rủi ro)
   - Tạo `theme-manager.js` + `light.css` (sáng/tối)
   - Thu gọn sidebar
   
2. **Phase 2 - Tính năng chính**
   - Hệ thống điểm uy tín (Reputation System)
   - Cải thiện chat realtime

3. **Phase 3 - Hoàn thiện UX**
   - Đưa logout xuống sidebar menu
   - Tinh chỉnh cuối cùng

---

## ⚠️ Lưu ý kỹ thuật

- **EF Migration**: Cần chạy `dotnet ef migrations add AddReputationHistory` sau khi tạo entity mới
- **Breaking changes**: ReputationService thay thế logic trực tiếp trong PostService → cần test kỹ voting flow
- **SignalR**: Đã có ChatHub, chỉ cần bổ sung broadcast event, không cần re-architect
- **Theme**: Cần test cả 2 mode trên tất cả 22 trang HTML
- **Sidebar collapse**: Lưu `localStorage` key `cf_sidebar_collapsed` để persist
- **Theme**: Lưu `localStorage` key `cf_theme` giá trị `dark` | `light`

---

## ✅ Tổng kết triển khai

### Trạng thái các phase

| Phase | Nội dung | Trạng thái |
|-------|----------|------------|
| 1 | Fix phông chữ & màu chữ | ✅ Hoàn thành |
| 1 | Theme sáng/tối + FOUC prevention | ✅ Hoàn thành |
| 1 | Thu gọn sidebar với localStorage | ✅ Hoàn thành |
| 2 | Hệ thống điểm uy tín (Entity + Service + Controller + UI) | ✅ Hoàn thành |
| 2 | Cải thiện chat realtime (reactions, pin, search, online) | ✅ Hoàn thành |
| 3 | Đưa logout xuống sidebar menu (12 trang HTML) | ✅ Hoàn thành |

### File mới được tạo

| File | Mục đích |
|------|----------|
| `wwwroot/css/themes/light.css` | Theme sáng override |
| `wwwroot/css/notification-bell.css` | CSS tách riêng cho notification bell |
| `wwwroot/js/theme-manager.js` | Quản lý theme sáng/tối |
| `wwwroot/js/sidebar-toggle.js` | Thu gọn/mở rộng sidebar |
| `Models/Entities/ReputationHistory.cs` | Entity lịch sử điểm uy tín |
| `Models/Entities/ChatMessageReaction.cs` | Entity reactions tin nhắn |
| `Interfaces/IReputationService.cs` | Interface reputation service |
| `Services/ReputationService.cs` | Logic cộng/trừ điểm |
| `Controllers/ReputationController.cs` | API reputation |
| `wwwroot/reputation.html` | Trang lịch sử + bảng xếp hạng điểm uy tín |
| `apply-migrations.ps1` | Script PowerShell apply migrations |
| `Migrations/...AddReputationHistory.cs` | EF migration reputation |
| `Migrations/...AddChatReactionsAndPinning.cs` | EF migration reactions & pinning |

### File đã sửa đáng chú ý

| File | Sửa gì |
|------|--------|
| `Services/PostService.cs` | Tích hợp `ReputationService` cho tạo/xóa/vote bài viết |
| `Services/CommentService.cs` | Tích hợp `ReputationService` cho tạo/xóa/vote bình luận |
| `Services/ChatService.cs` | Thêm 11 method mới (send/edit/delete/react/pin/search/addMember/markRead) |
| `Hubs/ChatHub.cs` | Thêm 3 method SignalR (ToggleReaction, TogglePinMessage, MarkAsRead) |
| `Controllers/ChatController.cs` | Thêm 9 endpoint mới |
| `Models/Entities/ChatMessage.cs` | Thêm PinnedAt, PinnedById, Reactions |
| `Models/Entities/User.cs` | Thêm navigation cho ReputationHistories |
| `Models/DTOs/OtherDto.cs` | Thêm ReactionDto, cập nhật ChatMessageDto, ReputationDto |
| `Models/DTOs/PostDto.cs` | Thêm AuthorRank, AuthorReputation |
| `Data/AppDbContext.cs` | DbSet mới + relationships + indexes |
| `Program.cs` | Đăng ký IReputationService |
| `wwwroot/js/api.js` | API endpoints reputation & chat |
| `wwwroot/js/site.js` | SiteUtils.bindLogoutButtons + auto-bind |
| `wwwroot/js/notification-bell.js` | Refactor inline style → CSS classes |
| `wwwroot/css/themes/cyber.css` | Thêm CSS variables, font weights, sidebar collapse, rank badges, theme toggle |
| `wwwroot/chat.html` | Viết lại hoàn toàn với UX mới |
| 12 trang HTML khác | Di chuyển nút logout, thêm SiteUtils, reputation link |

### Test API

| Endpoint | Status | Kết quả |
|----------|--------|---------|
| `GET /api/reputation/rules` | 200 | Trả về 8 rules + 6 ranks |
| `GET /api/reputation/leaderboard` | 200 | Trả về danh sách user theo điểm |

### Cách áp dụng migrations

```powershell
# Chạy script PowerShell
.\apply-migrations.ps1

# Hoặc chạy trực tiếp
dotnet ef database update
```

### Cách sử dụng các tính năng mới

1. **Điểm uy tín**: Truy cập `/reputation.html` để xem lịch sử, bảng xếp hạng
2. **Theme sáng/tối**: Bấm vào nút theme toggle trên navbar hoặc dùng `ThemeManager.toggle()`
3. **Thu gọn sidebar**: Bấm vào nút toggle trên sidebar, trạng thái được lưu vào localStorage
4. **Chat reactions**: Hover vào tin nhắn → click icon emoji → chọn reaction
5. **Pin message**: Hover vào tin nhắn → click icon pin
6. **Search chat**: Click icon search trên header chat
7. **Đăng xuất**: Vào sidebar "Quản trị" → click "Đăng xuất" ở dưới cùng
