namespace E_Commerce.Domain.Common.Errors;

public static class ErrorCodes
{
    public static class Common
    {
        public const string Validation = "COM_400_VALIDATION";
        public const string NotFound = "COM_404_NOT_FOUND";
        public const string Conflict = "COM_409_CONFLICT";
        public const string Unexpected = "COM_500_UNEXPECTED";
        public const string PageInvalid = "COM_400_PAGE_INVALID";
        public const string PageSizeInvalid = "COM_400_PAGE_SIZE_INVALID";
    }

    public static class Category
    {
        public const string NotFound = "CAT_404";
        public const string ParentNotFound = "CAT_404_PARENT";
        public const string ParentSelf = "CAT_409_PARENT_SELF";
        public const string ParentCycle = "CAT_409_PARENT_CYCLE";
        public const string SlugDuplicate = "CAT_409_SLUG_EXISTS";
        public const string DeleteHasChildren = "CAT_409_HAS_CHILDREN";
        public const string DeleteHasProducts = "CAT_409_HAS_PRODUCTS";
        public const string InvalidInput = "CAT_400_INVALID";
        public const string IdRequired = "CAT_400_ID_REQUIRED";
        public const string ParentInvalid = "CAT_400_PARENT_INVALID";
        public const string SlugRequired = "CAT_400_SLUG_REQUIRED";
        public const string SortOrderInvalid = "CAT_400_SORTORDER_INVALID";
        public const string ChildRequired = "CAT_400_CHILD_REQUIRED";
        public const string ChildDuplicate = "CAT_409_CHILD_DUPLICATE";
    }

    public static class Product
    {
        public const string NotFound = "PRD_404";
        public const string SlugDuplicate = "PRD_409_SLUG_EXISTS";
        public const string HasVariants = "PRD_409_HAS_VARIANTS";
        public const string InvalidInput = "PRD_400_INVALID";
        public const string IdRequired = "PRD_400_ID_REQUIRED";
        public const string CategoryRequired = "PRD_400_CATEGORY_REQUIRED";
        public const string SlugRequired = "PRD_400_SLUG_REQUIRED";
        public const string StatusInvalid = "PRD_400_STATUS_INVALID";
        public const string BasePriceRequired = "PRD_400_PRICE_REQUIRED";
        public const string BasePriceInvalid = "PRD_400_PRICE_INVALID";
        public const string CurrencyRequired = "PRD_400_CURRENCY_REQUIRED";
        public const string CurrencyInvalid = "PRD_400_CURRENCY_INVALID";
        public const string BrandTooLong = "PRD_400_BRAND_TOO_LONG";
    }

    public static class Variant
    {
        public const string NotFound = "VAR_404";
        public const string SkuDuplicate = "VAR_409_SKU_EXISTS";
        public const string DeleteReferenced = "VAR_409_REFERENCED";
        public const string InvalidInput = "VAR_400_INVALID";
        public const string ProductRequired = "VAR_400_PRODUCT_REQUIRED";
        public const string VariantIdRequired = "VAR_400_ID_REQUIRED";
        public const string SkuRequired = "VAR_400_SKU_REQUIRED";
        public const string SkuTooLong = "VAR_400_SKU_TOO_LONG";
        public const string PriceInvalid = "VAR_400_PRICE_INVALID";
        public const string PriceRequired = "VAR_400_PRICE_REQUIRED";
        public const string CurrencyRequired = "VAR_400_CURRENCY_REQUIRED";
        public const string CurrencyInvalid = "VAR_400_CURRENCY_INVALID";
    }

