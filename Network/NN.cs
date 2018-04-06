using System;
using System.Collections.Generic;

namespace Network
{
    class NN
    {
		public NN(int[] size)
		{
			sizes = size;
			biases = new Matrix[sizes.Length - 1];
			weights = new Matrix[sizes.Length - 1];

			for(int i = 1; i < sizes.Length; i++)
				biases[i - 1] = Matrix.Random(sizes[i], 1);

			for(int i = 0; i < sizes.Length - 1; i++)
				weights[i] = Matrix.Random(sizes[i + 1], sizes[i]);
		}

		int[] sizes;
		Matrix[] biases;
		Matrix[] weights;

		public Matrix[] Biases { get { return biases; } }
		public Matrix[] Weights { get { return weights; } }

		public void Calc(Image[] trainingData, int batchSize, Image[] testData = null, int epochs = 30, float lr = 3.0f)
		{
			for(int i = 0; i < epochs; i++)
			{
				Console.WriteLine("Running Epoch " + (i + 1));
				shuffle(ref trainingData);

				List<Image[]> batches = split(trainingData, batchSize);

				Console.WriteLine("Learning...");
				foreach(Image[] batch in batches)
					updateBatch(batch, lr);

				if(testData != null)
				{
					Console.WriteLine("Testing...");
					Console.WriteLine("Epoch " + (i + 1) + ": " + evaluate(testData) + " / " + testData.Length);
				}
				else
					Console.WriteLine("Epoch " + (i + 1) + " complete.");
			}
		}

		public override string ToString()
		{
			string s = "";

			s += "Sizes: {";
			foreach(int i in sizes)
				s += i + " ";
			s += "}\n\n";

			s += "Biases:\n";
			foreach(Matrix b in biases)
				s += b.ToString() + "\n";

			s += "Weights:\n";
			foreach(Matrix w in weights)
				s += w.ToString() + "\n";

			return s;
		}

		//// Private methods

		int evaluate(Image[] testData)
		{
			int sum = 0;

			foreach(Image test in testData)
				if(feedForward(test.Matrix).Max() == test.Label)
					sum++;

			return sum;
		}

		Matrix feedForward(Matrix input)
		{
			for(int i = 0; i < biases.Length; i++)
				input = (Matrix.Dot(weights[i], input) + biases[i]).Sigmoid();

			return input;
		}

		private void updateBatch(Image[] batch, float lr)
		{
			Matrix[] nbiases = empty(biases);
			Matrix[] nweights = empty(weights);

			foreach(Image image in batch)
			{
				Matrix[] deltaBiases, deltaWeights;

				backprop(image.Matrix, image.Label, out deltaBiases, out deltaWeights);

				for(int i = 0; i < nbiases.Length; i++)
					nbiases[i] += deltaBiases[i];

				for(int i = 0; i < nweights.Length; i++)
					nweights[i] += deltaWeights[i];
			}

			for(int i = 0; i < biases.Length; i++)
				biases[i] = biases[i] - (lr / batch.Length) * nbiases[i];

			for(int i = 0; i < weights.Length; i++)
				weights[i] = weights[i] - (lr / batch.Length) * nweights[i];
		}

		void backprop(Matrix input, Matrix label, out Matrix[] deltaBiases, out Matrix[] deltaWeights)
		{
			deltaBiases = empty(biases);
			deltaWeights = empty(weights);

			Matrix activation = input;
			List<Matrix> activations = new List<Matrix>();
			activations.Add(input);
			List<Matrix> zs = new List<Matrix>();

			for(int i = 0; i < biases.Length; i++)
			{
				Matrix z = Matrix.Dot(weights[i], activation) + biases[i];
				zs.Add(z);
				activation = z.Sigmoid();
				activations.Add(activation);
			}

			Matrix delta = costDerivative(activations[activations.Count - 1], label) * zs[zs.Count - 1].SigmoidPrime();
			deltaBiases[deltaBiases.Length - 1] = delta;
			deltaWeights[deltaWeights.Length - 1] = Matrix.Dot(delta, activations[activations.Count - 2].Transpose());

			for(int i = 2; i < sizes.Length; i++)
			{
				Matrix z = zs[zs.Count - i];
				Matrix sp = z.SigmoidPrime();
				delta = Matrix.Dot(weights[weights.Length - i + 1].Transpose(), delta) * sp;
				deltaBiases[deltaBiases.Length - i] = delta;
				deltaWeights[deltaWeights.Length - i] = Matrix.Dot(delta, activations[activations.Count - i - 1].Transpose());
			}
		}

		Matrix costDerivative(Matrix output, Matrix label)
		{
			return output - label;
		}

		void shuffle(ref Image[] img)
		{
			Random r = new Random();

			for(int i = 0; i < img.Length; i++)
			{
				Image tmp = img[i];
				int p = r.Next(i, img.Length);
				img[i] = img[p];
				img[p] = tmp;
			}
		}

		List<Image[]> split(Image[] training, int batchSize)
		{
			List<Image[]> ret = new List<Image[]>();

			for(int i = 0; i < training.Length; i += batchSize)
			{
				Image[] g = new Image[batchSize];

				if(training.Length < i + batchSize)
					batchSize = training.Length - i;

				Array.Copy(training, i, g, 0, batchSize);
				ret.Add(g);
			}

			return ret;
		}

		Matrix[] empty(Matrix[] a)
		{
			Matrix[] ret = new Matrix[a.Length];

			for(int i = 0; i < ret.Length; i++)
				ret[i] = Matrix.Zero(a[i].X, a[i].Y);

			return ret;
		}
	}
}
