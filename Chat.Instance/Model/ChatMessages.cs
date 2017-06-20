using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Instance.Model
{
    public class ChatMessages
    {
        public Int32 MessageKey { get; set; }
        public string ChatContent { get; set; }
        public string MessageTime { get; set; }
        public string RoomKey { get; set; }
        public string UserKey { get; set; }
    }
}