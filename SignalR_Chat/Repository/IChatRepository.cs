using SignalR_Chat.Models;

namespace SignalR_Chat.Repository
{
    public interface IChatRepository
    {   
        Task<List<Message>> GetMessage();
        Task<User> GetUser(string name);
        public bool CheckUser(string id);
        Task AddUser(string userName, string ConnectId);
 
        Task AddMessage(Message mess);
        Task Save();
    }
}
