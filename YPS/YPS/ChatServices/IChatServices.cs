using YPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPS.ChatServices
{
    public interface IChatServices
    {
        Task Connect();
        Task Send(ChatMessage message, string ToUserId);
        event EventHandler<ChatMessage> OnChatMessageReceived;
    }
}
