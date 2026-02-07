namespace LostFilmMonitoring.BLL.Models;

/// <summary>
/// Describes validation result.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    internal ValidationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    /// <param name="property">Property that has an error.</param>
    /// <param name="message">Error message.</param>
    internal ValidationResult(string property, string message)
    {
        this.Errors[property] = message;
    }

    /// <summary>
    /// Gets list of model properties and error messages.
    /// </summary>
    public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets a value indicating whether is the validation was successful or not.
    /// </summary>
    public bool IsValid => this.Errors == null || this.Errors.Count == 0;

    /// <summary>
    /// Gets <see cref="ValidationResult"/> without any errors.
    /// </summary>
    internal static ValidationResult Ok => new ();

    /// <summary>
    /// Gets an instance of <see cref="ValidationResult"/> with predefined error message.
    /// </summary>
    /// <param name="property">Property that has an error.</param>
    /// <param name="message">Error message.</param>
    /// <param name="objects">Parameters for string.format.</param>
    /// <returns>Instance of <see cref="ValidationResult"/>.</returns>
    internal static ValidationResult Fail(string property, string message, params string[] objects)
        => new (
            property,
            string.Format(message, new[] { property }.Union(objects ?? []).ToArray()));

    /// <summary>
    /// Gets an instance of <see cref="ValidationResult"/> with predefined error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>Instance of <see cref="ValidationResult"/>.</returns>
    internal static ValidationResult Fail(string message) => new ("model", message);

    /// <summary>
    /// Sets validation error to <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="property">Model property name.</param>
    /// <param name="message">Model property validation message.</param>
    internal void SetError(string property, string message)
    {
        this.Errors[property] = message;
    }
}
