<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chating.aspx.cs" Inherits="Chat.Instance.Chating" %>
<%@ Register Assembly="University.Mooc.Editor" Namespace="University.Mooc.Editor.UEditor" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/chating.css" rel="stylesheet" />
    <script lang="javascript" type="text/javascript" src="Scripts/jquery-1.6.4.min.js"></script>
    <script lang="javascript" type="text/javascript" src="Scripts/jquery.signalR-2.1.1.min.js"></script>
    <script lang="javascript" type="text/javascript" src="/signalr/hubs"></script>

    <script lang="javascript" type="text/javascript">
        $(function () {            
            var chatHub = $.connection.chat;
            $.connection.hub.qs = GetSearch();

            InitialChat(chatHub);
            registerEvents(chatHub);

            chatHub.client.messageReceived = function (message, who, when) {
                AddMessageItem(message, who, when);
            }

            $.connection.hub.start().done(function () {                
                chatHub.server.initialChat();
            }).fail(function (e) {
                //alert(e);
            });            
        });

        // initial chat history messages
        function InitialChat(chatHub)
        {            
            chatHub.client.recieveMessages = function (messages) {
                for (i = 0; i < messages.length; i++) {
                    AddMessageItem(messages[i].ChatContent, messages[i].UserName, messages[i].MessageTime);
                }
            };
        }

        // add message to client
        function AddMessageItem(message, who, when)
        {
            $('#chatmessage').append("<li class=\"chat-info\">"
                                    + "<div class=\"chat-detail-content\">" + message + "</div>"
                                    + "<div class=\"chat-detail-senderinfo\">"
                                    + "<span class=\"chat-user\">" + who + "</span>"
                                    + "<span class=\"chat-time\">" + when + "</span>"
                                    + "</div>"
                                    + "</li>"
                                );

            // single or doublle line color
            $(".chat-details li:odd").css("background-color", "#bfe7ee");
            $(".chat-details li:even").css("background-color", "#faf6d6");
            // to the bottom of element
            var height = $('.chat-details')[0].scrollHeight;
            $('.chat-details').scrollTop(height);
        }

        // register events
        function registerEvents(chatHub) {
            $('#btnSend').click(function () {
                sendEvent(chatHub);
            });
            $("#txtContent").keypress(function (e) {
                if (e.which == 13) {
                    sendEvent(chatHub);
                }
            });
        }

        function sendEvent(chatHub)
        {
            var msg = $.trim($("#txtContent").val());
            if (msg.length > 0) {
                chatHub.server.sendMessage(msg);
                $("#txtContent").val('');
            }
        }

        // with encode model url parameter
        function GetSearch() {
            var search = window.location.search; 
            var querystring = "";

            if (search.indexOf("?") != -1) {
                var parameters = search.substr(1).split("&");
                
                $.each(parameters, function (k, p) {
                    var e = p.split("=");
                    querystring += e[0] + "=" + encodeURIComponent(e[1]) + "&";
                });

                if (parameters.length > 0)
                {
                    querystring = querystring.substr(0, querystring.length - 1);
                }
            }
            return querystring;
        }
    </script>
    
</head>
<body>
    <form id="form1" runat="server" onsubmit="return false;">
        <div class="chat-container">
            <div class="chat-details">
                <ul id="chatmessage">
                </ul>
            </div>
            <div class="chat-active">
                <input type="text" id="txtContent" class="chat-content" />
                <input type="button" id="btnSend" value="Send" class="chat-send-event" />
            </div>
        </div>
    </form>
</body>
</html>
