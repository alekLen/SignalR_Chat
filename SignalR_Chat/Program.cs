using Microsoft.EntityFrameworkCore;
using SignalR_Chat;
using SignalR_Chat.Models;// ������������ ���� ������ ChatHub

var builder = WebApplication.CreateBuilder(args);
// ��� ������������� ���������������� ���������� SignalR,
// � ���������� ���������� ���������������� ��������������� �������
builder.Services.AddSignalR();
// �������� ������ ����������� �� ����� ������������
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<ChatContext>(options => options.UseSqlServer(connection));


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chat");   // ChatHub ����� ������������ ������� �� ���� /chat

app.Run();
