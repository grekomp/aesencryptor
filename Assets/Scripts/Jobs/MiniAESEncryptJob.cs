using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Encryptor
{


	[BurstCompile]
	public struct MiniAESEncryptJob : IJobParallelFor
	{
		public struct Block
		{
			public int nib0;
			public int nib1;
			public int nib2;
			public int nib3;

			public int Value {
				get {
					return (nib0 << 12) + (nib1 << 8) + (nib2 << 4) + nib3;
				}
				set {
					nib3 = value & 0b1111;
					nib2 = (value >> 4) & 0b1111;
					nib1 = (value >> 8) & 0b1111;
					nib0 = (value >> 12) & 0b1111;
				}
			}
		}

		// Algorithm configuration data
		[ReadOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<int> encryptionSubstitutionTable;
		[ReadOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<int> decryptionSubstitutionTable;
		public int fieldSize;
		public int multiplicationModuloPolynomial;


		// Input data
		public bool encrypt;
		public int encryptionKey;
		public NativeArray<int> inputData;

		public void Execute(int index)
		{
			// Load data
			Block block = new Block();
			block.Value = inputData[index];

			Block key0 = new Block();
			key0.Value = encryptionKey;
			Block key1 = GetRoundKey(key0, 1);
			Block key2 = GetRoundKey(key1, 2);

			if (encrypt)
			{
				block = AddKey(block, key0);

				block = NibbleSub(block, encryptionSubstitutionTable);
				block = ShiftRows(block);
				block = MixColumns(block);

				block = AddKey(block, key1);

				block = NibbleSub(block, encryptionSubstitutionTable);
				block = ShiftRows(block);

				block = AddKey(block, key2);
			}
			else
			{
				block = AddKey(block, key2);

				block = NibbleSub(block, decryptionSubstitutionTable);
				block = ShiftRows(block);

				block = AddKey(block, key1);

				block = MixColumns(block);
				block = NibbleSub(block, decryptionSubstitutionTable);
				block = ShiftRows(block);

				block = AddKey(block, key0);
			}

			// Output data
			inputData[index] = block.Value;
		}


		#region Nibble operations
		public int NibbleAdd(int left, int right)
		{
			return left ^ right;
		}
		public int NibbleMul(int left, int right)
		{
			// Multiplication
			int mulResult = 0;
			for (int i = 0; i < fieldSize; i++)
			{
				if ((right & 1) > 0)
				{
					mulResult ^= left;
				}
				right >>= 1;
				left <<= 1;
			}

			// Modulo
			for (int i = 2 * fieldSize - 2; i >= fieldSize; i--)
			{
				if ((mulResult & (1 << i)) > 0)
				{
					mulResult ^= multiplicationModuloPolynomial << (i - fieldSize);
				}
			}

			return mulResult;
		}

		public int NibbleSub(int nibbleIn, NativeArray<int> substitutionTable)
		{
			return substitutionTable[nibbleIn];
		}
		#endregion


		#region Block operations
		public Block AddKey(Block block, Block key)
		{
			block.Value ^= key.Value;
			return block;
		}
		public Block NibbleSub(Block block, NativeArray<int> substitutionTable)
		{
			block.nib0 = NibbleSub(block.nib0, substitutionTable);
			block.nib1 = NibbleSub(block.nib1, substitutionTable);
			block.nib2 = NibbleSub(block.nib2, substitutionTable);
			block.nib3 = NibbleSub(block.nib3, substitutionTable);

			return block;
		}
		public Block ShiftRows(Block block)
		{
			int tempNib = block.nib1;
			block.nib1 = block.nib3;
			block.nib3 = tempNib;

			return block;
		}
		public Block MixColumns(Block block)
		{
			return new Block
			{
				nib0 = NibbleAdd(NibbleMul(3, block.nib0), NibbleMul(2, block.nib1)),
				nib1 = NibbleAdd(NibbleMul(2, block.nib0), NibbleMul(3, block.nib1)),
				nib2 = NibbleAdd(NibbleMul(3, block.nib2), NibbleMul(2, block.nib3)),
				nib3 = NibbleAdd(NibbleMul(2, block.nib2), NibbleMul(3, block.nib3))
			};
		}
		#endregion


		#region Key schedule
		public Block GetRoundKey(Block previousKey, int roundNumber)
		{
			switch (roundNumber)
			{
				case 0:
					return previousKey;
				case 1:
				case 2:
					Block key = new Block();
					key.nib0 = previousKey.nib0 ^ NibbleSub(previousKey.nib3, encryptionSubstitutionTable) ^ roundNumber;
					key.nib1 = NibbleAdd(previousKey.nib1, key.nib0);
					key.nib2 = NibbleAdd(previousKey.nib2, key.nib1);
					key.nib3 = NibbleAdd(previousKey.nib3, key.nib2);
					return key;
				default:
					return previousKey;
			}
		}
		#endregion
	}
}
