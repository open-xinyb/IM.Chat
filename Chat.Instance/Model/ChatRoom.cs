using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Instance.Model
{
    public class ChatRoom
    {
        public string RoomKey { get; set; }
        public string RoomName { get; set; }
        public string Creator { get; set; }
        public int RoomStatus { get; set; }
    }
}