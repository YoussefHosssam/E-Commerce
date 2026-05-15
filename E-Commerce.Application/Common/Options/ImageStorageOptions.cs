namespace E_Commerce.Application.Common.Options;

public sealed class ImageStorageOptions
{
    public string CloudName { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string ApiSecret { get; init; } = string.Empty;
    public string UploadFolderRoot { get; init; } = "ecommerce";
    public long MaxImageBytes { get; init; } = 5 * 1024 * 1024;
    public string[] AllowedFormats { get; init; } = ["jpg", "jpeg", "png", "webp"];
    public string[] AllowedContentTypes { get; init; } = ["image/jpeg", "image/png", "image/webp"];
    public int MinWidth { get; init; } = 100;
    public int MinHeight { get; init; } = 100;
    public int MaxWidth { get; init; } = 5000;
    public int MaxHeight { get; init; } = 5000;
}
