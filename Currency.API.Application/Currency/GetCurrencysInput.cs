using Currency.API.Application.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Currency.API.Domain.IRepositories.View;
using System.Globalization;

namespace Currency.API.Application.Currency
{
	public class GetCurrencysInput : IRequest<ResponseModel<GetCurrencysResponse>>
	{
        public string? Language { get; set; }
    }

	public class GetCurrencysResponse
	{
		public string Updated { get; set; } = null!;
		public string Disclaimer { get; set; } = null!;
		public string ChartName { get; set; } = null!;
		public List<CurrencyResponse> Currencys { get; set; } = null!;
	}

	public class CurrencyResponse
	{
		public string? Code { get; set; }
		public string? Symbol { get; set; }
		public string? Rate { get; set; }
		public decimal RateFloat { get; set; }
		public string? Description { get; set; }
	}

	public class GetCurrencysInputHandler : IRequestHandler<GetCurrencysInput, ResponseModel<GetCurrencysResponse>>
	{
		private readonly IConfiguration _configuration;
		private readonly IViewRepository _viewRepository;

		public GetCurrencysInputHandler(IConfiguration configuration, IViewRepository viewRepository)
		{
			_configuration = configuration;
			_viewRepository = viewRepository;
		}

		public async Task<ResponseModel<GetCurrencysResponse>> Handle(GetCurrencysInput request, CancellationToken cancellationToken)
		{
		    var result = new ResponseModel<GetCurrencysResponse>();
			var timeInfoId = Guid.Parse(_configuration["CurrencyConfig:TimeInfoId"]!);

			#region 取得基本資訊
			var currencyInfos = await _viewRepository.GetCurrencyInfoAsync(timeInfoId);
			var currencyInfo = currencyInfos.Where(x => x.Language == request.Language).FirstOrDefault();

			if (currencyInfo == null)
			{
				return result.Error(ReturnCodeEnum.Fail, $"取得Currency資訊發生異常!");
			}

			string inputFormat = "MMM d, yyyy HH:mm:ss 'UTC'";
			string outputFormat = "yyyy/MM/dd HH:mm:ss";
			// 解析字串為 UTC 的 DateTime，並明確設定 Kind 為 UTC
			DateTime utcDateTime = DateTime.SpecifyKind(
				DateTime.ParseExact(currencyInfo!.Updated, inputFormat, CultureInfo.InvariantCulture),
				DateTimeKind.Utc);

			// 台灣時區 (UTC+8)
			TimeZoneInfo taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

			// 將 UTC 時間轉換為台灣時間
			DateTime taiwanDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, taiwanTimeZone);

			// 格式化轉換後的台灣時間
			string formattedTaiwanDate = taiwanDateTime.ToString(outputFormat);
			#endregion

			#region 取得幣別資訊
			var currencys = await _viewRepository.GetCurrencysAsync(timeInfoId);
			var currencyList = currencys.Where(x => x.Language == request.Language).OrderBy(y => y.CurrencyCode).ToList();

			if (currencyList == null || currencyList.Count <= 0)
			{
				return result.Error(ReturnCodeEnum.Fail, $"未能正確取得幣別資訊");
			}

			List<CurrencyResponse> currencyResponses = currencyList.Select(output => new CurrencyResponse
			{
				Code = output.CurrencyCode,
				Symbol = output.Symbol,
				Rate = output.Rate,
				RateFloat = output.RateFloat, 
				Description = output.Description
			}).ToList();
			#endregion

			GetCurrencysResponse currencysResponse = new GetCurrencysResponse
			{
				Updated = formattedTaiwanDate,
				ChartName = currencyInfo.ChartNameContent,
				Disclaimer = currencyInfo.DisclaimerContent,
				Currencys = currencyResponses
			};

			result.Data = currencysResponse;

			return result.Success();
		}
	}
}
