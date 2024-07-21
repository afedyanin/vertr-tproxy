using Refit;

namespace Vertr.Tproxy.Client.Tests;

[TestFixture(Category = "Intergration", Explicit = true)]
public class TproxyApiTests
{
    [Test]
    public async Task CanGetInstrument()
    {
        var api = RestService.For<ITproxyApi>("http://localhost:5100");

        var instrument = await api.GetShare("TQBR", "SBER");

        Assert.That(instrument, Is.Not.Null);
    }
}
