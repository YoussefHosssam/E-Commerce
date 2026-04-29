namespace E_Commerce.Domain.Common.Errors;

public static class DomainCategoryErrors
{
    public static readonly Error ChildNotFound = new(ErrorCodes.Domain.Category.ChildNotFound, "Child category not found.", ErrorType.NotFound);
    public static readonly Error ChildSelf = new(ErrorCodes.Domain.Category.ChildSelf, "A category cannot be its own child.", ErrorType.Conflict);
    public static readonly Error ParentRequired = new(ErrorCodes.Domain.Category.ParentRequired, "Parent category is required.", ErrorType.Validation);
}
