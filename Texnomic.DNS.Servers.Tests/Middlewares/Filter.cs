using Destructurama;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Threading.Tasks;
using Texnomic.DNS.Servers.Middlewares;
using Texnomic.FilterLists.Enums;

namespace Texnomic.DNS.Servers.Tests.Middlewares
{
    [TestClass]
    public class Filter
    {
        //private Tags[] FilterTags;
        //private FilterListsMiddleware FilterMiddleware;

        //[TestInitialize]
        //public void Initialize()
        //{
        //    Log.Logger = new LoggerConfiguration()
        //                   .Destructure.UsingAttributes()
        //                   .WriteTo.Seq("http://127.0.0.1:5341", compact: true)
        //                   .CreateLogger();

        //    FilterTags = new Tags[]
        //    {
        //        Tags.Malware,
        //        Tags.Phishing,
        //        Tags.Crypto,
        //    };

        //    FilterMiddleware = new FilterListsMiddleware(FilterTags, Log.Logger);
        //}

        //[TestMethod]
        //public async Task InitializeAsync()
        //{
        //    await FilterMiddleware.InitializeAsync(FilterTags);
        //}
    }
}
