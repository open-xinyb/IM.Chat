using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chat.Instance.Model;
using System.Threading.Tasks;
using Chat.Instance.Data;
using Chat.Instance.Utility;
using System.Data.SqlClient;
using System.Collections.Concurrent;

namespace Chat.Instance
{
    [HubName("chat")]
    public class Chat : Hub
    {
        #region recieve parameters
        public string RoomKey
        {
            get
            {
                return Context.QueryString["RoomKey"];
            }
        }

        public string UserKey
        {
            get 
            {
                return Context.QueryString["UserKey"];
            }
        }

        public string UserName
        {
            get
            {
                return HttpUtility.UrlDecode(Context.QueryString["UserName"]);
            }
        }
        #endregion

        private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>(StringComparer.InvariantCultureIgnoreCase);

        public static List<ChatUser> chatUser = new List<ChatUser>();

        public override Task OnConnected()
        {
            ChatProvider.ConnectRoom(UserKey, UserName, RoomKey, string.Empty);
            
            var user = Users.GetOrAdd(RoomKey, _ => new User
            {
                ProfileId = RoomKey,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(Context.ConnectionId);
                Groups.Add(Context.ConnectionId, RoomKey);
            }
                        
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool flag)
        {            
            User user;
            if (RoomKey != null)
            {
                Users.TryGetValue(RoomKey, out user);
                if (user != null)
                {
                    lock (user.ConnectionIds)
                    {
                        user.ConnectionIds.RemoveWhere(cid => cid.Equals(Context.ConnectionId));
                        Groups.Remove(Context.ConnectionId, user.ProfileId);
                        if (!user.ConnectionIds.Any())
                        {
                            User removedUser;
                            Users.TryRemove(RoomKey, out removedUser);
                        }
                    }
                }
            }
            return base.OnDisconnected(true);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public IEnumerable<string> GetConnectedUser()
        {
            return Users.Where(x =>
            {
                lock (x.Value.ConnectionIds)
                {
                    return !x.Value.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
                }
            }).Select(x => x.Key);
        }

        [HubMethodName("initialChat")]
        public Task InitialChat()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Chat>();
            return context.Clients.Client(Context.ConnectionId).RecieveMessages(ChatProvider.GetChat(RoomKey));
        }

        [HubMethodName("sendMessage")]
        public Task SendMessage(string message)
        {
            ChatProvider.Send(UserKey, RoomKey, message);
            return Clients.Group(RoomKey).MessageReceived(message, UserName, DateTime.Now.ToDate());
        }
    }
}