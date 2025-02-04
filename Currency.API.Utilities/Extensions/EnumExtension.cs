﻿using System.ComponentModel;

namespace Currency.API.Utilities.Extensions
{
	public static class EnumExtension
	{
		public static string GetFourDigitReturnCode(this Enum theEnum)
		{
			return theEnum.GetHashCode().ToString("D4");
		}

		public static string? GetDescription(this Enum theEnum)
		{
			var type = theEnum.GetType();
			var name = System.Enum.GetName(type, theEnum);
			if (name != null)
			{
				var field = type.GetField(name);
				if (field == null) return null;
				if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
				{
					return attr.Description;
				}
			}
			return null;
		}
	}
}
