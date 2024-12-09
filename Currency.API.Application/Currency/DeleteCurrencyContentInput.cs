using Currency.API.Application.Common;
using Currency.API.Domain.IRepositories.Content;
using MediatR;

namespace Currency.API.Application.Currency
{
	public class DeleteCurrencyContentInput : IRequest<ResponseModel>
	{
		public Guid TimeInfoId { get; set; }
		public string ContentKey { get; set; } = null!;
		public string Language { get; set; } = null!;
	}

	public class DeleteCurrencyContentInputHandler : IRequestHandler<DeleteCurrencyContentInput, ResponseModel>
	{
		private readonly IContentRepository _contentRepository;


		public DeleteCurrencyContentInputHandler(IContentRepository contentRepository)
		{
			_contentRepository = contentRepository;
		}

		public async Task<ResponseModel> Handle(DeleteCurrencyContentInput request, CancellationToken cancellationToken)
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

			var contentResult = contentsResult.Where(x => x.Language == request.Language && x.ContentKey == request.ContentKey).FirstOrDefault();
			if (contentResult == null)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的Language:{request.Language}或ContentKey:{request.ContentKey}不存在，無法刪除此語系");
			}

			var deleteBpiDetailResult = await _contentRepository.DeleteContentAsync(new Domain.IRepositories.Content.Model.DeleteContentInput { 
				TimeInfoId = request.TimeInfoId,
				ContentKey = request.ContentKey,
				Language = request.Language,
			});

			return deleteBpiDetailResult ? new ResponseModel().Success() : new ResponseModel().Error(ReturnCodeEnum.Fail, "刪除失敗!");
		}

		private ResponseModel Validate(DeleteCurrencyContentInput request)
		{
			if (request.TimeInfoId == Guid.Empty)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "TimeInfoId can't be empty.");
			}

			if (request.Language == "en-us")
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "en-us is default Language can't delete.");
			}

			if (request.ContentKey == null || (request.ContentKey != "Disclaimer" && request.ContentKey != "ChartName"))
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "ContentKey incorrect.");
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
