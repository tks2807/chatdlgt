using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models
{
    public class History
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
    }
}
