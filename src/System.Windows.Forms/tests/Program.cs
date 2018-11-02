using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using HelloWorld;

namespace HelloWorld.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void ReturnTrueShouldReturn()
        {
            // act
            var result = Program.ReturnTrue();

            // assert
            Assert.True(result);
        }

        // test copied from https://github.com/AArnott/Xunit.StaFact/blob/2e33ac2b88603852a2d72f81be4ff1c188f8ea55/src/Xunit.StaFact.Tests/Samples.cs
        [UIFact]
        public async Task UIFact_OnSTAThread()
        {
            int initialThread = Environment.CurrentManagedThreadId;
            await Task.Yield();
            Assert.Equal(initialThread, Environment.CurrentManagedThreadId);
        }
    }
}