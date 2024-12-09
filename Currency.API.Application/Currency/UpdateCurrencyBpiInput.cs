using Currency.API.Application.Common;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.BpiDetail;
using MediatR;

namespace Currency.API.Application.Currency
{
	public class UpdateCurrencyBpiInput : IRequest<ResponseModel>
	{
		public Guid TimeInfoId { get; set; }
		public string CurrencyCode { get; set; } = null!;
		public string? Symbol { get; set; }
		public string? Rate { get; set; }
		public decimal? RateFloat { get; set; }
		public string Language { get; set; } = null!;
		public string? Description { get; set; }
	}

	public class UpdateCurrencyBpiInputHandler : IRequestHandler<UpdateCurrencyBpiInput, ResponseModel>
	{
		private readonly IBpiRepository _bpiRepository;
		private readonly IBpiDetailRepository _bpiDetailRepository;


		public UpdateCurrencyBpiInputHandler(IBpiRepository bpiRepository, IBpiDetailRepository bpiDetailRepository)
		{
			_bpiRepository = bpiRepository;
			_bpiDetailRepository = bpiDetailRepository;
		}

		public async Task<ResponseModel> Handle(UpdateCurrencyBpiInput request, CancellationToken cancellationToken)
		{
			var validationResult = this.Validate(request);
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
				return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的CurrencyCode:{request.CurrencyCode}的Language:{request.Language}不存在，無法更新此語系");
			}

			var updateBpiResult = await _bpiRepository.UpdateBpiAsync(new Domain.IRepositories.Bpi.Model.UpdateBpiInput
			{
				TimeInfoId = request.TimeInfoId,
				Code = request.CurrencyCode,
				Rate = request.Rate,
				RateFloat = request.RateFloat,
				Symbol = request.Symbol,
			});

			if (!string.IsNullOrEmpty(request.Description))
			{
				var updateBpiDetailResult = await _bpiDetailRepository.UpdateBpiDetailAsync(new Domain.IRepositories.BpiDetail.Model.UpdateBpiDetailInput
				{
					BpiId = bpiResult.Id,
					Description = request.Description,
					Language = request.Language,
				});

				return updateBpiResult & updateBpiDetailResult ? new ResponseModel().Success() : new ResponseModel().Error(ReturnCodeEnum.Fail, "更新失敗!");
			}

			return updateBpiResult ? new ResponseModel().Success() : new ResponseModel().Error(ReturnCodeEnum.Fail, "更新失敗!");
		}

		private ResponseModel Validate(UpdateCurrencyBpiInput request)
		{
			if (request.TimeInfoId == Guid.Empty)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "TimeInfoId can't be empty.");
			}

			if (request.RateFloat < 0)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "RateFloat can't less than 0.");
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
