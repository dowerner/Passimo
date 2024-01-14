using Passimo.IO.Core.FileEncoding;
using Passimo.IO.Tests.Fixtures;

namespace Passimo.IO.Tests
{
    public class UnitTest1 : IClassFixture<DomainFixture>
    {
        private readonly DomainFixture _fixture;

        public UnitTest1(DomainFixture fixture) 
        {
            _fixture = fixture;
        }

        [Fact]
        public void Test1()
        {
            var profile = _fixture.PasswordProfileFactory.CreateDefault();

            var encoder = new PasswordProfileEncoder(profile);
            encoder.Encode();
        }
    }
}