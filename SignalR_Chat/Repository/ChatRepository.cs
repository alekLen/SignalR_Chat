using SignalR_Chat.Models;
using Microsoft.EntityFrameworkCore;
namespace SignalR_Chat.Repository
{
    public class ChatRepository : IChatRepository
    {
        ChatContext db;
        public ChatRepository(ChatContext context)
        {
            db = context;
        }     
        public async Task<List<Message>> GetMessage()
        {
            return await db.Messages.Include(p => p.user).ToListAsync();
        }
        public async Task<User> GetUser(string name)
        {
            return await db.Users.FirstOrDefaultAsync(m => m.Name == name);
        }
        public bool CheckUser(string id)
        {
            return db.Users.Any(x => x.ConnectionId == id);
        }
        public async Task AddUser(string userName,string ConnectId)
        {
            User user = new();
            user.Name = userName;
            user.ConnectionId = ConnectId;
            await db.AddAsync(user);
        }  
        public async Task AddMessage(Message mess)
        {
            await db.AddAsync(mess);
        }
        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
    }
}
