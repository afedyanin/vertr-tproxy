using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Client;
using Vertr.Tproxy.Server.Converters;

namespace Vertr.Tproxy.Server.Tests.Converters;

// https://visualrecode.com/blog/csharp-decimals-in-grpc/

[TestFixture(Category = "Unit")]
public class MoneyConverterTests
{
    [TestCase(320.48)]
    [TestCase(320)]
    [TestCase(0.045678)]
    public void CanConvertPriceToQuotation(decimal price)
    {
        Quotation quotation = price;
        decimal converted = quotation;
        Assert.That(converted, Is.EqualTo(price));
    }

    [TestCase(564.123)]
    [TestCase(0.0125)]
    [TestCase(785647)]
    public void CanConvertMoneyToMoneyValue(decimal price)
    {
        Money money = new Money("", price);
        MoneyValue moneyValue = money.Convert();
        decimal value = moneyValue;

        Assert.That(value, Is.EqualTo(price));
    }

    [TestCase(564.123)]
    [TestCase(0.0125)]
    [TestCase(785647)]
    public void CanConvertMoneyValueToMoney(decimal price)
    {
        Money money = new Money("", price);
        MoneyValue moneyValue = money.Convert();
        Money converted = moneyValue.Convert();
        decimal value = converted.Value;

        Assert.Multiple(() =>
        {
            Assert.That(converted, Is.EqualTo(money));
            Assert.That(value, Is.EqualTo(price));
        });
    }
}
