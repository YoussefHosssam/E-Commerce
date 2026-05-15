using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.ImageUploads.VariantImages;

public sealed class GenerateVariantImageUploadSignatureValidation : AbstractValidator<GenerateVariantImageUploadSignatureCommand>
{
    public GenerateVariantImageUploadSignatureValidation()
    {
        RuleFor(x => x.VariantId).NotEmpty().WithError(VariantErrors.VariantIdRequired);
        RuleFor(x => x.ContentType).NotEmpty().WithError(ImageUploadErrors.ContentTypeInvalid);
        RuleFor(x => x.SizeInBytes).GreaterThan(0).WithError(ImageUploadErrors.SizeInvalid);
    }
}

public sealed class CompleteVariantImageUploadValidation : AbstractValidator<CompleteVariantImageUploadCommand>
{
    public CompleteVariantImageUploadValidation()
    {
        RuleFor(x => x.VariantId).NotEmpty().WithError(VariantErrors.VariantIdRequired);
        RuleFor(x => x.StorageKey).NotEmpty().WithError(ImageUploadErrors.StorageKeyInvalid);
    }
}

public sealed class DeleteVariantImageValidation : AbstractValidator<DeleteVariantImageCommand>
{
    public DeleteVariantImageValidation()
    {
        RuleFor(x => x.VariantId).NotEmpty().WithError(VariantErrors.VariantIdRequired);
        RuleFor(x => x.ImageId).NotEmpty().WithError(VariantImageErrors.ImageNotFound);
    }
}

public sealed class SetPrimaryVariantImageValidation : AbstractValidator<SetPrimaryVariantImageCommand>
{
    public SetPrimaryVariantImageValidation()
    {
        RuleFor(x => x.VariantId).NotEmpty().WithError(VariantErrors.VariantIdRequired);
        RuleFor(x => x.ImageId).NotEmpty().WithError(VariantImageErrors.ImageNotFound);
    }
}

public sealed class ReorderVariantImagesValidation : AbstractValidator<ReorderVariantImagesCommand>
{
    public ReorderVariantImagesValidation()
    {
        RuleFor(x => x.VariantId).NotEmpty().WithError(VariantErrors.VariantIdRequired);
        RuleFor(x => x.SortOrders).NotEmpty().WithError(VariantImageErrors.ImageRequired);
        RuleForEach(x => x.SortOrders.Values).GreaterThan(0).WithError(VariantImageErrors.SortOrderInvalid);
    }
}
