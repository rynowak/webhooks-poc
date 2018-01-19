using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace SampleApp
{
    public class FunctionsRouteTable : IActionDescriptorChangeProvider
    {
        private readonly object _lock = new object();

        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Entry[] _entries = Array.Empty<Entry>();
        private IChangeToken _token;

        public FunctionsRouteTable()
        {
            _token = new CancellationChangeToken(_cts.Token);
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
            public Entry(string template, string reciever, string id, Func<HttpContext, Task<IActionResult>> execute)
            {
                Template = template;
                Reciever = reciever;
                Id = id;
                Execute = execute;
            }

            public string Template { get; }

            public string Reciever { get; }

            // !!! Always null in this sample. Would be used in real life.
            public string Id { get; }

            // !!! Is this the correct signature for Web Jobs? Matches the user function.
            public Func<HttpContext, Task<IActionResult>> Execute { get; }
        }
    }
}
