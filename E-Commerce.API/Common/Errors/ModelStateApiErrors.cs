using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.API.Common.Errors
{
    public static class ModelStateApiErrors
    {
        public static readonly Error UnsupportedFormat =
            new("MODST_400_INVALID_JSON",
            "Api only supports JSON data format.",
            ErrorType.Validation);

        public static readonly Error ValidationError =
            new("MODST_400_VALIDATION",
                "Validation failed.",
                ErrorType.Validation
                );
    }
}
