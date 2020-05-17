using Athanor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Encryptor
{
	public class EncryptionController : MonoBehaviour
	{
		public enum EncryptorType
		{
			MiniAES,
			AES
		}

		[Header("Components")]
		public EncryptorMiniAES encryptorMiniAES;
		public EncryptorAES encryptorAES;


		#region File encryption
		public void ProcessFile(string inputPath, string outputPath, string keyString, bool encrypt, bool useMiniAES = false)
		{
			// Read source file contents
			byte[] fileContent = ReadFile(inputPath);
			if (fileContent == null) return;

			// Generate key
			byte[] key = Encoding.UTF8.GetBytes(keyString);

			// Encrypt file contents
			byte[] output = null;
			if (useMiniAES)
			{
				if (encrypt)
				{
					output = encryptorMiniAES.Encrypt(fileContent, key);
				}
				else
				{
					output = encryptorMiniAES.Decrypt(fileContent, key);
				}
			}
			else
			{
				output = encryptorAES.ProcessData(fileContent, key, encrypt);
			}

			// Write file contents
			File.WriteAllBytes(outputPath, output);
		}
		#endregion


		#region File helper methods
		private byte[] ReadFile(string path)
		{
			try
			{
				return File.ReadAllBytes(path);
			}
			catch (IOException e)
			{
				Log.Warning(this, $"The file could not be read: {e.Message}", this);
				return null;
			}
		}
		#endregion
	}
}