    public static class User
    {
        public const string NotFound = "USR_404";
        public const string EmailAlreadyExists = "USR_409_EMAIL_EXISTS";
        public const string EmailRequired = "USR_400_EMAIL_REQUIRED";
        public const string EmailInvalid = "USR_400_EMAIL_INVALID";
        public const string EmailTooLong = "USR_400_EMAIL_TOO_LONG";
        public const string FirstNameRequired = "USR_400_FIRST_NAME_REQUIRED";
        public const string FirstNameTooShort = "USR_400_FIRST_NAME_TOO_SHORT";
        public const string FirstNameTooLong = "USR_400_FIRST_NAME_TOO_LONG";
        public const string LastNameRequired = "USR_400_LAST_NAME_REQUIRED";
        public const string LastNameTooShort = "USR_400_LAST_NAME_TOO_SHORT";
        public const string LastNameTooLong = "USR_400_LAST_NAME_TOO_LONG";
        public const string PhoneRequired = "USR_400_PHONE_REQUIRED";
        public const string PhoneTooLong = "USR_400_PHONE_TOO_LONG";
        public const string PhoneInvalid = "USR_400_PHONE_INVALID";
        public const string NameTooLong = "USR_400_NAME_TOO_LONG";
        public const string RoleInvalid = "USR_400_ROLE_INVALID";
    }

    public static class Auth
    {
        public const string InvalidCredentials = "AUTH_401_INVALID_CREDENTIALS";
        public const string InvalidToken = "AUTH_401_INVALID_TOKEN";
        public const string ExpiredToken = "AUTH_401_TOKEN_EXPIRED";
        public const string TwoFactorRequired = "AUTH_401_2FA_REQUIRED";
        public const string TwoFactorInvalid = "AUTH_401_2FA_INVALID";
        public const string TwoFactorSetupRequired = "AUTH_409_2FA_SETUP_REQUIRED";
        public const string InvalidRequest = "AUTH_400_INVALID_REQUEST";
        public const string PasswordInvalid = "AUTH_400_PASSWORD_INVALID";
        public const string PasswordRequired = "AUTH_400_PASSWORD_REQUIRED";
        public const string PasswordTooShort = "AUTH_400_PASSWORD_TOO_SHORT";
        public const string PasswordTooLong = "AUTH_400_PASSWORD_TOO_LONG";
        public const string PasswordUppercaseMissing = "AUTH_400_PASSWORD_UPPERCASE_MISSING";
        public const string PasswordLowercaseMissing = "AUTH_400_PASSWORD_LOWERCASE_MISSING";
        public const string PasswordDigitMissing = "AUTH_400_PASSWORD_DIGIT_MISSING";
        public const string PasswordSpecialCharacterMissing = "AUTH_400_PASSWORD_SPECIAL_MISSING";
        public const string PasswordWhitespaceInvalid = "AUTH_400_PASSWORD_WHITESPACE_INVALID";
        public const string EmailRequired = "AUTH_400_EMAIL_REQUIRED";
        public const string EmailInvalid = "AUTH_400_EMAIL_INVALID";
        public const string EmailTooLong = "AUTH_400_EMAIL_TOO_LONG";
        public const string TokenRequired = "AUTH_400_TOKEN_REQUIRED";
        public const string TokenExpired = "AUTH_401_TOKEN_EXPIRED";
    }

    public static class ValueObjects
    {
        public const string CurrencyRequired = "VO_400_CURRENCY_REQUIRED";
        public const string CurrencyInvalid = "VO_400_CURRENCY_INVALID";
        public const string EmailRequired = "VO_400_EMAIL_REQUIRED";
        public const string EmailInvalid = "VO_400_EMAIL_INVALID";
        public const string JsonInvalid = "VO_400_JSON_INVALID";
        public const string MoneyAmountInvalid = "VO_400_MONEY_AMOUNT_INVALID";
        public const string MoneyCurrencyMismatch = "VO_400_MONEY_CURRENCY_MISMATCH";
        public const string PasswordHashInvalid = "VO_400_PASSWORD_HASH_INVALID";
        public const string SlugRequired = "VO_400_SLUG_REQUIRED";
        public const string SlugInvalid = "VO_400_SLUG_INVALID";
        public const string TokenHashRequired = "VO_400_TOKEN_HASH_REQUIRED";
        public const string TokenHashInvalid = "VO_400_TOKEN_HASH_INVALID";
    }

