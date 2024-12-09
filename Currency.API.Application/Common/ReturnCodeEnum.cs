using System.ComponentModel;

namespace Currency.API.Application.Common
{
	public enum ReturnCodeEnum
	{
		[Description("成功")]
		Success = 0,

		[Description("失敗")]
		Fail = 9997,

		[Description("驗證失敗")]
		AuthenticationFail = 9998,

		[Description("未知錯誤")]
		UnKnown = 9999
	}
}
