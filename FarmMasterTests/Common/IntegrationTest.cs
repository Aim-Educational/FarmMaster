using DataAccess;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace FarmMasterTests.Common
{
    // Used as a proxy of Console.WriteLine -> ITestOutputHelper.WriteLine.
    // This is because *XUnit decides to gut Console.WriteLine otherwise*.
    class Converter : TextWriter
    {
        readonly ITestOutputHelper _output;
        public Converter(ITestOutputHelper output)
        {
            this._output = output;
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
        public override void WriteLine(string message)
        {
            this._output.WriteLine(message);
        }
        public override void WriteLine(string format, params object[] args)
        {
            this._output.WriteLine(format, args);
        }
    }

    /// <summary>
    /// The base class for every integration test.
    /// </summary>
    /// <remarks>
    /// This class will handle enabling console output (because xunit disables it); setting up a new TestServer;
    /// accessing the <see cref="FarmMasterContext"/>, and creating a <see cref="FarmClient"/>
    /// </remarks>
    public abstract class IntegrationTest : IDisposable
    {
        protected TestServer Host { private set; get; }
        protected FarmMasterContext Context { private set; get; }
        protected FarmClient Client { get; set; }

        public IntegrationTest(ITestOutputHelper output)
        {
            Console.SetOut(new Converter(output));

            this.Host = Common.TestHost; // Creates a new one each time.
            this.Client = new FarmClient(this.Host);
            this.Context = this.Host.Services.GetRequiredService<FarmMasterContext>();
        }

        public void Dispose()
        {
            try
            {
                this.Context.Database.EnsureDeleted();
                this.Host.Services.GetRequiredService<IdentityContext>().Database.EnsureDeleted();
            }
            catch (Exception) { }
            this.Host.Dispose();
        }
    }
}
