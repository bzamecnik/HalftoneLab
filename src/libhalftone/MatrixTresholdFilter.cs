// MatrixTresholdFilter.cs created with MonoDevelop
// User: bohous at 15:28 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
	public class MatrixTresholdFilter : TresholdFilter
	{
		private TresholdMatrix<int> _tresholdMatrix;
		
		public MatrixTresholdFilter(TresholdMatrix<int> matrix) {
			this._tresholdMatrix = matrix;
		}
		
		public override int treshold(Pixel pixel)
		{
			return _tresholdMatrix[pixel.Y, pixel.X];
		}
		
		public static TresholdMatrix<int> sampleMatrix;
		static MatrixTresholdFilter() {
			sampleMatrix = new TresholdMatrix<int>(2,2);
			sampleMatrix[0,0] = 51;
			sampleMatrix[1,1] = 102;
			sampleMatrix[0,1] = 153;
			sampleMatrix[1,0] = 204;
		}
 
		public class TresholdMatrix<T> {
			private T[,] _matrix;
			
			public TresholdMatrix(int width, int height) {
				_matrix = new T[height, width];
			}
			
			public TresholdMatrix(T[,] matrix) {
				_matrix = matrix;
			}
			
			public int Height {
				get { return _matrix.GetLength(0); }
			}			
			
			public int Width {
				get { return _matrix.GetLength(1); }
			}
			
			public T this[int y, int x] {
				get {
					return _matrix[y % Height, x % Width];
				}
				set {
					_matrix[y % Height, x % Width] = value;
				}
			}
		}
		
		public class Generator {
			public static TresholdMatrix<int> createFromIterativeMatrix(int[,] userMatrix) {
				int height = userMatrix.GetLength(0);
				int width = userMatrix.GetLength(1);
				double coeff = (double) 255 / (height * width + 1);
				TresholdMatrix<int> matrix = new TresholdMatrix<int>(width, height);
				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						matrix[y, x] = (int) (userMatrix[y, x] * coeff);
					}
				}
				return matrix;
			}
			
			// Create dispersed dot matrix of size 2^N x 2^N where N = magnitude
			// which is able to represent (2^N*2^N)-1 tones
			// Recursive algorithm is used
			public static TresholdMatrix<int> createBayerDispersedDotMatrix(int magnitude) {
				if ((magnitude < 0) || (magnitude > 4)) {
					return null;
				}
				if (magnitude == 0) {
					return createFromIterativeMatrix(new int[1,1]{{0}});
				} else {
					int[,] offsets = {{0,0},{1,1},{0,1},{1,0}};					
					int[,] matrix = new int[2,2]{{0,2},{3,1}};				
					int[,] newMatrix;
					for (int i = 1; i < magnitude; i++) {
						int oldSize = matrix.GetLength(0);
						newMatrix = new int[oldSize*2, oldSize*2];
						for (int quadrant = 0; quadrant < 4; quadrant++) {
							for (int y = 0; y < oldSize; y++) {
								for (int x = 0; x < oldSize; x++) {
									newMatrix[offsets[quadrant, 0]*oldSize + y,
									          offsets[quadrant, 1]*oldSize + x] =
										4 * matrix[y, x] + quadrant;
								}
							}
						}
						matrix = newMatrix;
					}
					for (int y = 0; y < matrix.GetLength(0); y++) {
						for (int x = 0; x < matrix.GetLength(1); x++) {
							matrix[y, x]++;
						}
					}
					return createFromIterativeMatrix(matrix);
				}
			}	
		}
	}
}
