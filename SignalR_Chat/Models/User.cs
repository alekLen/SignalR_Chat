namespace SignalR_Chat.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ConnectionId { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public string Active { get; set; }
    }
}
