using Currency.API.Application.Currency;
using Currency.API.Domain.IRepositories.Bpi.Model;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.BpiDetail.Model;
using Currency.API.Domain.IRepositories.BpiDetail;
using NSubstitute;
using Xunit;

namespace Currency.API.UnitTest.Application.Currency
{
	public class UpdateCurrencyBpiInputHandlerTests
	{
		private readonly IBpiRepository _bpiRepository;
		private readonly IBpiDetailRepository _bpiDetailRepository;
		private readonly UpdateCurrencyBpiInputHandler _handler;

		public UpdateCurrencyBpiInputHandlerTests()
		{
			_bpiRepository = Substitute.For<IBpiRepository>();
			_bpiDetailRepository = Substitute.For<IBpiDetailRepository>();
			_handler = new UpdateCurrencyBpiInputHandler(_bpiRepository, _bpiDetailRepository);
		}

		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenUpdateIsSuccessful()
		{
			// Arrange
			var request = new UpdateCurrencyBpiInput
			{
				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				CurrencyCode = "USD",
				Language = "zh-tw",
				Description = "更新語系文案",
				Rate = "9999,9999",
				RateFloat = 9999999, 
				Symbol = "$"
			};

			var bpiData = new List<GetBpiOutput>
			{
				new GetBpiOutput { Id = 1, TimeInfoId = request.TimeInfoId, CurrencyCode = "USD", Symbol = "@", Rate = "888,888.666", RateFloat = 8888888.6664 },
				new GetBpiOutput { Id = 2, TimeInfoId = request.TimeInfoId, CurrencyCode = "GBP", Symbol = "€", Rate = "94,078.661", RateFloat = 94078.6608 }
			};

			var bpiDetailData = new List<GetBpiDetailOutput>
			{
  				new GetBpiDetailOutput{  Id = 1, BpiId = 1, Description ="AAA", Language ="zh-tw"},
				new GetBpiDetailOutput{  Id = 2, BpiId = 1, Description ="BBB", Language ="en-us"},
			};

			_bpiRepository.GetBpiAsync(request.TimeInfoId).Returns(bpiData);
			_bpiDetailRepository.GetBpiDetailAsync(1).Returns(bpiDetailData);
			_bpiRepository.UpdateBpiAsync(Arg.Any<UpdateBpiInput>()).Returns(true);
			_bpiDetailRepository.UpdateBpiDetailAsync(Arg.Any<UpdateBpiDetailInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal("0000", response.ReturnCode);
		}

		[Fact]
		public async Task Handle_ShouldReturnFail_WhenUpdateIsNotFound()
		{
			// Arrange
			var request = new UpdateCurrencyBpiInput
			{
				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				CurrencyCode = "USD",
				Language = "zh-cn",
				Description = "更新語系文案",
				Rate = "9999,9999",
				RateFloat = 9999999,
				Symbol = "$"
			};

			var bpiData = new List<GetBpiOutput>
			{
				new GetBpiOutput { Id = 1, TimeInfoId = request.TimeInfoId, CurrencyCode = "USD", Symbol = "@", Rate = "888,888.666", RateFloat = 8888888.6664 },
				new GetBpiOutput { Id = 2, TimeInfoId = request.TimeInfoId, CurrencyCode = "GBP", Symbol = "€", Rate = "94,078.661", RateFloat = 94078.6608 }
			};

			var bpiDetailData = new List<GetBpiDetailOutput>
			{
  				new GetBpiDetailOutput{  Id = 1, BpiId = 1, Description ="AAA", Language ="zh-tw"},
				new GetBpiDetailOutput{  Id = 2, BpiId = 1, Description ="BBB", Language ="en-us"},
			};

			_bpiRepository.GetBpiAsync(request.TimeInfoId).Returns(bpiData);
			_bpiDetailRepository.GetBpiDetailAsync(1).Returns(bpiDetailData);
			_bpiRepository.UpdateBpiAsync(Arg.Any<UpdateBpiInput>()).Returns(true);
			_bpiDetailRepository.UpdateBpiDetailAsync(Arg.Any<UpdateBpiDetailInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal("9997", response.ReturnCode);
			Assert.Contains("不存在，無法更新此語系", response.Message);
		}
	}
}
