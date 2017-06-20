using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chat.Instance.Model;
using System.Data.SqlClient;
using System.Data;
using Chat.Instance.Utility;

namespace Chat.Instance.Data
{
    public class ChatProvider
    {
        public static void ConnectRoom(string userKey, string userName, string roomKey, string roomName)
        {
            if (!UserIsExists(userKey))
            {
                AddUser(new ChatUser() { UserKey = userKey, UserName = userName });
            }

            if (!RoomIsExists(new ChatRoom() { RoomKey = roomKey }))
            {
                AddRoom(new ChatRoom() { RoomKey = roomKey, RoomName = roomName, Creator = userKey, RoomStatus = 1 });
            }

            if (!UserIsInRoom(new RoomUser() { UserKey = userKey, RoomKey = roomKey }))
            {
                UserJoinRoom(new RoomUser() { UserKey = userKey, RoomKey = roomKey, CreateTime = DateTime.Now });
            }
        }

        public static void Send(string userKey, string roomKey, string chatContent)
        {
            SendChat(new ChatMessages()
            {
                RoomKey = roomKey,
                UserKey = userKey,
                ChatContent = chatContent,
                MessageTime = DateTime.Now.ToDate()
            });
        }

        public static List<ChatInfo> GetChat(string roomKey)
        {
            return GetRoomChatMessages(roomKey);
        }

        private static bool UserIsExists(string userKey)
        {
            string sqlScript = @"Select UserKey, UserName From ChatUser Where UserKey = @UserKey";
            
            SqlParameter[] parameters = new SqlParameter[]
			{
				SqlHelper.CreateInputSqlParameter("@UserKey", SqlDbType.NVarChar, userKey)
			};

            DataTable dt = SqlHelper.ExecuteDataset(ConnectionString.Writing, CommandType.Text, sqlScript, parameters).Tables[0];
            return dt.Rows.Count > 0;
        }

        private static void AddUser(ChatUser chatUser)
        {
            string sqlScript = @"Insert Into ChatUser(UserKey, UserName) values(@UserKey, @UserName)";

            SqlParameter[] parameters = new SqlParameter[]
			{
				SqlHelper.CreateInputSqlParameter("@UserKey", SqlDbType.NVarChar, chatUser.UserKey),
				SqlHelper.CreateInputSqlParameter("@UserName", SqlDbType.NVarChar, chatUser.UserName)
			};

            SqlHelper.ExecuteNonQuery(ConnectionString.Writing, CommandType.Text, sqlScript, parameters);
        }

        private static bool RoomIsExists(ChatRoom chatRoom)
        {
            string sqlScript = @"SELECT RoomKey, RoomName FROM ChatRoom Where RoomKey = @RoomKey";

            SqlParameter[] parameters = new SqlParameter[]
			{
				SqlHelper.CreateInputSqlParameter("@RoomKey", SqlDbType.NVarChar, chatRoom.RoomKey)
			};

            DataTable dt = SqlHelper.ExecuteDataset(ConnectionString.Writing, CommandType.Text, sqlScript, parameters).Tables[0];
            return dt.Rows.Count > 0;
        }

        private static void AddRoom(ChatRoom chatRoom)
        {
            string sqlScript = @"Insert Into ChatRoom(RoomKey, RoomName, Creator, RoomStatus) values(@RoomKey, @RoomName, @Creator, @RoomStatus)";

            SqlParameter[] parameters = new SqlParameter[]
			{
				SqlHelper.CreateInputSqlParameter("@RoomKey", SqlDbType.NVarChar, chatRoom.RoomKey),
				SqlHelper.CreateInputSqlParameter("@RoomName", SqlDbType.NVarChar, chatRoom.RoomName, true),
                SqlHelper.CreateInputSqlParameter("@Creator", SqlDbType.NVarChar, chatRoom.Creator, true),
                SqlHelper.CreateInputSqlParameter("@RoomStatus", SqlDbType.Int, chatRoom.RoomStatus, true)
			};

            SqlHelper.ExecuteNonQuery(ConnectionString.Writing, CommandType.Text, sqlScript, parameters);
        }

        private static bool UserIsInRoom(RoomUser roomUser)
        {
            string sqlScript = @"Select RoomKey, UserKey From RoomUser Where RoomKey = @RoomKey And UserKey = @UserKey";

            SqlParameter[] parameters = new SqlParameter[]
			{
                SqlHelper.CreateInputSqlParameter("@RoomKey", SqlDbType.NVarChar, roomUser.RoomKey),
				SqlHelper.CreateInputSqlParameter("@UserKey", SqlDbType.NVarChar, roomUser.UserKey)
			};

            DataTable dt = SqlHelper.ExecuteDataset(ConnectionString.Writing, CommandType.Text, sqlScript, parameters).Tables[0];
            return dt.Rows.Count > 0;
        }

        private static void UserJoinRoom(RoomUser roomUser)
        {
            string sqlScript = @"Insert Into RoomUser(RoomKey, UserKey, CreateTime) values(@RoomKey, @UserKey, @CreateTime)";

            SqlParameter[] parameters = new SqlParameter[]
			{
				SqlHelper.CreateInputSqlParameter("@RoomKey", SqlDbType.NVarChar, roomUser.RoomKey),
				SqlHelper.CreateInputSqlParameter("@UserKey", SqlDbType.NVarChar, roomUser.UserKey),
                SqlHelper.CreateInputSqlParameter("@CreateTime", SqlDbType.DateTime, roomUser.CreateTime, true)                
			};

            SqlHelper.ExecuteNonQuery(ConnectionString.Writing, CommandType.Text, sqlScript, parameters);
        }

        private static void SendChat(ChatMessages chatMessage)
        {
            string sqlScript = @"Insert Into ChatMessages(ChatContent, RoomKey, UserKey, MessageTime) Values(@ChatContent, @RoomKey, @UserKey, @MessageTime)";

            SqlParameter[] parameters = new SqlParameter[]
			{
                SqlHelper.CreateInputSqlParameter("@ChatContent", SqlDbType.NVarChar, chatMessage.ChatContent),
				SqlHelper.CreateInputSqlParameter("@RoomKey", SqlDbType.NVarChar, chatMessage.RoomKey),
				SqlHelper.CreateInputSqlParameter("@UserKey", SqlDbType.NVarChar, chatMessage.UserKey),
                SqlHelper.CreateInputSqlParameter("@MessageTime", SqlDbType.NVarChar, chatMessage.MessageTime, true)                
			};

            SqlHelper.ExecuteNonQuery(ConnectionString.Writing, CommandType.Text, sqlScript, parameters);
        }

        private static List<ChatInfo> GetRoomChatMessages(string roomKey)
        {
            string sqlScript = @"Select MessageKey, ChatContent, RoomKey, CM.UserKey, MessageTime, CU.UserName 
                                From ChatMessages CM 
                                Left Join ChatUser CU On CM.UserKey = CU.UserKey 
                                Where RoomKey = @RoomKey 
                                Order By MessageTime Asc";

            SqlParameter[] parameters = new SqlParameter[]
			{
				SqlHelper.CreateInputSqlParameter("@RoomKey", SqlDbType.NVarChar, roomKey)               
			};

            DataTable dt = SqlHelper.ExecuteDataset(ConnectionString.Writing, CommandType.Text, sqlScript, parameters).Tables[0];
            return dt.ToList<ChatInfo>();
        }
    }
}