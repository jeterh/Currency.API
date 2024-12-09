using Currency.API.Application.Currency;
using NSubstitute;
using Xunit;
using Currency.API.Domain.IRepositories.Bpi.Model;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.BpiDetail.Model;
using Currency.API.Domain.IRepositories.BpiDetail;

namespace Currency.API.UnitTest.Application.Currency
{
	public class DeleteCurrencyBpiLanguageInputHandlerTests
	{
		private readonly IBpiRepository _bpiRepository;
		private readonly IBpiDetailRepository _bpiDetailRepository;
		private readonly DeleteCurrencyBpiLanguageInputHandler _handler;

		public DeleteCurrencyBpiLanguageInputHandlerTests()
		{
			_bpiRepository = Substitute.For<IBpiRepository>();
			_bpiDetailRepository = Substitute.For<IBpiDetailRepository>();
			_handler = new DeleteCurrencyBpiLanguageInputHandler(_bpiRepository, _bpiDetailRepository);
		}

		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenDeleteIsSuccessful()
		{
			// Arrange
			var request = new DeleteCurrencyBpiLanguageInput
			{
				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				CurrencyCode = "USD",
				Language = "zh-tw",
			};

			var bpiData = new List<GetBpiOutput>
			{
				new GetBpiOutput { Id = 1, TimeInfoId = request.TimeInfoId, CurrencyCode = "USD" }
			};

			var bpiDetailData = new List<GetBpiDetailOutput> 
			{
  				new GetBpiDetailOutput{  Id = 1, BpiId = 1, Description ="AAA", Language ="zh-tw"},
				new GetBpiDetailOutput{  Id = 2, BpiId = 1, Description ="BBB", Language ="en-us"},
			};

			_bpiRepository.GetBpiAsync(request.TimeInfoId).Returns(bpiData);
			_bpiDetailRepository.GetBpiDetailAsync(1).Returns(bpiDetailData);
			_bpiDetailRepository.DeleteBpiDetailAsync(Arg.Any<int>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal("0000", response.ReturnCode);
		}

		[Fact]
		public async Task Handle_ShouldReturnFail_WhenDeleteIsNotFound()
		{
			// Arrange
			var request = new DeleteCurrencyBpiLanguageInput
			{
				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				CurrencyCode = "USD",
				Language = "zh-cn",
			};

			var bpiData = new List<GetBpiOutput>
			{
				new GetBpiOutput { Id = 1, TimeInfoId = request.TimeInfoId, CurrencyCode = "USD" }
			};

			var bpiDetailData = new List<GetBpiDetailOutput>
			{
  				new GetBpiDetailOutput{  Id = 1, BpiId = 1, Description ="AAA", Language ="zh-tw"},
				new GetBpiDetailOutput{  Id = 2, BpiId = 1, Description ="BBB", Language ="en-us"},
			};

			_bpiRepository.GetBpiAsync(request.TimeInfoId).Returns(bpiData);
			_bpiDetailRepository.GetBpiDetailAsync(1).Returns(bpiDetailData);
			_bpiDetailRepository.DeleteBpiDetailAsync(Arg.Any<int>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal("9997", response.ReturnCode);
			Assert.Contains("不存在，無法刪除此語系", response.Message);
		}
	}	
}
