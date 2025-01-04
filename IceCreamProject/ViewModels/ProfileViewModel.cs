namespace IceCreamProject.ViewModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ProfileImageUrl { get; set; } // Correct path to the profile image
        public IFormFile ProfileImage { get; set; }
    }

}
