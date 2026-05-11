using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Payment.Paymob
{
    public sealed record PaymobTransactionObject
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("pending")]
        public bool Pending { get; init; }

        [JsonPropertyName("amount_cents")]
        public long AmountCents { get; init; }

        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = default!;

        [JsonPropertyName("integration_id")]
        public long IntegrationId { get; init; }

        [JsonPropertyName("order")]
        public PaymobOrderObject? Order { get; init; }

        [JsonPropertyName("payment_key_claims")]
        public PaymobPaymentKeyClaims? PaymentKeyClaims { get; init; }

        [JsonPropertyName("is_auth")]
        public bool IsAuth { get; init; }

        [JsonPropertyName("is_capture")]
        public bool IsCapture { get; init; }

        [JsonPropertyName("is_standalone_payment")]
        public bool IsStandalonePayment { get; init; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; init; }

        [JsonPropertyName("is_refunded")]
        public bool IsRefunded { get; init; }

        [JsonPropertyName("is_void")]
        public bool IsVoid { get; init; }

        [JsonPropertyName("is_refund")]
        public bool IsRefund { get; init; }

        [JsonPropertyName("error_occured")]
        public bool ErrorOccurred { get; init; }

        [JsonPropertyName("source_data")]
        public PaymobSourceData? SourceData { get; init; }

        [JsonPropertyName("data")]
        public JsonElement Data { get; init; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? Extra { get; init; }
    }
    public sealed record PaymobOrderObject
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("amount_cents")]
        public long AmountCents { get; init; }

        [JsonPropertyName("paid_amount_cents")]
        public long PaidAmountCents { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = default!;

        [JsonPropertyName("merchant_order_id")]
        public string? MerchantOrderId { get; init; }

        [JsonPropertyName("payment_status")]
        public string? PaymentStatus { get; init; }
    }
    public sealed record PaymobPaymentKeyClaims
    {
        [JsonPropertyName("order_id")]
        public long OrderId { get; init; }

        [JsonPropertyName("amount_cents")]
        public long AmountCents { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = default!;

        [JsonPropertyName("integration_id")]
        public long IntegrationId { get; init; }

        [JsonPropertyName("extra")]
        public PaymobExtra? Extra { get; init; }

        [JsonPropertyName("next_payment_intention")]
        public string? NextPaymentIntention { get; init; }
    }
    public sealed record PaymobSourceData
    {
        [JsonPropertyName("pan")]
        public string? PanLast4 { get; init; }

        [JsonPropertyName("type")]
        public string? Type { get; init; }

        [JsonPropertyName("sub_type")]
        public string? SubType { get; init; }
    }
    public sealed record PaymobExtra
    {
        [JsonPropertyName("merchant_order_id")]
        public string? MerchantOrderId { get; init; }
    }
    public sealed record PaymobSessionRequest
    {
        public decimal amount => items.Sum(i => i.Amount);
        public string currency { get; set; } = "EGP";
        public required IReadOnlyCollection<int> payment_methods { get; set; }
        public required IReadOnlyCollection<PaymentSessionItemDto> items { get; set; }
        public PaymentBillingDataDto billing_data { get; set; } = default!;
        //public string redirection_url { get; set; } = default!;
        //public string notification_url { get; set; } = default!;
        public string special_reference { get; set; } = default!;
    }
    public sealed record PaymobPaymentIntentionResponse
    {
        [JsonPropertyName("payment_keys")]
        public List<PaymobPaymentKey> PaymentKeys { get; init; } = new();

        [JsonPropertyName("intention_order_id")]
        public string IntentionOrderId { get; init; } = default!;

        [JsonPropertyName("split_payment_methods")]
        public List<object> SplitPaymentMethods { get; init; } = new();

        [JsonPropertyName("id")]
        public string Id { get; init; } = default!;

        [JsonPropertyName("intention_detail")]
        public PaymobIntentionDetail IntentionDetail { get; init; } = default!;

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; init; } = default!;

        [JsonPropertyName("payment_methods")]
        public List<PaymobPaymentMethod> PaymentMethods { get; init; } = new();

        [JsonPropertyName("special_reference")]
        public string? SpecialReference { get; init; }

        [JsonPropertyName("extras")]
        public PaymobExtras Extras { get; init; } = default!;

        [JsonPropertyName("confirmed")]
        public bool Confirmed { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; } = default!;

        [JsonPropertyName("created")]
        public DateTime Created { get; init; }

        [JsonPropertyName("card_detail")]
        public JsonElement? CardDetail { get; init; }

        [JsonPropertyName("card_tokens")]
        public List<object> CardTokens { get; init; } = new();

        [JsonPropertyName("object")]
        public string Object { get; init; } = default!;
    }
    public sealed record PaymobPaymentKey
    {
        [JsonPropertyName("integration")]
        public long Integration { get; init; }

        [JsonPropertyName("key")]
        public string Key { get; init; } = default!;

        [JsonPropertyName("gateway_type")]
        public string GatewayType { get; init; } = default!;

        [JsonPropertyName("iframe_id")]
        public string? IframeId { get; init; }

        [JsonPropertyName("order_id")]
        public long OrderId { get; init; }

        [JsonPropertyName("redirection_url")]
        public string RedirectionUrl { get; init; } = default!;

        [JsonPropertyName("save_card")]
        public bool SaveCard { get; init; }
    }
    public sealed record PaymobIntentionDetail
    {
        [JsonPropertyName("amount")]
        public long Amount { get; init; }

        [JsonPropertyName("items")]
        public List<PaymobItem> Items { get; init; } = new();

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = default!;

        [JsonPropertyName("billing_data")]
        public PaymobBillingData BillingData { get; init; } = default!;
    }
    public sealed record PaymobItem
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = default!;

        [JsonPropertyName("amount")]
        public long Amount { get; init; }

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; init; }

        [JsonPropertyName("image")]
        public string? Image { get; init; }
    }
    public sealed record PaymobPaymentMethod
    {
        [JsonPropertyName("integration_id")]
        public long IntegrationId { get; init; }

        [JsonPropertyName("alias")]
        public string? Alias { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("method_type")]
        public string MethodType { get; init; } = default!;

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = default!;

        [JsonPropertyName("live")]
        public bool Live { get; init; }

        [JsonPropertyName("use_cvc_with_moto")]
        public bool UseCvcWithMoto { get; init; }
    }
    public sealed record PaymobExtras
    {
        [JsonPropertyName("creation_extras")]
        public JsonElement? CreationExtras { get; init; }

        [JsonPropertyName("confirmation_extras")]
        public JsonElement? ConfirmationExtras { get; init; }
    }
    public sealed record PaymobBillingData : PaymentBillingDataDto
    {
        public PaymobBillingData(string FirstName, string LastName, string Email, string PhoneNumber, string Street, string Building, string Floor, string Apartment, string City, string State, string Country, string PostalCode) : base(FirstName, LastName, Email, PhoneNumber, Street, Building, Floor, Apartment, City, State, Country, PostalCode)
        {
        }
    }
}
