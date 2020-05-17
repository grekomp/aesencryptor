using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Encryptor
{
	public struct AESEncryptJob : IJobParallelFor
	{
		public struct Block
		{
			public byte[] nibbles;

			public byte this[int row, int column] {
				get { return nibbles[row + column * 4]; }
				set { nibbles[row + column * 4] = value; }
			}
		}

		// Algorithm configuration data
		[ReadOnly, NativeDisableParallelForRestriction]
		public NativeArray<byte> encryptionSubstitutionTable;
		[ReadOnly, NativeDisableParallelForRestriction]
		public NativeArray<byte> decryptionSubstitutionTable;
		public bool encrypt;


		// Input data
		[ReadOnly, NativeDisableParallelForRestriction]
		public NativeArray<byte> expandedKey;
		[NativeDisableParallelForRestriction]
		public NativeArray<byte> inputData;


		public void Execute(int index)
		{
			// Read data
			Block block = new Block
			{
				nibbles = new byte[16],
			};
			for (int i = 0; i < 16; i++)
			{
				block.nibbles[i] = inputData[index * 16 + i];
			}


			if (encrypt)
			{
				// Initial key addition
				block = AddKey(block, expandedKey, 0);

				// Rounds
				for (int i = 1; i < 10; i++)
				{
					block = SubBytes(block, encryptionSubstitutionTable);
					block = ShiftRows(block);
					block = MixColumns(block);
					block = AddKey(block, expandedKey, i);
				}

				// Final round
				block = SubBytes(block, encryptionSubstitutionTable);
				block = ShiftRows(block);
				block = AddKey(block, expandedKey, 10);
			}
			else
			{
				// Initial key addition
				block = AddKey(block, expandedKey, 10);

				// Rounds
				for (int i = 1; i < 10; i++)
				{
					block = ShiftRowsInverse(block);
					block = SubBytes(block, decryptionSubstitutionTable);
					block = AddKey(block, expandedKey, 10 - i);
					block = MixColumnsInverse(block);
				}

				// Final round
				block = ShiftRowsInverse(block);
				block = SubBytes(block, decryptionSubstitutionTable);
				block = AddKey(block, expandedKey, 0);
			}


			// Write output
			for (int i = 0; i < 16; i++)
			{
				inputData[index * 16 + i] = block.nibbles[i];
			}
		}


		#region Nibble operations
		public static byte Add(byte left, byte right)
		{
			return (byte)(left ^ right);
		}
		public static byte Multiply(byte left, byte right)
		{
			byte result = 0;

			for (int counter = 0; counter < 8; counter++)
			{
				if ((right & 1) != 0)
				{
					result ^= left;
				}

				bool hi_bit_set = (left & 0x80) != 0;
				left <<= 1;
				if (hi_bit_set)
				{
					left ^= 0x1B; /* x^8 + x^4 + x^3 + x + 1 */
				}
				right >>= 1;
			}

			return result;
		}
		public static byte SubByte(byte input, NativeArray<byte> substitutionTable)
		{
			return substitutionTable[input];
		}
		#endregion


		#region Block operations
		public static Block AddKey(Block block, NativeArray<byte> expandedKeys, int roundNumber)
		{
			for (int i = 0; i < 16; i++)
			{
				block.nibbles[i] = Add(block.nibbles[i], expandedKeys[roundNumber * 16 + i]);
			}

			return block;
		}

		public static Block SubBytes(Block block, NativeArray<byte> substitutionTable)
		{
			for (int i = 0; i < block.nibbles.Length; i++)
			{
				block.nibbles[i] = SubByte(block.nibbles[i], substitutionTable);
			}

			return block;
		}

		public static Block ShiftRows(Block block)
		{
			Block result = new Block();
			result.nibbles = new byte[16];

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					result[i, j] = block[i, (j + i) % 4];
				}
			}

			return result;
		}
		public static Block ShiftRowsInverse(Block block)
		{
			Block result = new Block();
			result.nibbles = new byte[16];

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					result[i, j] = block[i, (4 + (j - i)) % 4];
				}
			}

			return result;
		}

		public static Block MixColumns(Block block)
		{
			Block result = new Block();
			result.nibbles = new byte[16];

			for (int column = 0; column < 4; column++)
			{
				result[0, column] = (byte)(Multiply(0x02, block[0, column]) ^ Multiply(0x03, block[1, column]) ^ block[2, column] ^ block[3, column]);
				result[1, column] = (byte)(block[0, column] ^ Multiply(0x02, block[1, column]) ^ Multiply(0x03, block[2, column]) ^ block[3, column]);
				result[2, column] = (byte)(block[0, column] ^ block[1, column] ^ Multiply(0x02, block[2, column]) ^ Multiply(0x03, block[3, column]));
				result[3, column] = (byte)(Multiply(0x03, block[0, column]) ^ block[1, column] ^ block[2, column] ^ Multiply(0x02, block[3, column]));
			}

			return result;
		}
		public static Block MixColumnsInverse(Block block)
		{
			Block result = new Block();
			result.nibbles = new byte[16];

			for (int column = 0; column < 4; column++)
			{
				result[0, column] = (byte)(Multiply(0x0e, block[0, column]) ^ Multiply(0x0b, block[1, column]) ^ Multiply(0x0d, block[2, column]) ^ Multiply(0x09, block[3, column]));
				result[1, column] = (byte)(Multiply(0x09, block[0, column]) ^ Multiply(0x0e, block[1, column]) ^ Multiply(0x0b, block[2, column]) ^ Multiply(0x0d, block[3, column]));
				result[2, column] = (byte)(Multiply(0x0d, block[0, column]) ^ Multiply(0x09, block[1, column]) ^ Multiply(0x0e, block[2, column]) ^ Multiply(0x0b, block[3, column]));
				result[3, column] = (byte)(Multiply(0x0b, block[0, column]) ^ Multiply(0x0d, block[1, column]) ^ Multiply(0x09, block[2, column]) ^ Multiply(0x0e, block[3, column]));
			}

			return result;
		}
		#endregion
	}
}
