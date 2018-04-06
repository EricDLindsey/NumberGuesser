using System;
using System.IO;

namespace Network
{
    static class MNIST
    {
		private static Image[] trainingData;
		private static Image[] testData;

		static public Image[] GetTrainingData()
		{
			if(trainingData == null)
				trainingData = getData("train-images.idx3-ubyte", "train-labels.idx1-ubyte");
			return trainingData;
		}

		static public Image[] GetTestData()
		{
			if(testData == null)
				testData = getData("t10k-images.idx3-ubyte", "t10k-labels.idx1-ubyte");
			return testData;
		}

		static Image[] getData(string imageName, string labelName)
		{
			FileStream ifsImages = new FileStream(imageName, FileMode.Open);
			FileStream ifsLabels = new FileStream(labelName, FileMode.Open);

			BinaryReader brImages = new BinaryReader(ifsImages);
			BinaryReader brLabels = new BinaryReader(ifsLabels);

			int magicI = ReadBigInt32(brImages); // Throw away
			int numImages = ReadBigInt32(brImages);
			int numRows = ReadBigInt32(brImages);
			int numCols = ReadBigInt32(brImages);

			int magicL = ReadBigInt32(brLabels); // Throw away
			int numLabels = ReadBigInt32(brLabels);

			Image[] images = new Image[numImages];

			for(int i = 0; i < numImages; i++)
			{
				byte[] pixels = new byte[numRows * numCols];

				for(int x = 0; x < numRows; x++)
					for(int y = 0; y < numCols; y++)
						pixels[x + numRows * y] = brImages.ReadByte();

				byte l = brLabels.ReadByte();

				Image img = new Image(pixels, l, numRows, numCols);

				images[i] = img;
			}

			ifsImages.Close();
			ifsLabels.Close();

			return images;
		}

		// Conversion for little endian ints
		static int ReadBigInt32(this BinaryReader br)
		{
			byte[] bytes = br.ReadBytes(sizeof(Int32));
			if(BitConverter.IsLittleEndian)
				Array.Reverse(bytes);
			return BitConverter.ToInt32(bytes, 0);
		}
    }
}
