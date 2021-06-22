namespace AuthenticationService.DTOs
{
    public class CreateUserRequestBody
    {
        public int UserId { get; set; }
        
        public string UserName { get; set; }
    }
}