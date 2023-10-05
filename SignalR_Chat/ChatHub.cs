using AutoMapper;
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
                await rep.Save();
                // Вызов метода AddMessage на всех клиентах
                await Clients.All.SendAsync("AddMessage", username, message, mes.MessageDate);
            }
        }
     
      
        // Подключение нового пользователя
        public async Task Connect(string userName)
        {
            User u= await rep.GetUser(userName);
            var id = Context.ConnectionId;
            if (u == null)
            {
               await rep.AddUser(userName, id,"true");
              await  rep.Save();
            }
            User u1 = await rep.CheckUser(id);
            if (u != null && u1 == null)
            {
               rep.UpdateUser(u, id,"true");
               await rep.Save();
            } 
                IEnumerable<UserViewModel> Users = await rep.GetViewUsers();               
            // Вызов метода Connected только на текущем клиенте, который обратился к серверу
          await Clients.Caller.SendAsync("Connected", id, userName, Users.ToList());

            // Вызов метода NewUserConnected на всех клиентах, кроме клиента с определенным id
            // await Clients.AllExcept(id).SendAsync("NewUserConnected", id, userName);
            await Clients.AllExcept(id).SendAsync("NewUserConnected", id, userName, Users.ToList());

            List<Message> list = await rep.GetMessage();
                foreach (var l in list)
                {
                    // Вызов метода AddMessage на всех клиентах
                    await Clients.Caller.SendAsync("AddMessage", l.user.Name, l.Text, l.MessageDate);
                }

         }       

        // OnDisconnectedAsync срабатывает при отключении клиента.
        // В качестве параметра передается сообщение об ошибке, которая описывает,
        // почему произошло отключение.
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            User item =await rep.CheckUser(Context.ConnectionId);
            if (item != null)
            {
                rep.UpdateUser(item, "", "false");
                await rep.Save();
                //var id = Context.ConnectionId;
                IEnumerable<UserViewModel> Users = await rep.GetViewUsers();
                // Вызов метода UserDisconnected на всех клиентах
                await Clients.All.SendAsync("UserDisconnected", Users.ToList());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
