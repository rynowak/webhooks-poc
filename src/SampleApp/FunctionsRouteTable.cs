using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace SampleApp
{
    public class FunctionsRouteTable : IActionDescriptorChangeProvider
    {
        private readonly object _lock;

        private IChangeToken _token;
        private CancellationTokenSource _cts;

        private Entry[] _entries;

        public FunctionsRouteTable()
        {
            _lock = new object();

            _cts = new CancellationTokenSource();
            _token = new CancellationChangeToken(_cts.Token);

            _entries = Array.Empty<Entry>();
        }

        public IChangeToken GetChangeToken()
        {
            lock (_lock)
            {
                return _token;
            }
        }

        public IReadOnlyList<Entry> GetEntries()
        {
            lock (_lock)
            {
                return _entries;
            }
        }

        public void Update(IEnumerable<Entry> entries)
        {
            lock (_lock)
            {
                var old = _cts;

                // You need to swap the tokens before triggering the change
                _cts = new CancellationTokenSource();
                _token = new CancellationChangeToken(_cts.Token);

                _entries = entries.ToArray();

                old.Cancel();
                old.Dispose();
            }
        }

        public class Entry
        {
            public Entry(string template, string reciever, Func<HttpContext, Task> execute)
            {
                Template = template;
                Reciever = reciever;
                Execute = execute;
            }

            public string Template { get; }
            public string Reciever { get; }
            public Func<HttpContext, Task> Execute { get; }
        }
    }
}
