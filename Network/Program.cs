using System;
using System.IO;

namespace Network
{
    class Program
    {
		static void Main(string[] args)
		{
			new Save().start();
        }
	}

	class Save
	{
		public void start()
		{
			nn = new NN(new int[] { 784, 30, 10 });

			nn.Calc(MNIST.GetTrainingData(), 30, MNIST.GetTestData(), 1);
			Console.WriteLine("Done");

			//string csv = parseNetwork();

			//writeFile(csv);

			Console.ReadLine();
		}

		NN nn;

		void writeFile(string csv)
		{
			try
			{
				StreamWriter sw = new StreamWriter("Values.txt");

				sw.WriteLine(csv);

				sw.Close();
			}
			catch(Exception e)
			{
				Console.WriteLine("Could not write file. " + e.Message);
			}

		}

		string parseNetwork()
		{
			string csv = "";

			Matrix[] biases = nn.Biases;
			Matrix[] weights = nn.Weights;

			for(int i = 0; i < 30; i++)
				csv += biases[0][i, 0].ToString() + ",";

			for(int i = 0; i < 10; i++)
				csv += biases[1][i, 0].ToString() + ",";

			for(int i = 0; i < 30; i++)
				for(int k = 0; k < 784; k++)
					csv += weights[0][i, k].ToString() + ",";

			for(int i = 0; i < 10; i++)
				for(int k = 0; k < 30; k++)
					csv += weights[1][i, k].ToString() + ",";
			csv.Remove(csv.Length - 1);
			return csv;
		}
	}
}
