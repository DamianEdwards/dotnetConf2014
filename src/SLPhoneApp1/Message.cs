using System;

namespace SLPhoneApp1
{
    public class Message
    {
        public Message(string text, string userName = null)
        {
            UserName = userName;
            Text = text;
            ReceivedAt = DateTime.Now;
        }

        public string UserName { get; set; }

        public string Text { get; private set; }

        public DateTime ReceivedAt { get; private set; }
    }
}