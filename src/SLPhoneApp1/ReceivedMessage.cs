using System;

namespace SLPhoneApp1
{
    public class ReceivedMessage
    {
        public ReceivedMessage(string text)
        {
            Text = text;
            ReceivedAt = DateTime.Now;
        }

        public string Text { get; private set; }

        public DateTime ReceivedAt { get; private set; }
    }
}