using System;
using System.Linq;

namespace Network
{
	class Matrix
	{
		public Matrix(int x, int y)
		{
			matrix = new float[x, y];
			_x = x;
			_y = y;
		}

		public Matrix(float[] a)
		{
			matrix = new float[a.Length, 1];
			_x = a.Length;
			_y = 1;

			for(int i = 0; i < _x; i++)
				matrix[i, 0] = a[i];
		}

		private float[,] matrix;
		private int _x;
		private int _y;

		public int X { get { return _x; } }
		public int Y { get { return _y; } }

		public float this[int x, int y]
		{
			get
			{
				return matrix[x, y];
			}

			set
			{
				matrix[x, y] = value;
			}
		}

		public Matrix Transpose()
		{
			int nx = _y;
			int ny = _x;
			Matrix retMatrix = new Matrix(nx, ny);

			for(int x = 0; x < nx; x++)
				for(int y = 0; y < ny; y++)
					retMatrix.matrix[x, y] = matrix[y, x];

			return retMatrix;
		}

		public Matrix Max()
		{
			Matrix retMatrix = new Matrix(_x, _y);
			float max = matrix.Cast<float>().Max();
			Tuple<int, int> index = indexOf(max);
			retMatrix.matrix[index.Item1, index.Item2] = 1;

			return retMatrix;
		}

		public Matrix Sigmoid()
		{
			Matrix retMatrix = new Matrix(_x, _y);

			for(int x = 0; x < _x; x++)
				for(int y = 0; y < _y; y++)
					retMatrix.matrix[x, y] = sigmoid(matrix[x, y]);

			return retMatrix;
		}

		public Matrix SigmoidPrime()
		{
			Matrix retMatrix = new Matrix(_x, _y);

			for(int x = 0; x < _x; x++)
				for(int y = 0; y < _y; y++)
					retMatrix.matrix[x, y] = sigmoidPrime(matrix[x, y]);

			return retMatrix;
		}

		public static Matrix Zero(int X, int Y)
		{
			Matrix retMatrix = new Matrix(X, Y);

			for(int x = 0; x < X; x++)
				for(int y = 0; y < Y; y++)
					retMatrix.matrix[x, y] = 0;

			return retMatrix;
		}

		public static Matrix Random(int x, int y, float range = 2f)
		{
			Matrix retMatrix = new Matrix(x, y);
			Random r = new Random();

			for(int i = 0; i < x; i++)
				for(int k = 0; k < y; k++)
					retMatrix.matrix[i, k] = (float)((r.NextDouble() * range) * 2.0f - range);

			return retMatrix;
		}

		public static Matrix Dot(Matrix a, Matrix b)
		{
			if(a.Y != b.X)
			{
				Console.WriteLine("(" + a.X + ", " + a.Y + ") dot (" + b.X + ", " + b.Y + ") is not valid.");
				return null;
			}

			Matrix retMatrix = new Matrix(a.X, b.Y);
			retMatrix.dot(a.matrix, b.matrix);

			return retMatrix;
		}

		public static Matrix operator +(Matrix a, Matrix b)
		{
			if(a.X != b.X && a.Y != b.Y)
			{
				Console.WriteLine("(" + a.X + ", " + a.Y + ") + (" + b.X + ", " + b.Y + ") is not valid.");
				return null;
			}

			Matrix retMatrix = new Matrix(a.X, a.Y);
			retMatrix.add(a.matrix, b.matrix);

			return retMatrix;
		}

		public static Matrix operator -(Matrix a, Matrix b)
		{
			if(a.X != b.X && a.Y != b.Y)
			{
				Console.WriteLine("(" + a.X + ", " + a.Y + ") - (" + b.X + ", " + b.Y + ") is not valid.");
				return null;
			}

			Matrix retMatrix = new Matrix(a.X, a.Y);
			retMatrix.sub(a.matrix, b.matrix);

			return retMatrix;
		}

		public static Matrix operator *(float a, Matrix b)
		{
			Matrix retMatrix = b;
			retMatrix.multiply(a);

			return retMatrix;
		}

