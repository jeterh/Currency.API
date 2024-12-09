using Currency.API.Application.Currency;
using NSubstitute;
using Xunit;
using Currency.API.Domain.IRepositories.Content;
using Currency.API.Domain.IRepositories.Content.Model;

namespace Currency.API.UnitTest.Application.Currency
{
	public class CreateCurrencyContentInputHandlerTests
	{
		 
		private readonly IContentRepository _contentRepository;
		private readonly CreateCurrencyContentInputHandler _handler;

		public CreateCurrencyContentInputHandlerTests()
		{
			_contentRepository = Substitute.For<IContentRepository>();
			_handler = new CreateCurrencyContentInputHandler(_contentRepository);
		}

		[Fact]
		public async Task Handle_ContentKey_Disclaimer_ShouldReturnSuccess_WhenInsertIsSuccessful()
		{
			// Arrange
			var request = new CreateCurrencyContentInput
			{
				 
				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				Content = "新增Disclaimer的測試語系",
				Language = "zh-cn",
				ContentKey = "Disclaimer"
			};

			var contentData = new List<GetContentOutput>
			{
				new GetContentOutput {Id = 1, TimeInfoId = request.TimeInfoId,Content ="AAA" , ContentKey ="Disclaimer" , Language = "zh-tw" },
				new GetContentOutput {Id = 2, TimeInfoId = request.TimeInfoId,Content ="BBB" , ContentKey ="Disclaimer" , Language = "en-us" }
			};

			_contentRepository.GetContentAsync(request.TimeInfoId).Returns(contentData);
			_contentRepository.InsertContentAsync(Arg.Any<InsertContentInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal("0000", response.ReturnCode);
		}

		[Fact]
		public async Task Handle_ContentKey_ChartName_ShouldReturnSuccess_WhenInsertIsSuccessful()
		{
			// Arrange
			var request = new CreateCurrencyContentInput
			{

				TimeInfoId = Guid.Parse("375A82F8-9811-40B7-B323-793C61F30AF9"),
				Content = "新增ChartName的測試語系",
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
			_contentRepository.InsertContentAsync(Arg.Any<InsertContentInput>()).Returns(true);

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
			var request = new CreateCurrencyContentInput
			{

				TimeInfoId = Guid.NewGuid(),
				Content = "新增Disclaimer的測試語系",
				Language = "zh-cn",
				ContentKey = "Disclaimer"
			};

			var contentData = new List<GetContentOutput>();

			_contentRepository.GetContentAsync(request.TimeInfoId).Returns(contentData);
			_contentRepository.InsertContentAsync(Arg.Any<InsertContentInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal("9997", response.ReturnCode);
			Assert.Contains("不存在任何資料。", response.Message);
		}

		[Fact]
		public async Task Handle_Incorrect_ContentKey_ShouldReturnFail_WhenTimeInfoIdNotFound()
		{
			// Arrange
			var request = new CreateCurrencyContentInput
			{

				TimeInfoId = Guid.NewGuid(),
				Content = "新增Disclaimer的測試語系",
				Language = "zh-cn",
				ContentKey = "Disclaimer_TEST"
			};

			var contentData = new List<GetContentOutput>
			{
				new GetContentOutput {Id = 1, TimeInfoId = request.TimeInfoId,Content ="AAA" , ContentKey ="Disclaimer" , Language = "zh-tw" },
				new GetContentOutput {Id = 2, TimeInfoId = request.TimeInfoId,Content ="BBB" , ContentKey ="Disclaimer" , Language = "en-us" },
				new GetContentOutput {Id = 3, TimeInfoId = request.TimeInfoId,Content ="AAA" , ContentKey ="ChartName" , Language = "zh-tw" },
				new GetContentOutput {Id = 4, TimeInfoId = request.TimeInfoId,Content ="BBB" , ContentKey ="ChartName" , Language = "en-us" }
			};

			_contentRepository.GetContentAsync(request.TimeInfoId).Returns(contentData);
			_contentRepository.InsertContentAsync(Arg.Any<InsertContentInput>()).Returns(true);

			// Act
			var response = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal("9997", response.ReturnCode);
			Assert.Contains("ContentKey incorrect.", response.Message);
		}
	}
}
