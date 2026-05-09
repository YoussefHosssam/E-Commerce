namespace E_Commerce.Domain.Entities;

public enum PaymentAttemptStatus
{
    Initiated = 1,
    AwaitingCustomerAction = 2,
    Paid = 3,
    Failed = 4,
    Expired = 5
}