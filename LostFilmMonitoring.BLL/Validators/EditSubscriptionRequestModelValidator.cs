// <copyright file="EditSubscriptionRequestModelValidator.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.BLL.Validators
{
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

            if (model == null || string.IsNullOrEmpty(model.UserId))
            {
                return ValidationResult.Fail(nameof(model.UserId), string.Format(ErrorMessages.FieldEmpty, nameof(model.UserId)));
            }

            if (model.Items == null)
            {
                return ValidationResult.Fail(nameof(model.UserId), string.Format(ErrorMessages.FieldEmpty, nameof(model.Items)));
            }

            foreach (var item in model.Items)
            {
                if (string.IsNullOrEmpty(item.SeriesName))
                {
                    result.SetError(nameof(item.SeriesName), string.Format(ErrorMessages.FieldEmpty, nameof(item.SeriesName)));
                    return result;
                }

                if (!IsIn(item.Quality, Quality.SD, Quality.H1080, Quality.H720))
                {
                    result.SetError(nameof(item.Quality), string.Format(ErrorMessages.ShouldBeIn, string.Join(", ", new[] { Quality.SD, Quality.H1080, Quality.H720 })));
                    return result;
                }

                if ((await this.seriesDAO.LoadAsync(item.SeriesName)) == null)
                {
                    result.SetError(nameof(item.SeriesName), string.Format(ErrorMessages.SeriesDoesNotExist, item.SeriesName));
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

        private static bool IsIn(string? str, params string[] values)
        {
            foreach (var value in values)
            {
                if (string.Equals(str, value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
