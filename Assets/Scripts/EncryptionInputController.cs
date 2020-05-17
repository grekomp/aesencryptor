using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encryptor
{
	public class EncryptionInputController : MonoBehaviour
	{
		[Header("Components")]
		public EncryptionController encryptionController;

		[Header("Variables")]
		public StringReference inputPath;
		public StringReference outputPath;
		public BoolReference useMiniAESEncryption;
		public StringReference key;

		#region Encryption methods
		[ContextMenu("Encrypt file")]
		public void EncryptFile()
		{
			encryptionController.ProcessFile(inputPath, outputPath, key, true, useMiniAESEncryption);
		}
		[ContextMenu("Decrypt file")]
		public void DecryptFile()
		{
			encryptionController.ProcessFile(inputPath, outputPath, key, false, useMiniAESEncryption);
		}
		#endregion
	}
}
