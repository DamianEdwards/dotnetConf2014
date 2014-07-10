using System;
#if !SILVERLIGHT
using Windows.Globalization.DateTimeFormatting;
#endif

namespace SignalRChat
{
    public class Message
    {
#if !SILVERLIGHT
        private readonly DateTimeFormatter _dateTimeFormatter = new DateTimeFormatter("hour minute second");
#endif

        public Message(string text, string userName = null)
        {
            UserName = userName;
            Text = text;
            ReceivedAt = DateTime.Now;
        }

        public string UserName { get; set; }

        public string Text { get; private set; }

        public DateTime ReceivedAt { get; private set; }

#if !SILVERLIGHT
        public string ReceivedAtFormatted
        {
            get
            {
                return _dateTimeFormatter.Format(ReceivedAt);
            }
        }
#endif
    }
}