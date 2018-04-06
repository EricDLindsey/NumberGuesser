using System.Linq;

namespace Network
{
    class Image
    {
		public Image(byte[] p, byte l, int w, int h)
		{
			label = l;
			pixels = p;
			width = w;
			height = h;
		}

		byte[] pixels;
		byte label;
		int width;
		int height;

		public Matrix Label
		{
			get
			{
				Matrix l = new Matrix(10, 1);
				l[label, 0] = 1;
				return l;
			}
		}

		public Matrix Matrix
		{
			get
			{
				float[] db = pixels.Select(x => ((float)x/255)).ToArray();
				return new Matrix(db);
			}
		}

		public override string ToString()
		{
			string s = "";

			for(int x = 0; x < width; x++)
			{
				for(int y = 0; y < height; y++)
				{
					if(pixels[x + width * y] == 0)
						s += " "; // White
					else if(pixels[x + width * y] == 255)
						s += "O"; // Black
					else
						s += "."; // Gray
				}
				s += "\n";
			}

			s += label.ToString();

			return s;
		}
	}
}
