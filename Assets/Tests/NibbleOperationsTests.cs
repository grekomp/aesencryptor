using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Encryptor.Tests
{
	public class NibbleOperationsTests
	{
		public static object[] NibbleMulTestCaseSource =
		{
			new object[] { 4, 0b10011, 0b1011, 0b0111, 0b0100 },
			new object[] { 4, 0b10011, 0b0011, 0b0011, 0b0101 },
			new object[] { 4, 0b10011, 0b1100, 0b1110, 0b0100 },
			new object[] { 4, 0b10011, 0b0000, 0b0000, 0b0000 },
			new object[] { 4, 0b10011, 0b1111, 0b1111, 0b1010 },

			new object[] { 3, 0b1011, 0b111, 0b101, 0b110 },
		};


		[Test, TestCaseSource("NibbleMulTestCaseSource")]
		public void NibbleMul_ShouldReturnTheMultipliedPolynomialModuloProvidedPolynomial(int fieldSize, int multiplicationModuloPolynomial, int left, int right, int result)
		{
			var job = new MiniAESEncryptJob()
			{
				fieldSize = fieldSize,
				multiplicationModuloPolynomial = multiplicationModuloPolynomial
			};

			Assert.That(job.NibbleMul(left, right), Is.EqualTo(result));
		}
	}
}
