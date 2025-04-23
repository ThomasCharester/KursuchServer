using System.Net.Sockets;
using System.Text;

namespace KursuchServer;

public class Command
{
    public CommandType Type;
    public TcpClient Client;
    public String Data;

    public virtual void Execute()
    {
    }

    public virtual void Undo()
    { 
    }
}