using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    public class ChatEventArgs : EventArgs
    {
        public DateTime? Date { get; set; }
        public string Username { get; set; }

    }
}
