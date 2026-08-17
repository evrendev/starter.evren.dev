using EvrenDev.Domain.Payments;

namespace EvrenDev.Application.Payments.Specifications;

public class PaymentOrderByPayPalOrderIdSpec : Specification<PaymentOrder>, ISingleResultSpecification<PaymentOrder>
{
    public PaymentOrderByPayPalOrderIdSpec(string payPalOrderId) =>
        Query.Where(o => o.PayPalOrderId == payPalOrderId);
}
