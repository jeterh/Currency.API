using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Currency.API.Utilities;

using Currency.API.Utilities.Extensions;
namespace Currency.API.Application.Common
{
	public class ResponseModel<T> : ResponseModel
	{
		public T? Data { get; set; }
	}
	public class ResponseModel
	{
		public bool IsSuccess { get; set; }
		public string ReturnCode { get; set; } = string.Empty;
		public string? Message { get; set; } = string.Empty;
	}

	public static class ResponseModelExtension
	{
		public static ResponseModel<T> Error<T>(this ResponseModel<T> responseModel, Enum returnCode, string? message)
		{
			responseModel.IsSuccess = false;
			responseModel.ReturnCode = returnCode.GetFourDigitReturnCode();
			responseModel.Message = message ?? returnCode.GetDescription();
			return responseModel;
		}

		public static ResponseModel Error(this ResponseModel responseModel, Enum returnCode, string? message)
		{
			responseModel.IsSuccess = false;
			responseModel.ReturnCode = returnCode.GetFourDigitReturnCode();
			responseModel.Message = message ?? returnCode.GetDescription();
			return responseModel;
		}

		public static ResponseModel<T> Success<T>(this ResponseModel<T> responseModel, Enum? returnCode = null, string? message = null)
		{
			returnCode ??= ReturnCodeEnum.Success;
			responseModel.IsSuccess = true;
			responseModel.ReturnCode = returnCode.GetFourDigitReturnCode();
			responseModel.Message = message ?? returnCode.GetDescription();
			return responseModel;
		}

		public static ResponseModel Success(this ResponseModel responseModel, Enum? returnCode = null, string? message = null)
		{
			returnCode ??= ReturnCodeEnum.Success;
			responseModel.IsSuccess = true;
			responseModel.ReturnCode = returnCode.GetFourDigitReturnCode();
			responseModel.Message = message ?? returnCode.GetDescription();
			return responseModel;
		}
	}
}