		public static Matrix operator *(Matrix a, Matrix b)
		{
			Matrix retMatrix;

			if(a.X != b.X && (a.Y == 1 || b.Y == 1))
			{
				Console.WriteLine("(" + a.X + ", " + a.Y + ") * (" + b.X + ", " + b.Y + ") is not valid.");
				return null;
			}

			if(a.Y == 1)
			{
				retMatrix = new Matrix(b.X, b.Y);
				retMatrix.multiply(b.matrix, a.matrix);
			}
			else
			{
				retMatrix = new Matrix(a.X, a.Y);
				retMatrix.multiply(a.matrix, b.matrix);
			}

			return retMatrix;
		}

		public static bool operator ==(Matrix a, Matrix b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Matrix a, Matrix b)
		{
			return a.Equals(b);
		}

		public override bool Equals(object obj)
		{
			return (obj.ToString() == ToString());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			string s = "";

			for(int x = 0; x < _x; x++)
			{
				s += "[";
				for(int y = 0; y < _y; y++)
				{
					float max = columnWidth(y);
					s += String.Format((matrix[x, y] >= 0.0f && matrix[x, y] != max ? " " : "") +
						"{0,-" + ((max.ToString().Length +
						(max.ToString()[0] == '-' ? 0 : 1)) -
						(matrix[x, y] >= 0.0f ? 1 : 0)) + "}" +
						(y < (_y - 1) ? " " : ""), matrix[x, y]);
					
					if(_y > 15 && y == 2)
					{
						s += "... ";
						y = _y - 4;
					}
				}
				s += "]" + (x < _x - 1 ? "\n" : "");

				if(_x > 50 && x == 10)
				{
					s += "...\n";
					x = _x - 12;
				}
			}

			return s;
		}

		//// Private methods

		float columnWidth(int col)
		{
			float max = 0;

			for(int i = 0; i < _x; i++)
				if(matrix[i, col].ToString().Length > max.ToString().Length)
					max = matrix[i, col];

			return max;
		}

		float[,] empty(int x, int y)
		{
			float[,] a = new float[x, y];

			for(int i = 0; i < x; i++)
				for(int k = 0; k < y; k++)
					a[i, k] = 0;

			return a;
		}

		Tuple<int, int> indexOf(float a)
		{
			for(int x = 0; x < _x; x++)
				for(int y = 0; y < _y; y++)
					if(matrix[x, y] == a)
						return Tuple.Create(x, y);

			return Tuple.Create(-1, -1);
		}

		private float sigmoid(float z)
		{
			return 1f / (1f + (float)Math.Exp(-z));
		}

		private float sigmoidPrime(float z)
		{
			return sigmoid(z) * (1f - sigmoid(z));
		}

		void dot(float[,] a, float[,] b)
		{
			float sum;
			int ay = a.GetLength(1);

			for(int x = 0; x < _x; x++)
			{
				for(int y = 0; y < _y; y++)
				{
					sum = 0f;

					for(int i = 0; i < ay; i++)
						sum += a[x, i] * b[i, y];

					matrix[x, y] = sum;
				}
			}
		}

		void add(float[,] a, float[,] b)
		{
			for(int x = 0; x < _x; x++)
				for(int y = 0; y < _y; y++)
					matrix[x, y] = a[x, y] + b[x, y];
		}

		void sub(float[,] a, float[,] b)
		{
			for(int x = 0; x < _x; x++)
				for(int y = 0; y < _y; y++)
					matrix[x, y] = a[x, y] - b[x, y];
		}

		void multiply(float a)
		{
			for(int x = 0; x < _x; x++)
				for(int y = 0; y < _y; y++)
					matrix[x, y] *= a;
		}

		void multiply(float[,] a, float[,] b)
		{
			for(int y = 0; y < _y; y++)
				for(int x = 0; x < _x; x++)
					matrix[x, y] = a[x, y] * b[x, 0];
		}
	}
}