    public static class Domain
    {
        public static class TwoFactor
        {
            public const string AlreadyDisabled = "TF_409_ALREADY_DISABLED";
            public const string AlreadyEnabled = "TF_409_ALREADY_ENABLED";
            public const string AlreadyVerified = "TF_409_ALREADY_VERIFIED";
            public const string ChallengeExpired = "TF_400_CHALLENGE_EXPIRED";
            public const string ChallengeMaxAttempts = "TF_400_CHALLENGE_MAX_ATTEMPTS";
            public const string NotEnabled = "TF_404_NOT_ENABLED";
            public const string NotSetup = "TF_404_NOT_SETUP";
            public const string RecoveryInvalid = "TF_400_RECOVERY_INVALID";
            public const string RecoveryUsed = "TF_409_RECOVERY_USED";
            public const string SecretRequired = "TF_400_SECRET_REQUIRED";
            public const string UserIdRequired = "TF_400_USER_ID_REQUIRED";
        }

        public static class AuthToken
        {
            public const string ExpiresAtRequired = "AT_400_EXPIRES_AT_REQUIRED";
            public const string TokenHashRequired = "AT_400_TOKEN_HASH_REQUIRED";
            public const string TokenTypeRequired = "AT_400_TOKEN_TYPE_REQUIRED";
            public const string UserIdRequired = "AT_400_USER_ID_REQUIRED";
        }

        public static class Cart
        {
            public const string AnonymousTokenRequired = "CART_400_ANONYMOUS_TOKEN_REQUIRED";
            public const string AnonymousTokenTooLong = "CART_400_ANONYMOUS_TOKEN_TOO_LONG";
            public const string ItemMismatch = "CART_409_ITEM_MISMATCH";
            public const string ItemNotFound = "CART_404_ITEM_NOT_FOUND";
            public const string ItemRequired = "CART_400_ITEM_REQUIRED";
            public const string NotActive = "CART_409_NOT_ACTIVE";
            public const string NowRequired = "CART_400_NOW_REQUIRED";
            public const string StatusInvalid = "CART_400_STATUS_INVALID";
            public const string UserIdRequired = "CART_400_USER_ID_REQUIRED";
        }

        public static class CartItem
        {
            public const string CartIdRequired = "CI_400_CART_ID_REQUIRED";
            public const string DeltaInvalid = "CI_400_DELTA_INVALID";
            public const string NowRequired = "CI_400_NOW_REQUIRED";
            public const string QuantityInvalid = "CI_400_QUANTITY_INVALID";
            public const string VariantIdRequired = "CI_400_VARIANT_ID_REQUIRED";
        }

        public static class Category
        {
            public const string ChildNotFound = "CAT_404_CHILD_NOT_FOUND";
            public const string ChildSelf = "CAT_409_CHILD_SELF";
            public const string ParentRequired = "CAT_400_PARENT_REQUIRED";
        }

        public static class EmailMessage
        {
            public const string BodyRequired = "EM_400_BODY_REQUIRED";
            public const string BodyTooLong = "EM_400_BODY_TOO_LONG";
            public const string ErrorRequired = "EM_400_ERROR_REQUIRED";
            public const string ErrorTooLong = "EM_400_ERROR_TOO_LONG";
            public const string ProviderRequired = "EM_400_PROVIDER_REQUIRED";
            public const string ProviderTooLong = "EM_400_PROVIDER_TOO_LONG";
            public const string SubjectRequired = "EM_400_SUBJECT_REQUIRED";
            public const string SubjectTooLong = "EM_400_SUBJECT_TOO_LONG";
            public const string TypeInvalid = "EM_400_TYPE_INVALID";
        }

