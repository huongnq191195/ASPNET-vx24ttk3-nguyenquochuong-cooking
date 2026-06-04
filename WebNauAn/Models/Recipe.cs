namespace WebNauAn.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string TenMon { get; set; }

        // Lưu phân loại quốc gia (Món Việt, Món Hàn... hoặc chữ tự nhập)
        public string MucCha { get; set; }

        // Lưu loại món (Món chính, Món khai vị... hoặc chữ tự nhập)
        public string MucCon { get; set; }

        public string HinhanhUrl { get; set; }
        public string CongThuc { get; set; }

        // Trạng thái duyệt bài: Admin đăng = true (hiện luôn), Thành viên đăng = false (chờ duyệt)
        public bool IsApproved { get; set; }
    }
}