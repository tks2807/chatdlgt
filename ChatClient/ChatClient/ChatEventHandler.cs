using ChatClient.Protos;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;


namespace ChatClient
{
    public class ChatEventHandler
    {
        public event EventHandler<ChatEventArgs> MessageGot;
        protected virtual void OnMessageRecieved(ChatEventArgs c)
        {
            MessageGot?.Invoke(this, c);
        }
        public void ChatEvent(AsyncDuplexStreamingCall<Message, Message> chat)
        {
            var response = chat.ResponseStream.Current;
            ChatEventArgs args = new ChatEventArgs
            {
                Date = DateTime.Now,
                Username = response.User,
            };
            OnMessageRecieved(args);
        }
        
    }

}

