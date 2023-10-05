using SignalR_Chat.Models;

namespace SignalR_Chat.Repository
{
    public interface IChatRepository
    {   
        Task<List<Message>> GetMessage();
        Task<List<User>> GetAllUsers();
        Task<IEnumerable<UserViewModel>> GetViewUsers();
        Task<User> GetUser(string name);
        Task<User> CheckUser(string id);
        void UpdateUser(User u, string id, string q);
        Task AddUser(string userName, string ConnectId, string q);
 
        Task AddMessage(Message mess);
        Task Save();
    }
}
