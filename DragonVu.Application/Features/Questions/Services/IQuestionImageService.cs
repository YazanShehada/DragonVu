using Microsoft.AspNetCore.Http;

namespace DragonVu.Application.Features.Questions.Services;

/// <summary>
/// Stores and removes question images on disk.
/// </summary>
public interface IQuestionImageService
{
    bool IsSupported(string fileName);

    /// <summary>
    /// Saves the uploaded image as a compressed WebP and returns the relative URL to store in the database.
    /// </summary>
    Task<string> SaveAsync(IFormFile image, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a stored image, ignoring the shared default image.
    /// </summary>
    Task DeleteAsync(string? relativeUrl);

    void EnsureSubjectFolder(int subjectId);
}
