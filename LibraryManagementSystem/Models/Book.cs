using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; } // 書籍唯一 ID，主鍵

        [Required]
        [StringLength(200)]
        public string Title { get; set; } // 書籍標題，必填，最大長度 200

        [Required]
        [StringLength(100)]
        public string Author { get; set; } // 作者名稱，必填，最大長度 100

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "可借閱"; // 書籍狀態，必填，預設為 "可借閱"

        [Required]
        [Range(0, int.MaxValue)]
        public int TotalCopies { get; set; } // 書籍總數量，必填，最小值 0

        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableCopies { get; set; } // 可借閱書籍數量，必填，最小值 0

        [StringLength(20)]
        public string? ISBN { get; set; } // 書籍 ISBN 編號，可選，最大長度 20

        public DateTime? PublishDate { get; set; } // 書籍出版日期，可選

        public string? CoverImagePath { get; set; }
    }
}
