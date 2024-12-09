using Currency.API.Application.Common;
using MediatR;
using System.Net.Http.Json;
using Currency.API.Domain.IRepositories.Content;
using Microsoft.Extensions.Configuration;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.TimeInfo;
using Currency.API.Domain.IRepositories.BpiDetail;
using System.Net;

namespace Currency.API.Application.Coindesk
{
	public class GetCoindeskInfoInput : IRequest<ResponseModel<GetCoindeskInfoResponse>>
	{
	}

	public class GetCoindeskInfoResponse
	{
	    public Time? Time { get; set; }
		public string? Disclaimer { get; set; }
		public string? ChartName { get; set; }
		public Bpi? Bpi { get; set; }
	}

	public class Bpi
	{
		public USD? Usd { get; set; }
		public GBP? Gbp { get; set; }
		public EUR? Eur { get; set; }
	}

	public class EUR
	{
		public string? Code { get; set; }
		public string? Symbol { get; set; }
		public string? Rate { get; set; }
		public string? Description { get; set; }
		public decimal Rate_Float { get; set; }
	}

	public class GBP
	{
		public string? Code { get; set; }
		public string? Symbol { get; set; }
		public string? Rate { get; set; }
		public string? Description { get; set; }
		public decimal Rate_float { get; set; }
	}

	public class Time
	{
		public string? Updated { get; set; }
		public string UpdatedISO { get; set; } = null!;
		public string? Updateduk { get; set; }
	}

	public class USD
	{
		public string? Code { get; set; }
		public string? Symbol { get; set; }
		public string? Rate { get; set; }
		public string? Description { get; set; }
		public decimal Rate_float { get; set; }
	}

	public class GetCoindeskInfoInputHandler : IRequestHandler<GetCoindeskInfoInput, ResponseModel<GetCoindeskInfoResponse>>
	{
		private readonly HttpClient _httpClient;
		private readonly ITimeInfoRepository _timeInfoRepository;
		private readonly IContentRepository _contentRepository;
		private readonly IBpiRepository _bpiRepository;
		private readonly IBpiDetailRepository _bpiDetailRepository;
		private readonly IConfiguration _configuration;

		public GetCoindeskInfoInputHandler(HttpClient httpClient,  IConfiguration configuration, ITimeInfoRepository timeInfoRepository,
			IContentRepository contentRepository, IBpiRepository bpiRepository, IBpiDetailRepository bpiDetailRepository
			)
		{
			_httpClient = httpClient;
			_configuration = configuration;
			_timeInfoRepository = timeInfoRepository;
			_contentRepository = contentRepository;
			_bpiRepository = bpiRepository;
			_bpiDetailRepository = bpiDetailRepository;
		}

		public async Task<ResponseModel<GetCoindeskInfoResponse>> Handle(GetCoindeskInfoInput request, CancellationToken cancellationToken)
		{
			var result = new ResponseModel<GetCoindeskInfoResponse>();
			
			var requestUrl = "https://api.coindesk.com/v1/bpi/currentprice.json";
			var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
			var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				return result.Error(ReturnCodeEnum.Fail, $"Coindesk 回應錯誤StatusCode，StatusCode={response.StatusCode}");
			}

			var condeskInfo = await response.Content.ReadFromJsonAsync<GetCoindeskInfoResponse>();
			if (condeskInfo == null)
			{
				return result.Error(ReturnCodeEnum.Fail, "取得Coindesk資訊失敗");
			}

			var timeInfoId = Guid.Parse(_configuration["CurrencyConfig:TimeInfoId"]!);

