using System;
using Windows.Globalization.DateTimeFormatting;

namespace SignalRChat
{
    public class Message
    {
        private readonly DateTimeFormatter _dateTimeFormatter = new DateTimeFormatter("hour minute second");

        public Message(string text, string userName = null)
        {
            UserName = userName;
            Text = text;
            ReceivedAt = DateTime.Now;
        }

        public string UserName { get; set; }

        public string Text { get; private set; }

        public DateTime ReceivedAt { get; private set; }

        public string ReceivedAtFormatted
        {
            get { return _dateTimeFormatter.Format(ReceivedAt); }
        }
    }
}