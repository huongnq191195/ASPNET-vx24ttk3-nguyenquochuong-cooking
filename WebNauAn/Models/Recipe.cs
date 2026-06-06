using System.Collections.Generic;

namespace WebNauAn.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public string NguyenLieu { get; set; } = string.Empty;
        public string CongThuc { get; set; } = string.Empty;
        public string HinhanhUrl { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public int LuotLike { get; set; } = 0;

        public string Username { get; set; } = string.Empty;
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // ==========================================
        // CÁC TRƯỜNG DỮ LIỆU ĐỂ LƯU LỊCH SỬ SỬA ĐỔI
        // ==========================================
        public bool IsSuaDoi { get; set; } = false; // Đánh dấu bài này có đang bị sửa hay ko
        public string? TenMonCu { get; set; }
        public string? NguyenLieuCu { get; set; }
        public string? CongThucCu { get; set; }
    }
}