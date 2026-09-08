using Aggregation.Backend.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agregation.Backend.Infrastructure.Tests.Helpers
{
    public class MockOptions : IHttpClientOptions
    {
        public string BaseUrl { get; set; }
        public string ListUri { get; set; }
        public string ApiKey { get; set; }
    }
}
