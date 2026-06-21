	# HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY HỆ THỐNG

## 1. Yêu cầu môi trường

- Visual Studio 2022 hoặc mới hơn
- .NET 8 SDK
- SQL Server Express hoặc SQL Server LocalDB (khuyến nghị LocalDB đi kèm Visual Studio)
- SQL Server Management Studio (SSMS)

---

## 2. Tải mã nguồn

- Clone hoặc tải source code từ GitHub.
- Mở file Solution (`.sln`) bằng Visual Studio.

---

## 3. Khôi phục cơ sở dữ liệu

### Bước 1: Mở SQL Server Management Studio (SSMS)

Kết nối tới SQL Server với các thông số:

```text
Server type: Database Engine
Server name: (localdb)\MSSQLLocalDB
Authentication: Windows Authentication
```

### Bước 2: Mở file SQL

Chọn:

```text
File → Open → File...
```

Mở file:

```text
setup/WebNauAnDb_V16.sql
```

Sau khi mở file, đảm bảo đang kết nối đúng Server `(localdb)\MSSQLLocalDB` trước khi nhấn Execute.

### Bước 3: Thực thi Script

Nhấn nút **Execute** để tạo cơ sở dữ liệu và dữ liệu mẫu.

Sau khi thực hiện thành công sẽ xuất hiện cơ sở dữ liệu:

```text
WebNauAnDb_V16
```

---

## 4. Kiểm tra chuỗi kết nối

Mở file:

```text
Program.cs
```

Kiểm tra chuỗi kết nối:

```csharp
options.UseSqlServer(
    "Server=(localdb)\\mssqllocaldb;Database=WebNauAnDb_V16;Trusted_Connection=True;MultipleActiveResultSets=true"
);
```

Nếu sử dụng SQL Server khác, cần điều chỉnh lại thông tin Server cho phù hợp.

---

## 5. Chạy chương trình

Trong Visual Studio:

- Build Solution
- Nhấn F5 hoặc Start Debugging

Hệ thống sẽ tự động mở trên trình duyệt.

---

## 6. Tài khoản mẫu

### Quản trị viên (Admin)

```text
Tên đăng nhập: admin
Mật khẩu: 123
```

### Người dùng thông thường

```text
Tên đăng nhập: nqhuong
Mật khẩu: 111
```

```text
Tên đăng nhập: nntuyen
Mật khẩu: 111
```

```text
Tên đăng nhập: npqui
Mật khẩu: 111
```

```text
Tên đăng nhập: txkieu
Mật khẩu: 111
```

```text
Tên đăng nhập: nnthuy
Mật khẩu: 111
```

```text
Tên đăng nhập: test123
Mật khẩu: 111
```

---

## 7. Chức năng hệ thống

### Người dùng

- Đăng ký tài khoản
- Đăng nhập / Đăng xuất
- Xem danh sách công thức
- Tìm kiếm công thức
- Xem chi tiết công thức
- Thích (Like) công thức
- Bình luận công thức
- Lưu công thức yêu thích

### Quản trị viên

- Duyệt công thức do người dùng gửi
- Tìm kiếm công thức nâng cao: Tìm kiếm nhanh các món ăn theo tên hoặc từ khóa ngay tại trang quản lý
- Bộ lọc trạng thái bài đăng: Lọc danh sách món ăn theo trạng thái (Đã duyệt / Chờ kiểm duyệt)
- Thêm danh mục món ăn
- Thay đổi hình nền hệ thống
- Xem báo cáo thống kê số lượng món ăn theo danh mục

---

## 8. Ghi chú

- Hình ảnh món ăn được lưu trong thư mục:

```text
wwwroot/images
```

- Cơ sở dữ liệu mẫu đã bao gồm tài khoản, công thức, bình luận và dữ liệu kiểm thử phục vụ đánh giá hệ thống.

- Sau khi import file SQL, có thể đăng nhập ngay bằng các tài khoản mẫu ở trên để kiểm tra chức năng.