namespace LostFilmMonitoring.BLL.Validators;

/// <inheritdoc/>
public class EditUserRequestModelValidator : IValidator<EditUserRequestModel>
{
    /// <inheritdoc/>
    public Task<ValidationResult> ValidateAsync(EditUserRequestModel model)
    {
        var result = new ValidationResult();
        if (string.IsNullOrEmpty(model?.TrackerId))
        {
            result.SetError(nameof(model.TrackerId), $"Поле {nameof(model.TrackerId)} не заполнено");
        }

        return Task.FromResult(result);
    }
}
