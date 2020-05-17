using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encryptor.Tests
{
	public class AESEncryptorTests
	{
		//public static object[] MixColumns_Data = new object[]
		//{
		//	new object[] {
		//		new byte[]{ 219, 19, 83, 69, 242, 10, 34, 92, 1, 1, 1, 1, 198, 198, 198, 198 },
		//		new byte[] { 142, 77, 161, 188, 159, 220, 88, 157, 1, 1, 1, 1, 198, 198, 198, 198 }
		//	}
		//};
		//[Test, TestCaseSource("MixColumns_Data")]
		//public void MixColumns_ShouldReturnColumnsMixedAccordingToTestVectors(byte[] nibbles, byte[] expectedResult)
		//{
		//	Assert.That(AESEncryptJob.MixColumns(new AESEncryptJob.Block() { nibbles = nibbles }).nibbles, Is.EqualTo(expectedResult));
		//}

		//public static object[] MixColumnsInverse_Data = new object[]
		//{
		//	new object[] {
		//		new byte[] { 142, 77, 161, 188, 159, 220, 88, 157, 1, 1, 1, 1, 198, 198, 198, 198 },
		//		new byte[]{ 219, 19, 83, 69, 242, 10, 34, 92, 1, 1, 1, 1, 198, 198, 198, 198 },
		//	}
		//};
		//[Test, TestCaseSource("MixColumnsInverse_Data")]
		//public void MixColumnsInverse_ShouldReturnColumnsMixedAccordingToTestVectors(byte[] nibbles, byte[] expectedResult)
		//{
		//	Assert.That(AESEncryptJob.MixColumnsInverse(new AESEncryptJob.Block() { nibbles = nibbles }).nibbles, Is.EqualTo(expectedResult));
		//}


		//public static object[] ShiftRows_Data = new object[]
		//{
		//	new object[] {
		//		new byte[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
		//		new byte[] { 0, 5, 10, 15, 4, 9, 14, 3, 8, 13, 2, 7, 12, 1, 6, 11 }
		//	}
		//};
		//[Test, TestCaseSource("ShiftRows_Data")]
		//public void ShiftRows_ShouldReturnDataShiftedLeftByTheRowNumber(byte[] nibbles, byte[] expectedResult)
		//{
		//	Assert.That(AESEncryptJob.ShiftRows(new AESEncryptJob.Block() { nibbles = nibbles }).nibbles, Is.EqualTo(expectedResult));
		//}

		//public static object[] ShiftRowsInverse_Data = new object[]
		//{
		//	new object[] {
		//		new byte[] { 0, 5, 10, 15, 4, 9, 14, 3, 8, 13, 2, 7, 12, 1, 6, 11 },
		//		new byte[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
		//	}
		//};
		//[Test, TestCaseSource("ShiftRowsInverse_Data")]
		//public void ShiftRowsInverse_ShouldReturnDataShiftedRightByTheRowNumber(byte[] nibbles, byte[] expectedResult)
		//{
		//	Assert.That(AESEncryptJob.ShiftRowsInverse(new AESEncryptJob.Block() { nibbles = nibbles }).nibbles, Is.EqualTo(expectedResult));
		//}
	}
}