        public static class OAuth
        {
            public const string AlreadyLinked = "OA_409_ALREADY_LINKED";
            public const string Duplicate = "OA_409_DUPLICATE";
            public const string EmailTooLong = "OA_400_EMAIL_TOO_LONG";
            public const string NotLinked = "OA_404_NOT_LINKED";
            public const string NowRequired = "OA_400_NOW_REQUIRED";
            public const string ProviderInvalid = "OA_400_PROVIDER_INVALID";
            public const string ProviderRequired = "OA_400_PROVIDER_REQUIRED";
            public const string ProviderTooLong = "OA_400_PROVIDER_TOO_LONG";
            public const string ProviderUserIdRequired = "OA_400_PROVIDER_USER_ID_REQUIRED";
            public const string ProviderUserIdTooLong = "OA_400_PROVIDER_USER_ID_TOO_LONG";
            public const string UserIdRequired = "OA_400_USER_ID_REQUIRED";
        }

        public static class Order
        {
            public const string BillingAddressRequired = "ORD_400_BILLING_ADDRESS_REQUIRED";
            public const string CancelNotAllowed = "ORD_409_CANCEL_NOT_ALLOWED";
            public const string CancelReasonTooLong = "ORD_400_CANCEL_REASON_TOO_LONG";
            public const string CurrencyRequired = "ORD_400_CURRENCY_REQUIRED";
            public const string DiscountInvalid = "ORD_400_DISCOUNT_INVALID";
            public const string ItemCurrencyMismatch = "ORD_409_ITEM_CURRENCY_MISMATCH";
            public const string ItemNotFound = "ORD_404_ITEM_NOT_FOUND";
            public const string ItemRequired = "ORD_400_ITEM_REQUIRED";
            public const string NotEditable = "ORD_409_NOT_EDITABLE";
            public const string NotesTooLong = "ORD_400_NOTES_TOO_LONG";
            public const string NowRequired = "ORD_400_NOW_REQUIRED";
            public const string NumberRequired = "ORD_400_NUMBER_REQUIRED";
            public const string NumberTooLong = "ORD_400_NUMBER_TOO_LONG";
            public const string PaymentMismatch = "ORD_409_PAYMENT_MISMATCH";
            public const string PaymentRequired = "ORD_400_PAYMENT_REQUIRED";
            public const string ShippingAddressRequired = "ORD_400_SHIPPING_ADDRESS_REQUIRED";
            public const string ShippingFeeInvalid = "ORD_400_SHIPPING_FEE_INVALID";
            public const string StatusInvalidTransition = "ORD_409_STATUS_INVALID_TRANSITION";
            public const string TaxInvalid = "ORD_400_TAX_INVALID";
            public const string TotalInvalid = "ORD_400_TOTAL_INVALID";
            public const string UserIdRequired = "ORD_400_USER_ID_REQUIRED";
        }

        public static class OrderItem
        {
            public const string CurrencyInvalid = "OI_400_CURRENCY_INVALID";
            public const string LineTotalInvalid = "OI_400_LINE_TOTAL_INVALID";
            public const string OrderIdRequired = "OI_400_ORDER_ID_REQUIRED";
            public const string QuantityInvalid = "OI_400_QUANTITY_INVALID";
            public const string SkuRequired = "OI_400_SKU_REQUIRED";
            public const string SkuTooLong = "OI_400_SKU_TOO_LONG";
            public const string TitleRequired = "OI_400_TITLE_REQUIRED";
            public const string TitleTooLong = "OI_400_TITLE_TOO_LONG";
            public const string UnitPriceInvalid = "OI_400_UNIT_PRICE_INVALID";
            public const string VariantIdRequired = "OI_400_VARIANT_ID_REQUIRED";
            public const string VariantSnapshotRequired = "OI_400_VARIANT_SNAPSHOT_REQUIRED";
        }

        public static class Payment
        {
            public const string AmountInvalid = "PAY_400_AMOUNT_INVALID";
            public const string CurrencyRequired = "PAY_400_CURRENCY_REQUIRED";
            public const string NowRequired = "PAY_400_NOW_REQUIRED";
            public const string OrderIdRequired = "PAY_400_ORDER_ID_REQUIRED";
            public const string ProviderRequired = "PAY_400_PROVIDER_REQUIRED";
            public const string ProviderTooLong = "PAY_400_PROVIDER_TOO_LONG";
            public const string ProviderPaymentIdRequired = "PAY_400_PROVIDER_PAYMENT_ID_REQUIRED";
            public const string ProviderPaymentIdTooLong = "PAY_400_PROVIDER_PAYMENT_ID_TOO_LONG";
            public const string RefundNotCaptured = "PAY_409_REFUND_NOT_CAPTURED";
            public const string StatusFinal = "PAY_409_STATUS_FINAL";
            public const string StatusInvalidTransition = "PAY_409_STATUS_INVALID_TRANSITION";
        }

