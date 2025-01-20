namespace BookStore.BLL.ViewModels.Authentication
{
    public class LoginResponseVm
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
    }
}
