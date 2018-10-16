using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Windows.Forms.Tests
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

        // test copied from https://github.com/AArnott/Xunit.StaFact
        [StaFact]
        public async Task WpfFact_OnSTAThread()
        {
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
            await Task.Yield();
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        }
    }
}