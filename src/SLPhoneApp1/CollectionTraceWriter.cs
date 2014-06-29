using System;
using System.Collections.Generic;
using System.IO;
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
            ActionExtensions.Dispatch(v => _collection.Add(v), value, _dispatcher);
        }

        public override void WriteLine(object value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(string format, object arg0)
        {
            ActionExtensions.Dispatch((f, a) => _collection.Add(string.Format(f, a)), format, arg0, _dispatcher);
        }

        public override void WriteLine(string format, object arg1, object arg2)
        {
            ActionExtensions.Dispatch((f, a1, a2) => _collection.Add(string.Format(f, a1, a2)), format, arg1, arg2, _dispatcher);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            ActionExtensions.Dispatch((f, a) => _collection.Add(string.Format(f, a)), format, arg, _dispatcher);
        }
    }
}