        public static class Product
        {
            public const string ImageRequired = "PRD_400_IMAGE_REQUIRED";
            public const string NowRequired = "PRD_400_NOW_REQUIRED";
            public const string VariantNotFound = "PRD_404_VARIANT_NOT_FOUND";
            public const string VariantPriceOverrideCurrencyMismatch = "PRD_409_VARIANT_PRICE_OVERRIDE_CURRENCY_MISMATCH";
            public const string VariantPriceOverrideInvalid = "PRD_400_VARIANT_PRICE_OVERRIDE_INVALID";
            public const string VariantSkuDuplicate = "PRD_409_VARIANT_SKU_DUPLICATE";
            public const string VariantSkuRequired = "PRD_400_VARIANT_SKU_REQUIRED";
        }

        public static class Refresh
        {
            public const string DeviceInfoTooLong = "REF_400_DEVICE_INFO_TOO_LONG";
            public const string ExpiresAtInvalid = "REF_400_EXPIRES_AT_INVALID";
            public const string IpInvalid = "REF_400_IP_INVALID";
            public const string IpTooLong = "REF_400_IP_TOO_LONG";
            public const string NowRequired = "REF_400_NOW_REQUIRED";
            public const string ReplacedByRequired = "REF_400_REPLACED_BY_REQUIRED";
            public const string ReplacedBySame = "REF_409_REPLACED_BY_SAME";
            public const string TokenHashRequired = "REF_400_TOKEN_HASH_REQUIRED";
            public const string UserIdRequired = "REF_400_USER_ID_REQUIRED";
        }

        public static class Refund
        {
            public const string AmountInvalid = "RFD_400_AMOUNT_INVALID";
            public const string PaymentIdRequired = "RFD_400_PAYMENT_ID_REQUIRED";
            public const string ProviderRefundIdRequired = "RFD_400_PROVIDER_REFUND_ID_REQUIRED";
            public const string StatusFinal = "RFD_409_STATUS_FINAL";
        }

        public static class UserCredential
        {
            public const string PasswordHashInvalid = "UC_400_PASSWORD_HASH_INVALID";
            public const string PasswordHashRequired = "UC_400_PASSWORD_HASH_REQUIRED";
            public const string UserIdInvalid = "UC_400_USER_ID_INVALID";
        }

        public static class Variant
        {
            public const string ColorTooLong = "VAR_400_COLOR_TOO_LONG";
            public const string ImageRequired = "VAR_400_IMAGE_REQUIRED";
            public const string InventoryRequired = "VAR_400_INVENTORY_REQUIRED";
            public const string SizeTooLong = "VAR_400_SIZE_TOO_LONG";
        }

        public static class VariantImage
        {
            public const string SortOrderInvalid = "VI_400_SORT_ORDER_INVALID";
            public const string UrlEmpty = "VI_400_URL_EMPTY";
            public const string VariantIdEmpty = "VI_400_VARIANT_ID_EMPTY";
        }
    }

    public static class Infrastructure
    {
        public const string PersistenceFailure = "INF_500_PERSISTENCE";
        public const string DatabaseUnavailable = "INF_503_DB_UNAVAILABLE";
    }

    public static class External
    {
        public const string ServiceUnavailable = "EXT_503_SERVICE_UNAVAILABLE";
        public const string PaymentProviderUnavailable = "EXT_503_PAYMENT_PROVIDER";
        public const string EmailProviderUnavailable = "EXT_503_EMAIL_PROVIDER";
    }
}
