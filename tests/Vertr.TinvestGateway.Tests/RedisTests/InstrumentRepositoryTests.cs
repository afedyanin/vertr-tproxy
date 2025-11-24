using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.DataAccess.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests
{
    public class InstrumentRepositoryTests
    {
        private IConnectionMultiplexer _connectionMultiplexer;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _connectionMultiplexer.Close();
        }

        [Test]
        public async Task CanSaveInstrument()
        {
            var repo = new InstrumentRepository(_connectionMultiplexer);

            var id = Guid.NewGuid();

            var instrument = new Instrument
            {
                ClassCode = "ClassCode",
                Ticker = "Ticker",
                Currency = "RUB",
                Id = id,
                InstrumentType = "SomeType",
                LotSize = 10,
                Name = "Some instrument name"
            };

            await repo.Save(instrument);

            var saved = await repo.Get(id);
            Assert.That(saved, Is.Not.Null);

            Console.WriteLine(saved);
        }

        [Test]
        public async Task CanGetInstrumentByTicker()
        {
            var repo = new InstrumentRepository(_connectionMultiplexer);

            var item = await repo.GetByTicker("Ticker");

            Assert.That(item, Is.Not.Null);
            Console.WriteLine(item);
        }
    }
}
