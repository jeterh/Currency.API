using Currency.API.Application.Common;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.BpiDetail;
using MediatR;

namespace Currency.API.Application.Currency
{
	public class CreateCurrencyBpiLanguageInput : IRequest<ResponseModel>
	{
		public Guid TimeInfoId { get; set; }
		public string CurrencyCode { get; set; } = null!;
		public string Language { get; set; } = null!;
		public string Description { get; set; } = null!;
	}

	public class CreateCurrencyBpiLanguageInputHandler : IRequestHandler<CreateCurrencyBpiLanguageInput, ResponseModel>
	{
		private readonly IBpiRepository _bpiRepository;
		private readonly IBpiDetailRepository _bpiDetailRepository;

		public CreateCurrencyBpiLanguageInputHandler(IBpiRepository bpiRepository, IBpiDetailRepository bpiDetailRepository)
		{
			_bpiRepository = bpiRepository;
			_bpiDetailRepository = bpiDetailRepository;
		}

		public async Task<ResponseModel> Handle(CreateCurrencyBpiLanguageInput request, CancellationToken cancellationToken)
		{
			var validationResult = Validate(request);
			if (!validationResult.IsSuccess)
			{
				return validationResult;
			}

			var bpisResult = await _bpiRepository.GetBpiAsync(request.TimeInfoId);
			if (bpisResult == null || !bpisResult.Any())
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

			if (bpiDetailInfo != null)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的CurrencyCode:{request.CurrencyCode}的Language:{request.Language}已存在，無法新增此語系");
			}

			var insertBpiDetailResult = await _bpiDetailRepository.InsertBpiDetailAsync(new Domain.IRepositories.BpiDetail.Model.InsertBpiDetailInput
			{
				BpiId = bpiResult.Id,
				Description = request.Description,
				Language = request.Language,
			});

			return insertBpiDetailResult ? new ResponseModel().Success() : new ResponseModel().Error(ReturnCodeEnum.Fail, "新增失敗!");
		}

		private ResponseModel Validate(CreateCurrencyBpiLanguageInput request)
		{
			if (request.TimeInfoId == Guid.Empty)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "TimeInfoId can't be empty.");
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
