using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace SignalRChat
{
    public class CollectionTraceWriter : TextWriter
    {
        private readonly ICollection<string> _collection;
        private readonly CoreDispatcher _dispatcher;

        public CollectionTraceWriter(ICollection<string> collection, CoreDispatcher dispatcher)
        {
            _collection = collection;
            _dispatcher = dispatcher;
        }

        public override Encoding Encoding
        {
            get { return UTF8Encoding.UTF8; }
        }

        public override void WriteLine(object value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(string value)
        {
            ActionExtensions.Dispatch(v => _collection.Add(v), value, _dispatcher).Forget();
        }

        public override void WriteLine(string format, params object[] arg)
        {
            ActionExtensions.Dispatch((f, a) => _collection.Add(string.Format(f, a)), format, arg, _dispatcher).Forget();
        }

        public override void Write(char value)
        {
            Write(value.ToString());
        }
    }
}
