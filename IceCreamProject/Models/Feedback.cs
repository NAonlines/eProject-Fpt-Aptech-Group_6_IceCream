using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Feedback
{
	public int FeedbackId { get; set; }

	public string Content { get; set; }

	public DateTime SubmittedDate { get; set; }

	// Foreign keys
	public string UserId { get; set; } // Liên kết đến User.Id

	// Navigation properties
	public User User { get; set; }
}
