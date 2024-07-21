namespace Vertr.Tproxy.Client.Operations;

public record class GetOperationsResponse(bool HasNext, string? NextCursor, IEnumerable<OperationItem> Items);

public record class OperationItem(
    string OperationId,
    string ParentOperationId,
    string Name,
    DateTime Timestamp,
    OperationType OperationType,
    string InstrumentId,
    string Description,
    string InstrumentType,
    InstrumentType InstrumentKind,
    OperationState State,
    Money Payment,
    Money Price,
    Money Comission,
    Money Yield,
    decimal YieldRelative,
    Money AccuredInt,
    long Qty,
    long QtyRest,
    long QtyDone,
    DateTime CancelTime,
    string CancelReason,
    IEnumerable<OperationItemTrade> Trades
    );

public record class OperationItemTrade(
    string Num,
    DateTime Timestamp,
    long Qty,
    Money Price,
    Money Yield,
    decimal YieldRelative
    );

public enum InstrumentType
{
    Unspecified = 0,
    Bond = 1, //Облигация.
    Share = 2, //Акция.
    Currency = 3, //Валюта.
    Etf = 4, //Exchange-traded fund. Фонд.
    Futures = 5, //Фьючерс.
    Sp = 6, //Структурная нота.
    Option = 7, //Опцион.
    ClearingCertificate = 8, //Clearing certificate.
    Index = 9, //Индекс.
    Commodity = 10 //Товар.
}

public enum OperationState
{
    Unspecified = 0,
    Executed = 1,
    Canceled = 2,
    Progress = 3,
}

public enum OperationType
{
    Unspecified = 0,
    Input = 1,
    BondTax = 2,
    OutputSecurities = 3,
    Overnight = 4,
    Tax = 5,
    BondRepaymentFull = 6,
    SellCard = 7,
    DividendTax = 8,
    Output = 9,
    BondRepayment = 10,
    TaxCorrection = 11,
    ServiceFee = 12,
    BenefitTax = 13,
    MarginFee = 14,
    Buy = 15,
    BuyCard = 16,
    InputSecurities = 17,
    SellMargin = 18,
    BrokerFee = 19,
    BuyMargin = 20,
    Dividend = 21,
    Sell = 22,
    Coupon = 23,
    SuccessFee = 24,
    DividendTransfer = 25,
    AccruingVarmargin = 26,
    WritingOffVarmargin = 27,
    DeliveryBuy = 28,
    DeliverySell = 29,
    TrackMfee = 30,
    TrackPfee = 31,
    TaxProgressive = 32,
    BondTaxProgressive = 33,
    DividendTaxProgressive = 34,
    BenefitTaxProgressive = 35,
    TaxCorrectionProgressive = 36,
    TaxRepoProgressive = 37,
    TaxRepo = 38,
    TaxRepoHold = 39,
    TaxRepoRefund = 40,
    TaxRepoHoldProgressive = 41,
    TaxRepoRefundProgressive = 42,
    DivExt = 43,
    TaxCorrectionCoupon = 44,
    CashFee = 45,
    OutFee = 46,
    OutStampDuty = 47,
    OutputSwift = 50,
    InputSwift = 51,
    OutputAcquiring = 53,
    InputAcquiring = 54,
    OutputPenalty = 55,
    AdviceFee = 56,
    TransIisBs = 57,
    TransBsBs = 58,
    OutMulti = 59,
    InpMulti = 60,
    OverPlacement = 61,
    OverCom = 62,
    OverIncome = 63,
    OptionExpiration = 64,
    FutureExpiration = 65
}
