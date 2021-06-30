namespace MessageService.Models
{
    /// <summary>
    /// User creation request class.
    /// </summary>
    public class CreateUserRequest
    {
        // User username.
        public string UserName { get; set; }

        // User Email.
        public string EMail { get; set; }
    }
}