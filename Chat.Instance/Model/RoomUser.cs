using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Instance.Model
{
    public class RoomUser
    {
        public string RoomKey { get; set; }
        public string UserKey { get; set; }
        public DateTime CreateTime { get; set; }
    }
}