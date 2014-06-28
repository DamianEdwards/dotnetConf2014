using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace SLPhoneApp1
{
    public class CollectionTraceWriter : TextWriter
    {
        private readonly ICollection<string> _collection;
        private readonly Dispatcher _dispatcher;

        public CollectionTraceWriter(ICollection<string> collection, Dispatcher dispatcher)
        {
            _collection = collection;
            _dispatcher = dispatcher;
        }

        public override Encoding Encoding
        {
            get { return UTF8Encoding.UTF8; }
        }

        public override void WriteLine(string value)
        {
            SmartDispatch(() => _collection.Add(value));
        }

        public override void WriteLine(string format, object arg0)
        {
            base.WriteLine(format, arg0);
        }

        private void SmartDispatch(Action action)
        {
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _dispatcher.BeginInvoke(() => action());
            }
        }
    }
}
