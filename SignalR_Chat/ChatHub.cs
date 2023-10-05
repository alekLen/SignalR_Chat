using Microsoft.AspNetCore.SignalR;
using SignalR_Chat.Models;
using SignalR_Chat.Repository;
namespace SignalR_Chat
{
    /*
    Ключевой сущностью в SignalR, через которую клиенты обмениваются сообщеними 
    с сервером и между собой, является хаб (hub). 
    Хаб представляет некоторый класс, который унаследован от абстрактного класса 
    Microsoft.AspNetCore.SignalR.Hub.
    */
    public class ChatHub : Hub
    {
        static List<User> Users = new List<User>();
        IChatRepository rep;
        public ChatHub(IChatRepository rep)
        {
            this.rep = rep;
        }

        // Отправка сообщений
        public async Task Send(string username, string message)
        {
            Message mes = new();
            User user=await rep.GetUser(username);
            if (user == null) { }
            else
            {
                mes.Text = message; 
                mes.user = user;
                mes.MessageDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                await rep.AddMessage(mes);
                // Вызов метода AddMessage на всех клиентах
                await Clients.All.SendAsync("AddMessage", username, message, mes.MessageDate);
            }
        }
        public async Task GetMessages()
        {
            List<Message> list = await rep.GetMessage();
            foreach (var l in list)
            {              
                // Вызов метода AddMessage на всех клиентах
                await Clients.Caller.SendAsync("AddMessage", l.user.Name, l.Text, l.MessageDate);
            }
        }

        // Подключение нового пользователя
        public async Task Connect(string userName)
        {
            var id = Context.ConnectionId;
            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new User { ConnectionId = id, Name = userName });

                if (!rep.CheckUser(id))
                {
                    rep.AddUser(userName, id);
                }

                // Вызов метода Connected только на текущем клиенте, который обратился к серверу
                await Clients.Caller.SendAsync("Connected", id, userName, Users);

                // Вызов метода NewUserConnected на всех клиентах, кроме клиента с определенным id
                await Clients.AllExcept(id).SendAsync("NewUserConnected", id, userName);
            }
        }

        // OnDisconnectedAsync срабатывает при отключении клиента.
        // В качестве параметра передается сообщение об ошибке, которая описывает,
        // почему произошло отключение.
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                // Вызов метода UserDisconnected на всех клиентах
                await Clients.All.SendAsync("UserDisconnected", id, item.Name);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
