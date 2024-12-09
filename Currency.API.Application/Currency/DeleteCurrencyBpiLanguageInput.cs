using Currency.API.Application.Common;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.BpiDetail;
using MediatR;

namespace Currency.API.Application.Currency
{
	public class DeleteCurrencyBpiLanguageInput : IRequest<ResponseModel>
	{
		public Guid TimeInfoId { get; set; }
        public string CurrencyCode { get; set; } = null!;
		public string Language { get; set; } = null!;

		public class DeleteCurrencyBpiLanguageInputHandler : IRequestHandler<DeleteCurrencyBpiLanguageInput, ResponseModel>
		{
			private readonly IBpiRepository _bpiRepository;
			private readonly IBpiDetailRepository _bpiDetailRepository;

			public DeleteCurrencyBpiLanguageInputHandler(IBpiRepository bpiRepository, IBpiDetailRepository bpiDetailRepository)
			{
				_bpiRepository = bpiRepository;
				_bpiDetailRepository = bpiDetailRepository;

			}

			public async Task<ResponseModel> Handle(DeleteCurrencyBpiLanguageInput request, CancellationToken cancellationToken)
			{
				var validationResult = request.Validate();
				if (!validationResult.IsSuccess)
				{
					return validationResult;
				}

				var bpisResult = await _bpiRepository.GetBpiAsync(request.TimeInfoId);
				if (bpisResult == null)
				{
					return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}不存在任何資料。");
				}

				var bpiResult = bpisResult.Where(x => x.CurrencyCode == request.CurrencyCode).FirstOrDefault();
				if (bpiResult == null)
				{
					return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的CurrencyCode:{request.CurrencyCode}不存在");
				}

				var bpiDetailResult = await _bpiDetailRepository.GetBpiDetailAsync(bpiResult.Id);
				if (bpiDetailResult == null)
				{
					return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的CurrencyCode:{request.CurrencyCode}的Language資訊不存在");
				}
				var bpiDetailInfo = bpiDetailResult.Where(x => x.Language == request.Language).FirstOrDefault();

				if (bpiDetailInfo == null)
				{
					return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的CurrencyCode:{request.CurrencyCode}的Language:{request.Language}不存在，無法刪除此語系");
				}
				
				var deleteBpiDetailResult = await _bpiDetailRepository.DeleteBpiDetailAsync(bpiDetailInfo.Id);
				return deleteBpiDetailResult ? new ResponseModel().Success() : new ResponseModel().Error(ReturnCodeEnum.Fail, "更新失敗!");	
			}
		}

		private ResponseModel Validate()
		{
			if (TimeInfoId == Guid.Empty)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "TimeInfoId can't be empty.");
			}

			if (Language == "en-us")
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "en-us is default Language can't delete.");
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
