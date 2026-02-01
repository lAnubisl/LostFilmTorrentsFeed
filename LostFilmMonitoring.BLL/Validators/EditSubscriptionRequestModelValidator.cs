namespace LostFilmMonitoring.BLL.Validators;

/// <inheritdoc/>
public class EditSubscriptionRequestModelValidator : IValidator<EditSubscriptionRequestModel>
{
    private readonly IUserDao userDAO;
    private readonly ISeriesDao seriesDAO;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditSubscriptionRequestModelValidator"/> class.
    /// </summary>
    /// <param name="userDAO">Instance of <see cref="IUserDao"/>.</param>
    /// <param name="seriesDAO">Instance of <see cref="ISeriesDao"/>.</param>
    public EditSubscriptionRequestModelValidator(IUserDao userDAO, ISeriesDao seriesDAO)
    {
        this.userDAO = userDAO ?? throw new ArgumentNullException(nameof(userDAO));
        this.seriesDAO = seriesDAO ?? throw new ArgumentNullException(nameof(seriesDAO));
    }

    /// <inheritdoc/>
    public async Task<ValidationResult> ValidateAsync(EditSubscriptionRequestModel model)
    {
        var result = new ValidationResult();
        var allSeries = await this.seriesDAO.LoadAsync();

        if (model == null || string.IsNullOrEmpty(model.UserId))
        {
            return ValidationResult.Fail(nameof(model.UserId), string.Format(ErrorMessages.FieldEmpty, nameof(model.UserId)));
        }

        if (model.Items == null)
        {
            return ValidationResult.Fail(nameof(model.Items), string.Format(ErrorMessages.FieldEmpty, nameof(model.Items)));
        }

        foreach (var item in model.Items)
        {
            if (string.IsNullOrEmpty(item.SeriesId))
            {
                result.SetError(nameof(item.SeriesId), string.Format(ErrorMessages.FieldEmpty, nameof(item.SeriesId)));
                return result;
            }

            if (!IsIn(item.Quality, Quality.SD, Quality.H1080, Quality.H720))
            {
                result.SetError(nameof(item.Quality), string.Format(ErrorMessages.ShouldBeIn, nameof(item.Quality), string.Join(", ", [Quality.SD, Quality.H1080, Quality.H720])));
                return result;
            }

            if (!Guid.TryParse(item.SeriesId, out var seriesId))
            {
                result.SetError(nameof(item.SeriesId), string.Format(ErrorMessages.SeriesDoesNotExist, item.SeriesId));
                return result;
            }

            var series = allSeries?.FirstOrDefault(s => s.Id == seriesId);
            if (series == null)
            {
                result.SetError(nameof(item.SeriesId), string.Format(ErrorMessages.SeriesDoesNotExist, item.SeriesId));
                return result;
            }
        }

        if ((await this.userDAO.LoadAsync(model.UserId)) == null)
        {
            result.SetError(nameof(model.UserId), string.Format(ErrorMessages.UserDoesNotExist, model.UserId));
            return result;
        }

        return result;
    }

    private static bool IsIn(string? str, params string[] values) => values.Contains(str);
}