			//判斷是否有存在的資料
			var timeInfoResult = await _timeInfoRepository.GetTimeInfoAsync(timeInfoId);
			if (timeInfoResult == null)
			{
				var insertTimeInfoResult = await _timeInfoRepository.InsertTimeInfoAsync(new Domain.IRepositories.TimeInfo.Model.InsertTimeInfoInput
				{
					Id = timeInfoId,
					Updated = condeskInfo.Time!.Updated!,
					UpdatedISO = condeskInfo.Time!.UpdatedISO!,
					UpdatedUK	= condeskInfo.Time!.Updateduk!,
					UpdatedAt = DateTime.UtcNow,
				});

				#region Inset USD BPI & BpiDetail
				var isInsertUSDBpiSuccess = false;
				var insertUsdBpiResult = await _bpiRepository.InsertBpiAsync(new Domain.IRepositories.Bpi.Model.InsertBpiInput
				{
					TimeInfoId = timeInfoId,
					CurrencyCode = condeskInfo.Bpi!.Usd!.Code!,
					Rate = condeskInfo.Bpi.Usd.Rate!,
					RateFloat = condeskInfo.Bpi!.Usd!.Rate_float!,
					Symbol = WebUtility.HtmlDecode(condeskInfo.Bpi!.Usd!.Symbol!),
				});

				if (insertUsdBpiResult != null || insertUsdBpiResult > 0)
				{
					var insertBpiDetailResult = await _bpiDetailRepository.InsertBpiDetailAsync(new Domain.IRepositories.BpiDetail.Model.InsertBpiDetailInput
					{
						BpiId = (int)insertUsdBpiResult,
						Description = condeskInfo.Bpi!.Usd!.Description!,
						Language = "en-us"
					});
					isInsertUSDBpiSuccess = insertBpiDetailResult;
				}
				#endregion

				#region Inset GBP BPI & BpiDetail 
				var isInsertGBPBpiSuccess = false;
				var insertGbpBpiResult = await _bpiRepository.InsertBpiAsync(new Domain.IRepositories.Bpi.Model.InsertBpiInput
				{
					TimeInfoId = timeInfoId,
					CurrencyCode = condeskInfo.Bpi!.Gbp!.Code!,
					Rate = condeskInfo.Bpi.Gbp.Rate!,
					RateFloat = condeskInfo.Bpi!.Gbp!.Rate_float!,
					Symbol = WebUtility.HtmlDecode(condeskInfo.Bpi!.Gbp!.Symbol!),
				});

				if (insertGbpBpiResult != null || insertGbpBpiResult > 0)
				{
					var insertBpiDetailResult = await _bpiDetailRepository.InsertBpiDetailAsync(new Domain.IRepositories.BpiDetail.Model.InsertBpiDetailInput
					{
						BpiId = (int)insertGbpBpiResult,
						Description = condeskInfo.Bpi!.Gbp!.Description!,
						Language = "en-us"
					});
					isInsertGBPBpiSuccess = insertBpiDetailResult;
				}
				#endregion

				#region Inset EUR BPI & BpiDetail
				var isInsertEURBpiSuccess = false;
				var insertEurBpiResult = await _bpiRepository.InsertBpiAsync(new Domain.IRepositories.Bpi.Model.InsertBpiInput
				{
					TimeInfoId = timeInfoId,
					CurrencyCode = condeskInfo.Bpi!.Eur!.Code!,
					Rate = condeskInfo.Bpi.Eur.Rate!,
					RateFloat = condeskInfo.Bpi!.Eur!.Rate_Float!,
					Symbol = WebUtility.HtmlDecode(condeskInfo.Bpi!.Eur!.Symbol!),
				});

				if (insertEurBpiResult != null || insertEurBpiResult > 0)
				{
					var insertBpiDetailResult = await _bpiDetailRepository.InsertBpiDetailAsync(new Domain.IRepositories.BpiDetail.Model.InsertBpiDetailInput
					{
						BpiId = (int)insertEurBpiResult,
						Description = condeskInfo.Bpi!.Eur!.Description!,
						Language = "en-us"
					});
					isInsertEURBpiSuccess = insertBpiDetailResult;
				}
				#endregion

				#region Insert Content Disclaimer
				var insertContentDisclaimer = await _contentRepository.InsertContentAsync(new Domain.IRepositories.Content.Model.InsertContentInput
				{
					TimeInfoId = timeInfoId,
					Language = "en-us",
					ContentKey = nameof(condeskInfo.Disclaimer),
					Content = condeskInfo.Disclaimer!
				});
				#endregion

				#region Insert Content ChartName
				var insertContentChartName = await _contentRepository.InsertContentAsync(new Domain.IRepositories.Content.Model.InsertContentInput
				{
					TimeInfoId = timeInfoId,
					Language = "en-us",
					ContentKey = nameof(condeskInfo.ChartName),
					Content = condeskInfo.ChartName!
				});
				#endregion

				var isSuccess = insertTimeInfoResult & isInsertUSDBpiSuccess | isInsertGBPBpiSuccess | isInsertEURBpiSuccess &
					insertContentDisclaimer & insertContentChartName;

				if (!isSuccess) 
				{
					result.Data = null;
				}

				return isSuccess ? result.Success() :result.Error(ReturnCodeEnum.Fail, "新增TimeInfo失敗");
			}
			else
			{
				//更新所有en-us的資訊
				var updateTimeInfoResult = await _timeInfoRepository.UpdateTimeInfoAsync(new Domain.IRepositories.TimeInfo.Model.UpdateTimeInfoInput
				{
					Id = timeInfoId,
					Updated = condeskInfo.Time!.Updated!,
					UpdatedISO = condeskInfo.Time!.UpdatedISO!,
					UpdatedUK = condeskInfo.Time!.Updateduk!,
					UpdatedAt = DateTime.UtcNow,
				});

				if (!updateTimeInfoResult)
				{
					return result.Error(ReturnCodeEnum.Fail, "更新TimeInfo失敗");
				}
				
				var updateUsdBpiResult = await _bpiRepository.UpdateBpiAsync(new Domain.IRepositories.Bpi.Model.UpdateBpiInput
				{
					TimeInfoId = timeInfoId,
					Code = condeskInfo.Bpi!.Usd!.Code!,
					Rate = condeskInfo.Bpi!.Usd!.Rate!,
					RateFloat = condeskInfo.Bpi!.Usd!.Rate_float!,
					Symbol = WebUtility.HtmlDecode(condeskInfo.Bpi!.Usd!.Symbol!),
				});

				if (!updateUsdBpiResult)
				{
					return result.Error(ReturnCodeEnum.Fail, "更新Bpi USD失敗");
				}

				var updateGbpBpiResult = await _bpiRepository.UpdateBpiAsync(new Domain.IRepositories.Bpi.Model.UpdateBpiInput
				{
					TimeInfoId = timeInfoId,
					Code = condeskInfo.Bpi!.Gbp!.Code!,
					Rate = condeskInfo.Bpi!.Gbp!.Rate!,
					RateFloat = condeskInfo.Bpi!.Gbp!.Rate_float!,
					Symbol = WebUtility.HtmlDecode(condeskInfo.Bpi!.Gbp!.Symbol!),
				});

				if (!updateTimeInfoResult)
				{
					return result.Error(ReturnCodeEnum.Fail, "更新Bpi GBP失敗");
				}

				var updateEurBpiResult = await _bpiRepository.UpdateBpiAsync(new Domain.IRepositories.Bpi.Model.UpdateBpiInput
				{
					TimeInfoId = timeInfoId,
					Code = condeskInfo.Bpi!.Eur!.Code!,
					Rate = condeskInfo.Bpi!.Eur!.Rate!,
					RateFloat = condeskInfo.Bpi!.Eur!.Rate_Float!,
					Symbol = WebUtility.HtmlDecode(condeskInfo.Bpi!.Eur!.Symbol!),
				});

				if (!updateTimeInfoResult)
				{
					return result.Error(ReturnCodeEnum.Fail, "更新Bpi EUR失敗");
				}

				var bpisResult = await _bpiRepository.GetBpiAsync(timeInfoId);

				foreach (var bpi in bpisResult)
				{
					var description = string.Empty;
					switch (bpi.CurrencyCode)
					{
						case "USD":
							description = condeskInfo.Bpi.Usd.Description;
							break;
						case "GBP":
							description = condeskInfo.Bpi.Gbp.Description;
							break;
						case "EUR":
							description = condeskInfo.Bpi.Eur.Description;
							break;
						default:
							break;
					}

					var updateBpiDetailResult = await _bpiDetailRepository.UpdateBpiDetailAsync(new Domain.IRepositories.BpiDetail.Model.UpdateBpiDetailInput
					{
						BpiId = bpi.Id,
						Description = description!,
						Language = "en-us"
					});

					if (!updateBpiDetailResult)
					{
						return result.Error(ReturnCodeEnum.Fail, $"更新BpiDetail {bpi.CurrencyCode}失敗");
					}
				}

				var updateContentDisclaimerResult =  await _contentRepository.UpdateContentAsync(new Domain.IRepositories.Content.Model.UpdateContentInput
				{
					TimeInfoId = timeInfoId,
					Language = "en-us",
					ContentKey = nameof(condeskInfo.Disclaimer),
					Content = condeskInfo.Disclaimer!
				});

				if (!updateContentDisclaimerResult)
				{
					return result.Error(ReturnCodeEnum.Fail, $"更新Content Disclaimer 失敗");
				}

				var updateContentChartNameResult = await _contentRepository.UpdateContentAsync(new Domain.IRepositories.Content.Model.UpdateContentInput
				{
					TimeInfoId = timeInfoId,
					Language = "en-us",
					ContentKey = nameof(condeskInfo.ChartName),
					Content = condeskInfo.ChartName!
				});

				if (!updateContentDisclaimerResult)
				{
					return result.Error(ReturnCodeEnum.Fail, $"更新Content ChartName 失敗");
				}

				return result.Success();
			}
		}
	}
}
