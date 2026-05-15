using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.ImageUploads.ProductImages;

public sealed class GenerateProductImageUploadSignatureValidation : AbstractValidator<GenerateProductImageUploadSignatureCommand>
{
    public GenerateProductImageUploadSignatureValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(ProductErrors.IdRequired);
        RuleFor(x => x.ContentType).NotEmpty().WithError(ImageUploadErrors.ContentTypeInvalid);
        RuleFor(x => x.SizeInBytes).GreaterThan(0).WithError(ImageUploadErrors.SizeInvalid);
    }
}

public sealed class CompleteProductImageUploadValidation : AbstractValidator<CompleteProductImageUploadCommand>
{
    public CompleteProductImageUploadValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(ProductErrors.IdRequired);
        RuleFor(x => x.StorageKey).NotEmpty().WithError(ImageUploadErrors.StorageKeyInvalid);
    }
}

public sealed class DeleteProductImageValidation : AbstractValidator<DeleteProductImageCommand>
{
    public DeleteProductImageValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(ProductErrors.IdRequired);
        RuleFor(x => x.ImageId).NotEmpty().WithError(ProductImageErrors.ImageNotFound);
    }
}

public sealed class SetPrimaryProductImageValidation : AbstractValidator<SetPrimaryProductImageCommand>
{
    public SetPrimaryProductImageValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(ProductErrors.IdRequired);
        RuleFor(x => x.ImageId).NotEmpty().WithError(ProductImageErrors.ImageNotFound);
    }
}

public sealed class ReorderProductImagesValidation : AbstractValidator<ReorderProductImagesCommand>
{
    public ReorderProductImagesValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(ProductErrors.IdRequired);
        RuleFor(x => x.SortOrders).NotEmpty().WithError(ProductImageErrors.ImageRequired);
        RuleForEach(x => x.SortOrders.Values).GreaterThan(0).WithError(ProductImageErrors.SortOrderInvalid);
    }
}
