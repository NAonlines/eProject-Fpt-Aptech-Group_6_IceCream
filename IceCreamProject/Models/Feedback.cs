using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IceCreamProject.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public string Name { get; set; } // Thêm Name vào đây

        public string Content { get; set; }

        public DateTime SubmittedDate { get; set; }

        // Foreign keys
        public string? UserId { get; set; } // Liên kết đến User.Id

        public string? Email { get; set; } // Email của người gửi

        // Navigation properties
        public User? User { get; set; }
    }
}
