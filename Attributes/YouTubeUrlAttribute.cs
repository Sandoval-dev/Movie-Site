using System.ComponentModel.DataAnnotations;

public class YouTubeUrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        var url = value.ToString();

        if (string.IsNullOrWhiteSpace(url))
            return ValidationResult.Success; // opsiyonel alan gibi davranır, Required ile birlikte kullanırsan bu gerekmez

        if (url != null &&
            (url.StartsWith("https://www.youtube.com/watch?v=") ||
             url.StartsWith("https://youtu.be/")))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Please enter a valid YouTube video URL.");
    }
}
