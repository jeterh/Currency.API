using Currency.API.Application.Common;
using Currency.API.Domain.IRepositories.Content;
using MediatR;

namespace Currency.API.Application.Currency
{
	public class UpdateCurrencyContentInput : IRequest<ResponseModel>
	{
		public Guid TimeInfoId { get; set; }
		public string ContentnKey { get; set; } = null!;
		public string Language { get; set; } = null!;
		public string Content { get; set; } = null!;
	}

	public class UpdateCurrencyContentInputHandler : IRequestHandler<UpdateCurrencyContentInput, ResponseModel>
	{
		private readonly IContentRepository _contentRepository;

		public UpdateCurrencyContentInputHandler(IContentRepository contentRepository)
		{
			_contentRepository = contentRepository;
		}

		public async Task<ResponseModel> Handle(UpdateCurrencyContentInput request, CancellationToken cancellationToken)
		{
			var validationResult = this.Validate(request);
			if (!validationResult.IsSuccess)
			{
				return validationResult;
			}

			var contentsResult = await _contentRepository.GetContentAsync(request.TimeInfoId);
			if (contentsResult == null)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}不存在任何資料。");
			}

			var contentResult = contentsResult.Where(x => x.Language == request.Language).FirstOrDefault();
			if (contentResult == null)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的Language:{request.Language}不存在，無法更新此語系");
			}
				
			var updateContentResult = await _contentRepository.UpdateContentAsync(new Domain.IRepositories.Content.Model.UpdateContentInput
			{
				TimeInfoId = request.TimeInfoId,
				Language = request.Language,
				ContentnKey = request.ContentnKey,
				Content = request.Content
			});

			return updateContentResult ? new ResponseModel().Success() : new ResponseModel().Error(ReturnCodeEnum.Fail, "更新失敗!");
		}

		private ResponseModel Validate(UpdateCurrencyContentInput request)
		{
			if (request.TimeInfoId == Guid.Empty)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "TimeInfoId can't be empty.");
			}

			if (request.ContentnKey == null || (request.ContentnKey != "Disclaimer" && request.ContentnKey != "ChartName"))
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "ContentnKey incorrect.");
			}

			return new ResponseModel
			{
				IsSuccess = true,
				ReturnCode = "Validation Success",
				Message = "Validation passed."
			};
		}
	}
}
