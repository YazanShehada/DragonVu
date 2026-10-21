using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DragonVu.Application.Features.Questions.Services;

public sealed class QuestionImageService : IQuestionImageService
{
    public const string DefaultImageUrl = "images/default-question.png";

    private const string ResizeGeometry = "1000x1000>";
    private const uint WebpQuality = 75;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IWebHostEnvironment _environment;

    public QuestionImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public bool IsSupported(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    public Task<string> SaveAsync(
        IFormFile image,
        int subjectId,
        CancellationToken cancellationToken = default)
    {
        if (!IsSupported(image.FileName))
        {
            throw new InvalidImageFormatException(image.FileName);
        }

        var folderPath = GetSubjectFolder(subjectId);
        Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid()}.webp";
        var filePath = Path.Combine(folderPath, fileName);

        using var input = image.OpenReadStream();
        using var magick = new MagickImage(input);

        // Shrink to fit 1000x1000 while keeping the aspect ratio; never upscale.
        magick.Resize(new MagickGeometry(ResizeGeometry));
        magick.Quality = WebpQuality;
        magick.Format = MagickFormat.WebP;
        magick.Write(filePath);

        return Task.FromResult($"images/Questions/subjects/{subjectId}/{fileName}");
    }

    public Task DeleteAsync(string? relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl)
            || string.Equals(relativeUrl, DefaultImageUrl, StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        var fullPath = Path.Combine(_environment.WebRootPath ?? string.Empty, relativeUrl);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public void EnsureSubjectFolder(int subjectId)
    {
        Directory.CreateDirectory(GetSubjectFolder(subjectId));
    }

    private string GetSubjectFolder(int subjectId) =>
        Path.Combine(
            _environment.WebRootPath ?? string.Empty,
            "images",
            "Questions",
            "subjects",
            subjectId.ToString());
}

public sealed class InvalidImageFormatException(string fileName)
    : Exception($"The image format of '{fileName}' is not supported.");
