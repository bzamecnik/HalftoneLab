// MatrixTresholdFilter.cs created with MonoDevelop
// User: bohous at 23:35 26.3.2009
//

using System;
//using NUnit.Framework;

namespace Halftone
{
	
	
	//[TestFixture()]
	public class MatrixTresholdFilterTest
	{
		
		//[Test()]
		public void createFromIterativeMatrix()
		{
			int[,] iterMatrix = new int[3,3]{{9,5,8},{4,1,2},{6,3,7}};
			for (int y = 0; y < iterMatrix.GetLength(0); y++) {
				for (int x = 0; x < iterMatrix.GetLength(0); x++) {
					Console.Write("{0} ", iterMatrix[y, x]);
				}
				Console.WriteLine();
			}
			MatrixTresholdFilter.TresholdMatrix<int> matrix =
				MatrixTresholdFilter.Generator.createFromIterativeMatrix(iterMatrix);
			for (int y = 0; y < matrix.Height; y++) {
				for (int x = 0; x < matrix.Width; x++) {
					Console.Write("{0} ", matrix[y, x]);
				}
				Console.WriteLine();
			}
		}
		
		//[Test()]
		public void createDispersedDotMatrix() {
			MatrixTresholdFilter.TresholdMatrix<int> matrix =
				MatrixTresholdFilter.Generator.createDispersedDotMatrix(3);
			for (int y = 0; y < matrix.Height; y++) {
				for (int x = 0; x < matrix.Width; x++) {
					Console.Write("{0} ", matrix[y, x]);
				}
				Console.WriteLine();
			}
		}
	}
}
