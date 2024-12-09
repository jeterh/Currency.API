using Currency.API.Application.Currency;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.Bpi.Model;
using Currency.API.Domain.IRepositories.BpiDetail;
using Currency.API.Domain.IRepositories.BpiDetail.Model;
using NSubstitute;
using Xunit;

namespace Currency.API.UnitTest.Application.Currency
{
	public class CreateCurrencyBpiLanguageInputHandlerTests
	{
		private readonly IBpiRepository _bpiRepository;
		private readonly IBpiDetailRepository _bpiDetailRepository;
		private readonly CreateCurrencyBpiLanguageInputHandler _handler;

		public CreateCurrencyBpiLanguageInputHandlerTests()
		{
			_bpiRepository = Substitute.For<IBpiRepository>();
			_bpiDetailRepository = Substitute.For<IBpiDetailRepository>();
			_handler = new CreateCurrencyBpiLanguageInputHandler(_bpiRepository, _bpiDetailRepository);
		}

		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenInsertIsSuccessful()
		{
			// Arrange
			var request = new CreateCurrencyBpiLanguageInput
			{
				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				CurrencyCode = "USD",
				Language = "zh-cn",
				Description = "測試多國語系寫入"
			};

			var bpiData = new List<GetBpiOutput>
			{
				new GetBpiOutput { Id = 1, TimeInfoId = request.TimeInfoId, CurrencyCode = "USD" }
			};

			var bpiDetailData = new List<GetBpiDetailOutput>();

			_bpiRepository.GetBpiAsync(request.TimeInfoId).Returns(bpiData);
			_bpiDetailRepository.GetBpiDetailAsync(1).Returns(bpiDetailData);
			_bpiDetailRepository.InsertBpiDetailAsync(Arg.Any<InsertBpiDetailInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal("0000", response.ReturnCode);
		}

		[Fact]
		public async Task Handle_ShouldReturnFail_WhenTimeInfoIdNotFound()
		{
			// Arrange
			var request = new CreateCurrencyBpiLanguageInput
			{
				TimeInfoId = Guid.NewGuid(),
				CurrencyCode = "USDXX",
				Language = "zh-cn",
				Description = "English description"
			};

			_bpiRepository.GetBpiAsync(request.TimeInfoId).Returns((IEnumerable<GetBpiOutput>)null);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal("9997", response.ReturnCode);
			Assert.Contains("不存在任何資料。", response.Message);
		}
	}
}
