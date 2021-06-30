namespace MessageService.Models
{
    /// <summary>
    /// Message creation request class.
    /// </summary>
    public class CreateMessageRequest
    {
        // Message subject.
        public string Subject { get; set; }

        // Message body.
        public string Message { get; set; }

        // Email address of the sender of the message.
        public string SenderId { get; set; }

        // Email address of the receiver of the message.
        public string ReceiverId { get; set; }
    }
}