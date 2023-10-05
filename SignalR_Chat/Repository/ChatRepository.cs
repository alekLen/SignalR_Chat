using SignalR_Chat.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

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
        public async Task<List<User>> GetAllUsers()
        {
            return await db.Users.ToListAsync();
        }
        public async Task<User> CheckUser(string id)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.ConnectionId == id);
        }
       public  void UpdateUser(User u,string id ,string q)
        {
            u.ConnectionId = id;
            u.Active = q;
            db.Users.Update(u);
        }
        public async Task AddUser(string userName,string ConnectId,string q)
        {
            User user = new();
            user.Name = userName;
            user.ConnectionId = ConnectId;
            user.Active = q;
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
        public async Task<IEnumerable<UserViewModel>> GetViewUsers()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserViewModel>());
            var mapper = new Mapper(config);
            return mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(await GetAllUsers());
        }
    }
}
