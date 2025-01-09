using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.Models
{
    public class FeedbackResponse
    {
        [Key]
        public int ResponseId { get; set; }

        public int FeedbackId { get; set; }

        public string ResponseContent { get; set; } = null!;

        public DateTime? RespondedDate { get; set; }

        public string RespondedBy { get; set; } = null!;
    }
}
