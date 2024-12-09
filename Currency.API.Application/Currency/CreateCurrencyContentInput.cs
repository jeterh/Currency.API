﻿using Currency.API.Application.Common;
using Currency.API.Domain.IRepositories.Content;
using MediatR;

namespace Currency.API.Application.Currency
{
	public class CreateCurrencyContentInput : IRequest<ResponseModel>
	{
		public Guid TimeInfoId { get; set; }
		public string ContentnKey { get; set; } = null!;
		public string Language { get; set; } = null!;
		public string Content { get; set; } = null!;

		public class CreateCurrencyContentInputHandler : IRequestHandler<CreateCurrencyContentInput, ResponseModel>
		{
			private readonly IContentRepository _contentRepository;


			public CreateCurrencyContentInputHandler(IContentRepository contentRepository)
			{
				_contentRepository = contentRepository;
			}

			public async Task<ResponseModel> Handle(CreateCurrencyContentInput request, CancellationToken cancellationToken)
			{
				var validationResult = request.Validate();
				if (!validationResult.IsSuccess)
				{
					return validationResult;
				}

				var contentsResult = await _contentRepository.GetContentAsync(request.TimeInfoId);
				if (contentsResult == null)
				{
					return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}不存在任何資料。");
				}

				var contentResult = contentsResult.Where(x => x.Language == request.Language).FirstOrDefault();
				if (contentResult != null)
				{
					return new ResponseModel().Error(ReturnCodeEnum.Fail, $"你輸入的TimeInfoId：{request.TimeInfoId}的Language:{request.Language}已存在，無法新增此語系");
				}

				var insertContentResut = await _contentRepository.InsertContentAsync(new Domain.IRepositories.Content.Model.InsertContentInput
				{
					TimeInfoId = request.TimeInfoId,
					Language = request.Language,
					ContentnKey = request.ContentnKey,
					Content = request.Content
				});

				return insertContentResut ? new ResponseModel().Success() : new ResponseModel().Error(ReturnCodeEnum.Fail, "新增失敗!");
			}
		}

		private ResponseModel Validate()
		{
			if (TimeInfoId == Guid.Empty)
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "TimeInfoId can't be empty.");
			}

			if (ContentnKey == null || (ContentnKey != "Disclaimer" && ContentnKey != "ChartName"))
			{
				return new ResponseModel().Error(ReturnCodeEnum.Fail, "ContentnKey incorrect.");
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
