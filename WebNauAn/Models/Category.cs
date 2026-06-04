namespace WebNauAn.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string TenQuocGia { get; set; } // Món Việt, Món Hàn, Món Thái...
        public string LoaiMon { get; set; } // Khai vị, Món chính, Tráng miệng...
    }
}