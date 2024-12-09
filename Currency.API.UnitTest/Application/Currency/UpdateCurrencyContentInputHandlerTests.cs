using Currency.API.Application.Currency;
using Currency.API.Domain.IRepositories.Content.Model;
using Currency.API.Domain.IRepositories.Content;
using NSubstitute;
using Xunit;

namespace Currency.API.UnitTest.Application.Currency
{
	public class UpdateCurrencyContentInputHandlerTests
	{

		private readonly IContentRepository _contentRepository;
		private readonly UpdateCurrencyContentInputHandler _handler;

		public UpdateCurrencyContentInputHandlerTests()
		{
			_contentRepository = Substitute.For<IContentRepository>();
			_handler = new UpdateCurrencyContentInputHandler(_contentRepository);
		}

		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenUpdateIsSuccessful()
		{
			// Arrange
			var request = new UpdateCurrencyContentInput
			{
				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				Language = "zh-tw",
				ContentKey = "Disclaimer",
				Content = "更新語系文案"
			};

			var contentData = new List<GetContentOutput>
			{
				new GetContentOutput {Id = 1, TimeInfoId = request.TimeInfoId,Content ="AAA" , ContentKey ="Disclaimer" , Language = "zh-tw" },
				new GetContentOutput {Id = 2, TimeInfoId = request.TimeInfoId,Content ="BBB" , ContentKey ="Disclaimer" , Language = "en-us" }
			};

			_contentRepository.GetContentAsync(request.TimeInfoId).Returns(contentData);
			_contentRepository.UpdateContentAsync(Arg.Any<UpdateContentInput>()).Returns(true);

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
			var request = new UpdateCurrencyContentInput
			{

				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				Language = "zh-cn",
				ContentKey = "ChartName"
			};

			var contentData = new List<GetContentOutput>
			{
				new GetContentOutput {Id = 1, TimeInfoId = request.TimeInfoId,Content ="AAA" , ContentKey ="Disclaimer" , Language = "zh-tw" },
				new GetContentOutput {Id = 2, TimeInfoId = request.TimeInfoId,Content ="BBB" , ContentKey ="Disclaimer" , Language = "en-us" },
				new GetContentOutput {Id = 3, TimeInfoId = request.TimeInfoId,Content ="AAA" , ContentKey ="ChartName" , Language = "zh-tw" },
				new GetContentOutput {Id = 4, TimeInfoId = request.TimeInfoId,Content ="BBB" , ContentKey ="ChartName" , Language = "en-us" }
			};

			_contentRepository.GetContentAsync(request.TimeInfoId).Returns(contentData);
			_contentRepository.UpdateContentAsync(Arg.Any<UpdateContentInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal("9997", response.ReturnCode);
			Assert.Contains("不存在，無法更新此語系", response.Message);
		}
	}
}
