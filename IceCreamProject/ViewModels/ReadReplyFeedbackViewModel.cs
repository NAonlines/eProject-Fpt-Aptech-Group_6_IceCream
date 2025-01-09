using System.ComponentModel.DataAnnotations;

namespace IceCreamProject.Models
{
	public class ReadReplyFeedbackViewModel
	{
		public List<Feedback> Feedbacks { get; set; }
		public List<FeedbackResponse> FeedbackResponses { get; set; }
	}
}

