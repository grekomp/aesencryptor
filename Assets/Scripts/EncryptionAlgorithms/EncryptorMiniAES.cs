using Athanor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Encryptor
{
	public class EncryptorMiniAES : MonoBehaviour
	{
		[Header("Algorithm configuration data")]
		int finiteFieldSize = 4;
		int moduloPolynomial = 0b10011;
		int[] encryptionSubstitutionTable = new int[] {
			0b1110, 0b0100, 0b1101, 0b0001,
			0b0010, 0b1111, 0b1011, 0b1000,
			0b0011, 0b1010, 0b0110, 0b1100,
			0b0101, 0b1001, 0b0000, 0b0111
		};
		int[] decryptionSubstitutionTable = new int[]
		{
			0b1110, 0b0011, 0b0100, 0b1000,
			0b0001, 0b1100, 0b1010, 0b1111,
			0b0111, 0b1101, 0b1001, 0b0110,
			0b1011, 0b0010, 0b0000, 0b0101
		};

		//public int encryptionKey = 0b1100_0011_1111_0000;

		#region Encryption & Decryption
		[ContextMenu("Execute")]
		public void Execute(ref int[] data, int key, bool encrypt)
		{
			NativeArray<int> nativeEncryptionSubstitutionTable = new NativeArray<int>(encryptionSubstitutionTable, Allocator.TempJob);
			NativeArray<int> nativeDecryptionSubstitutionTable = new NativeArray<int>(decryptionSubstitutionTable, Allocator.TempJob);
			NativeArray<int> nativeData = new NativeArray<int>(data, Allocator.TempJob);

			MiniAESEncryptJob job = new MiniAESEncryptJob()
			{
				encryptionSubstitutionTable = nativeEncryptionSubstitutionTable,
				decryptionSubstitutionTable = nativeDecryptionSubstitutionTable,
				fieldSize = finiteFieldSize,
				multiplicationModuloPolynomial = moduloPolynomial,

				encrypt = encrypt,
				inputData = nativeData,
				encryptionKey = key,
			};
			job.Schedule(nativeData.Length, 1).Complete();

			data = nativeData.ToArray();

			nativeData.Dispose();
			nativeEncryptionSubstitutionTable.Dispose();
			nativeDecryptionSubstitutionTable.Dispose();
		}

		public byte[] Encrypt(byte[] data, byte[] key)
		{
			int[] dataBlocks = Generate16BitDataBlocks(data);

			int keyInt = key[0] << 8 + key[1];
			Execute(ref dataBlocks, keyInt, true);

			return GetByteArrayFrom16BitDataBlocks(dataBlocks);
		}

		public byte[] Decrypt(byte[] data, byte[] key)
		{
			int[] dataBlocks = Generate16BitDataBlocks(data);

			int keyInt = key[0] << 8 + key[1];
			Execute(ref dataBlocks, keyInt, false);

			return GetByteArrayFrom16BitDataBlocks(dataBlocks);
		}
		#endregion


		#region Data transformation
		public static int[] Generate16BitDataBlocks(byte[] data)
		{
			int[] dataBlocks = new int[data.Length / 2 + data.Length % 2];

			for (int i = 0; i * 2 < data.Length; i++)
			{
				dataBlocks[i] = data[i * 2] << 8;

				if ((i * 2 + 1) < data.Length)
				{
					dataBlocks[i] += data[i * 2 + 1];
				}
			}

			return dataBlocks;
		}
		/// <summary>
		/// Transforms the 16 bit data blocks into 8-bit byte array.
		/// </summary>
		/// <param name="byteArrayLength">The lenght of the output array. Defaults to dataBlocks.Length * 2</param>
		public static byte[] GetByteArrayFrom16BitDataBlocks(int[] dataBlocks)
		{
			bool hasEvenCharactersNumber = (dataBlocks.Last() & 0b11111111) > 0;
			byte[] output = new byte[hasEvenCharactersNumber ? dataBlocks.Length * 2 : dataBlocks.Length * 2 - 1];

			int i = 0;
			while (i < dataBlocks.Length && i * 2 < output.Length)
			{
				output[2 * i] = unchecked((byte)(dataBlocks[i] >> 8));
				if (2 * i + 1 < output.Length)
				{
					output[2 * i + 1] = unchecked((byte)dataBlocks[i]);
				}

				i++;
			}

			return output;
		}
		#endregion
	}
}
