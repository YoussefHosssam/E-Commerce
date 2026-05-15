namespace E_Commerce.Domain.Common.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = new(ErrorCodes.Category.NotFound, "Category not found.", ErrorType.NotFound);
    public static readonly Error ParentNotFound = new(ErrorCodes.Category.ParentNotFound, "Parent category not found.", ErrorType.NotFound);
    public static readonly Error ParentSelf = new(ErrorCodes.Category.ParentSelf, "A category cannot be its own parent.", ErrorType.Conflict);
    public static readonly Error ParentCycle = new(ErrorCodes.Category.ParentCycle, "The selected parent category would create a cycle.", ErrorType.Conflict);
    public static readonly Error SlugAlreadyExists = new(ErrorCodes.Category.SlugDuplicate, "Category slug already exists.", ErrorType.Conflict);
    public static readonly Error DeleteHasChildren = new(ErrorCodes.Category.DeleteHasChildren, "Category cannot be deleted while it has child categories.", ErrorType.Conflict);
    public static readonly Error DeleteHasProducts = new(ErrorCodes.Category.DeleteHasProducts, "Category cannot be deleted while it has products.", ErrorType.Conflict);
    public static readonly Error InvalidInput = new(ErrorCodes.Category.InvalidInput, "Category data is invalid.", ErrorType.Validation);
    public static readonly Error IdRequired = new(ErrorCodes.Category.IdRequired, "Category id is required.", ErrorType.Validation);
    public static readonly Error ParentInvalid = new(ErrorCodes.Category.ParentInvalid, "Parent category is invalid.", ErrorType.Validation);
    public static readonly Error SlugRequired = new(ErrorCodes.Category.SlugRequired, "Slug is required.", ErrorType.Validation);
    public static readonly Error SortOrderInvalid = new(ErrorCodes.Category.SortOrderInvalid, "Sort order cannot be negative.", ErrorType.Validation);
    public static readonly Error ChildRequired = new(ErrorCodes.Category.ChildRequired, "Child category is required.", ErrorType.Validation);
    public static readonly Error ChildDuplicate = new(ErrorCodes.Category.ChildDuplicate, "Child category already exists.", ErrorType.Conflict);
    public static readonly Error SlugDuplicate = new(ErrorCodes.Category.SlugDuplicate, "Slug already exists.", ErrorType.Conflict);
    public static readonly Error NameAlreadyExsit = new(ErrorCodes.Category.NameAlreadyExist, "Name already exists.", ErrorType.Conflict);
    public static readonly Error ChildNotFound = new(ErrorCodes.Category.ChildNotFound, "Child category not found.", ErrorType.NotFound);
    public static readonly Error ChildSelf = new(ErrorCodes.Category.ChildSelf, "A category cannot be its own child.", ErrorType.Conflict);
    public static readonly Error ParentRequired = new(ErrorCodes.Category.ParentRequired, "Parent category is required.", ErrorType.Validation);
    public static readonly Error InvalidName = new(ErrorCodes.Category.InvalidName, "Category name is invalid.", ErrorType.Validation);


}
