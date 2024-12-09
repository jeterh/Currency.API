using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Currency.API.Utilities.Helpers
{
	public static class SecurityHelper
	{
		public static string? AESEncrypt(string value, string key, string iv)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			try
			{
				byte[] sourceBytes = Encoding.ASCII.GetBytes(value);

				byte[] akey = Encoding.UTF8.GetBytes(key);
				byte[] aiv = Encoding.UTF8.GetBytes(iv);

				var aes = GetAes(akey, aiv);
				var transform = aes.CreateEncryptor();
				var encryptedBytes = transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
				var result = Convert.ToBase64String(encryptedBytes);

				return result;
			}
			catch
			{
				return null;
			}
		}

		public static string? AESDecrypt(string value, string key, string iv)
		{
			try
			{
				byte[] sourceBytes = Convert.FromBase64String(value);

				byte[] akey = Encoding.UTF8.GetBytes(key);
				byte[] aiv = Encoding.UTF8.GetBytes(iv);

				var aes = GetAes(akey, aiv);
				var transform = aes.CreateDecryptor();
				var decryptedBytes = transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
				var result = Encoding.UTF8.GetString(decryptedBytes);

				return result;
			}
			catch
			{
				return null;
			}
		}

		private static Aes GetAes(byte[] key, byte[] iv)
		{
			var aes = Aes.Create();
			aes.KeySize = 256;
			aes.BlockSize = 128;
			aes.Key = key;
			aes.IV = iv;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			return aes;
		}
	}
}
