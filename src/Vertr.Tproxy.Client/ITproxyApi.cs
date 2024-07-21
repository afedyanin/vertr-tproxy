using Refit;
using Vertr.Tproxy.Client.Instruments;
using Vertr.Tproxy.Client.Orders;

namespace Vertr.Tproxy.Client;
public interface ITproxyApi
{
    // Orders
    [Post("/api/orders/post")]
    Task<PostOrderResponse> PostOrder([Body] PostOrderRequest orderRequest);

    [Post("/api/orders/cancel")]
    Task<DateTime?> CancelOrder([Body] CancelOrderRequest orderRequest);

    [Get("/api/orders/state{orderId}")]
    Task<OrderStateResponse> GetOrderState(string orderId);

    [Get("/api/orders")]
    Task<OrderStateResponse[]> GetOrders();

    // Instruments
    [Get("/api/instruments/shares")]
    Task<Share[]> GetShares();

    [Get("/api/instruments/shares/{classcode}/{ticker}")]
    Task<Share> GetShare(string classcode, string ticker);

    // Market Data
}
