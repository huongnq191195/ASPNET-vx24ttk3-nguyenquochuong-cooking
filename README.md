# ĐỒ ÁN MÔN HỌC: CHUYÊN ĐỀ ASP.NET

## ĐỀ TÀI: XÂY DỰNG WEBSITE CHIA SẺ CÔNG THỨC NẤU ĂN

---

###  THÔNG TIN SINH VIÊN & GIẢNG VIÊN HƯỚNG DẪN

* **Sinh viên thực hiện:** Nguyễn Quốc Hướng
* **Mã lớp:** VX24TTK3
* **Email:** huongnq191195@tvu-onschool.edu.vn
* **SĐT:** 0939764766
* **Giảng viên hướng dẫn:** TS. Đoàn Phước Miền (Giáo viên quản trị: `antonio86doan@gmail.com`)

---

## 1. Giới thiệu đề tài
Website **Chia sẻ công thức nấu ăn** là một nền tảng trực tuyến giúp kết nối những người yêu thích ẩm thực. Hệ thống cho phép người dùng tìm kiếm, học hỏi và chia sẻ các công thức nấu ăn ngon, đồng thời cung cấp công cụ quản trị mạnh mẽ cho Admin để kiểm soát nội dung.

### Các công nghệ sử dụng:
* **Backend:** ASP.NET Core (.NET 8 SDK)
* **Database:** SQL Server (Express / LocalDB)
* **ORM:** Entity Framework Core
* **Frontend:** HTML5, CSS3, JavaScript, Razor Pages/MVC Views

---

## 2. Các chức năng cốt lõi của hệ thống

###  Đối với Người dùng (User)
* Đăng ký tài khoản mới và Đăng nhập / Đăng xuất hệ thống.
* Xem danh sách công thức món ăn, tìm kiếm công thức theo từ khóa hoặc bộ lọc.
* Xem chi tiết từng món ăn (nguyên liệu, các bước thực hiện, hình ảnh trực quan).
* Tương tác xã hội: Thích (Like) công thức, để lại bình luận (Comment).
* Tính năng cá nhân hóa: Lưu các công thức nấu ăn yêu thích vào danh mục riêng.

###  Đối với Quản trị viên (Admin)
* Duyệt hoặc từ chối các công thức món ăn mới do người dùng đóng góp.
* Quản lý danh mục món ăn (Thêm, sửa, xóa các loại hình ẩm thực).
* Thay đổi và cấu hình giao diện hệ thống (Hình nền, thông tin cơ bản).

---

## 3. Cấu trúc thư mục dự án
Hệ thống được tổ chức khoa học để phục vụ việc chấm điểm và kiểm tra tiến độ:
* `setup/` : Chứa file cơ sở dữ liệu `WebNauAnDb_V16.sql` và tài liệu hướng dẫn chi tiết.
* `progress-report/` : Thư mục lưu trữ các file báo cáo tiến độ thực hiện đồ án hàng tuần.
* `wwwroot/images/` : Thư mục lưu trữ toàn bộ hình ảnh thực tế của các món ăn trên website.

---

## 4. Hướng dẫn cài đặt và chạy chương trình

Để hỗ trợ Giảng viên cài đặt và chấm điểm nhanh chóng, toàn bộ quy trình cấu hình chi tiết đã được biên soạn riêng. Vui lòng truy cập đường dẫn dưới đây:

 **[XEM HƯỚNG DẪN CÀI ĐẶT CHI TIẾT TẠI ĐÂY](setup/huong_dan_cai_dat.md)**

### Tóm tắt các bước thực hiện nhanh:
1. **Tải mã nguồn:** Clone hoặc download mã nguồn từ Repository này về máy.
2. **Khôi phục Database:** Mở SQL Server Management Studio (SSMS), mở và thực thi (`Execute`) file script tại đường dẫn `setup/WebNauAnDb_V16.sql` để khởi tạo database `WebNauAnDb_V16` cùng dữ liệu mẫu.
3. **Mở dự án:** Khởi động Visual Studio 2022 (hoặc mới hơn) và mở file Solution (`.sln`) của dự án.
4. **Kiểm tra kết nối:** Đảm bảo chuỗi kết nối (Connection String) trong file `Program.cs` khớp với cấu hình SQL Server trên máy của thầy.
5. **Khởi chạy:** Nhấn `F5` hoặc nút `Play` để chạy ứng dụng trên trình duyệt.

---

## 5. Tài khoản thử nghiệm (Demo Accounts)

Sau khi khôi phục database thành công, thầy có thể sử dụng các tài khoản mẫu dưới đây để kiểm tra toàn bộ phân quyền hệ thống:

| Vai trò (Role) | Tên đăng nhập (Username) | Mật khẩu (Password) |
| :--- | :--- | :--- |
| **Quản trị viên (Admin)** | `admin` | `123` |
| **Người dùng mẫu 1** | `nqhuong` | `111` |
| **Người dùng mẫu 2** | `nntuyen` | `111` |
| **Người dùng mẫu 3** | `nnthuy` | `111` |
| **Người dùng mẫu 4** | `npqui` | `111` |
| **Người dùng mẫu 5** | `txkieu` | `111` |
| **Người dùng mẫu 6** | `test123` | `111` |

---

## 6. Quản lý tiến độ đồ án
* **Báo cáo tuần:** Được cập nhật liên tục tại thư mục **`[progress-report/]`** theo đúng quy định của học phần.
* **Lịch sử commit:** Được duy trì đều đặn tối thiểu 1 tuần/lần để minh chứng cho tiến độ thực hiện thực tế của sinh viên.