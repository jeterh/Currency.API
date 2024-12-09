using Currency.API.Application.Currency;
using NSubstitute;
using Xunit;
using Currency.API.Domain.IRepositories.View;
using Currency.API.Domain.IRepositories.View.Model;

namespace Currency.API.UnitTest.Application.Currency
{
	public class GetCurrencysInputHanderTests
	{
		private readonly IViewRepository _viewRepository;
		private readonly GetCurrencysInputHandler _handler;

		public GetCurrencysInputHanderTests()
		{
			_viewRepository = Substitute.For<IViewRepository>();
			_handler = new GetCurrencysInputHandler(_viewRepository);
		}

		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenAllConditionsMet()
		{
			// Arrange
			var timeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9");
			var request = new GetCurrencysInput { Language = "en-us", TimeInfoId = timeInfoId };

			List<CurrencyResponse> currencys = new List<CurrencyResponse> 
			{
				new CurrencyResponse
				{
					Code = "GBP",
					Description = "British Pound Sterling",
					Rate = "78,038.045",
					RateFloat = Convert.ToDecimal(78038.0447),
					Symbol = "£"
				},new CurrencyResponse
				{
					Code = "USD",
					Description = "United States Dollar",
					Rate = "77,666.666",
					RateFloat = Convert.ToDecimal(77777.6666),
					Symbol = "$"
				}
			};
			
			var currencyInfo = new List<GetCurrencysResponse>
			{
				 new GetCurrencysResponse
				 { 
					ChartName = "Bitcoin",
					Disclaimer = "This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org",
					Updated = "2024/12/07 21:25:40",
					Currencys = currencys,
				 }
			};

			var getCurrencyInfoOutput = new List<GetCurrencyInfoOutput>
			{
				new GetCurrencyInfoOutput {  ChartNameContent = "Bitcoin", DisclaimerContent = "This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org", Language = "en-us", Updated ="Dec 7, 2024 13:25:40 UTC" }
			};

			var getCurrencysOutput = new List<GetCurrencysOutput>
			{
				new GetCurrencysOutput { CurrencyCode = "USD", Language = "en-us", Description = "United States Dollar", Rate ="77,666.666", Symbol ="$", RateFloat= Convert.ToDecimal(77777.6666) }
			};

			_viewRepository.GetCurrencyInfoAsync(timeInfoId).Returns(getCurrencyInfoOutput);
			_viewRepository.GetCurrencysAsync(timeInfoId).Returns(getCurrencysOutput);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.NotNull(response.Data);
			Assert.Equal("Bitcoin", response.Data.ChartName);
			Assert.Equal("This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org", response.Data.Disclaimer);
			Assert.Single(response.Data.Currencys);
			Assert.Equal("USD", response.Data.Currencys[0].Code);
		}

		[Fact]
		public async Task Handle_ShouldReturnFail_WhenAllConditionsNotMet()
		{
			// Arrange
			var timeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9");
			var request = new GetCurrencysInput { Language = "zh-cn", TimeInfoId = timeInfoId };

			var currencyInfo = new List<GetCurrencysResponse>
			{
				 new GetCurrencysResponse
				 {
					ChartName = "Bitcoin",
					Disclaimer = "This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org",
					Updated = "2024/12/07 21:25:40",
					Currencys = new List<CurrencyResponse>(),
				 }
			};

			var getCurrencyInfoOutput = new List<GetCurrencyInfoOutput>
			{
				new GetCurrencyInfoOutput {  ChartNameContent = "Bitcoin", DisclaimerContent = "This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org", Language = "en-us", Updated ="Dec 7, 2024 13:25:40 UTC" }
			};

			var getCurrencysOutput = new List<GetCurrencysOutput>
			{
				new GetCurrencysOutput { CurrencyCode = "USD", Language = "en-us", Description = "United States Dollar", Rate ="77,666.666", Symbol ="$", RateFloat= Convert.ToDecimal(77777.6666) }
			};

			_viewRepository.GetCurrencyInfoAsync(timeInfoId).Returns(getCurrencyInfoOutput);
			_viewRepository.GetCurrencysAsync(timeInfoId).Returns(getCurrencysOutput);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal("9997", response.ReturnCode);
			Assert.Contains("取得Currency資訊發生異常!", response.Message);
		}
	}
}
