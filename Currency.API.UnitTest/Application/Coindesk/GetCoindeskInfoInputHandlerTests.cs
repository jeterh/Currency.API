using NSubstitute;
using Xunit;
using Currency.API.Application.Coindesk;
using Currency.API.Domain.IRepositories.Bpi;
using Currency.API.Domain.IRepositories.BpiDetail;
using Currency.API.Domain.IRepositories.Content;
using Currency.API.Domain.IRepositories.TimeInfo;
using Currency.API.Domain.IRepositories.BpiDetail.Model;
using Currency.API.Domain.IRepositories.Content.Model;
using Currency.API.Domain.IRepositories.TimeInfo.Model;
using Currency.API.Domain.IRepositories.Bpi.Model;

namespace Currency.API.UnitTest.Application.Coindesk
{
	public class GetCoindeskInfoInputHandlerTests
	{
		private readonly HttpClient _httpClient;
		private readonly ITimeInfoRepository _timeInfoRepository;
		private readonly IContentRepository _contentRepository;
		private readonly IBpiRepository _bpiRepository;
		private readonly IBpiDetailRepository _bpiDetailRepository;
		private readonly GetCoindeskInfoInputHandler _handler;

		public GetCoindeskInfoInputHandlerTests()
		{
			_httpClient = new HttpClient();
			_timeInfoRepository = Substitute.For<ITimeInfoRepository>();
			_contentRepository = Substitute.For<IContentRepository>();
			_bpiRepository = Substitute.For<IBpiRepository>();
			_bpiDetailRepository = Substitute.For<IBpiDetailRepository>();
			_handler = new GetCoindeskInfoInputHandler(_httpClient, _timeInfoRepository, _contentRepository, _bpiRepository, _bpiDetailRepository);
		}

		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenAllConditionsMet()
		{
			// Arrange
			var timeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9");
			var request = new GetCoindeskInfoInput { TimeInfoId = timeInfoId };

			var coindeskResponse = new GetCoindeskInfoResponse
			{
				Time = new Time { Updated = "Updated", UpdatedISO = "UpdatedISO", Updateduk = "Updateduk" },
				Disclaimer = "Disclaimer",
				ChartName = "ChartName",
				Bpi = new Bpi
				{
					Usd = new USD { Code = "USD", Symbol = "$", Rate = "1000", Description = "US Dollar", Rate_float = 1000 },
					Gbp = new GBP { Code = "GBP", Symbol = "£", Rate = "800", Description = "British Pound", Rate_float = 800 },
					Eur = new EUR { Code = "EUR", Symbol = "€", Rate = "900", Description = "Euro", Rate_Float = 900 }
				}
			};

			var getTimeInfoOutput = new GetTimeInfoOutput
			{
				Id = timeInfoId,
				Updated = "Dec 7, 2024 13:25:40 UTC",
				UpdatedAt = DateTime.Now,
				UpdatedISO = "2024-12-07T13:25:40+00:00", 
				UpdatedUK = "Dec 7, 2024 at 13:25 GMT"
			};

			_timeInfoRepository.GetTimeInfoAsync(timeInfoId).Returns(getTimeInfoOutput);
			_timeInfoRepository.UpdateTimeInfoAsync(Arg.Any<UpdateTimeInfoInput>()).Returns(true);
			_timeInfoRepository.InsertTimeInfoAsync(Arg.Any<InsertTimeInfoInput>()).Returns(true);
			_bpiRepository.InsertBpiAsync(Arg.Any<InsertBpiInput>()).Returns(1);
			_bpiRepository.UpdateBpiAsync(Arg.Any<UpdateBpiInput>()).Returns(true);
			_bpiDetailRepository.InsertBpiDetailAsync(Arg.Any<InsertBpiDetailInput>()).Returns(true);
			_bpiDetailRepository.UpdateBpiDetailAsync(Arg.Any<UpdateBpiDetailInput>()).Returns(true);
			_contentRepository.InsertContentAsync(Arg.Any<InsertContentInput>()).Returns(true);
			_contentRepository.UpdateContentAsync(Arg.Any<UpdateContentInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal("0000", response.ReturnCode);
		}
	}
}
