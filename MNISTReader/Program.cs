using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNISTReader
{
    class Program
    {
        // http://yann.lecun.com/exdb/mnist/index.html

        private static int SwapEndianness(int value)
        {
            var b1 = (value >> 0) & 0xff;
            var b2 = (value >> 8) & 0xff;
            var b3 = (value >> 16) & 0xff;
            var b4 = (value >> 24) & 0xff;

            return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
        }

        // http://paulbourke.net/dataformats/asciiart/
        // black -> white
        private static readonly string greyScale = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\" ^`'. ";

        private static char GetCharFromGreyScale(byte value)
        {
            var valueComplement = 255 - value;
            return greyScale[Math.Max(0, Convert.ToInt32(Math.Round(greyScale.Length / 255.0 * valueComplement)) - 1)];
        }

        private static string Print(byte[] input)
        {
            StringBuilder strBldr = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                strBldr.Append(GetCharFromGreyScale(input[i]));
            }

            return strBldr.ToString();
        }


        static void Main(string[] args)
        {
            var inputFile = "data\\t10k-images.idx3-ubyte"; 
            using (var inputStream = File.OpenRead(inputFile))
            {
                byte[] buffer = new byte[4];
                var bytesRead = inputStream.Read(buffer, 0, 4);
                var magicNumber = SwapEndianness(BitConverter.ToInt32(buffer, 0));
                Console.WriteLine($"Magic number: {magicNumber}");


                bytesRead = inputStream.Read(buffer, 0, 4);
                var numberOfImages = SwapEndianness(BitConverter.ToInt32(buffer, 0));
                Console.WriteLine($"Number of images: {numberOfImages}");

                bytesRead = inputStream.Read(buffer, 0, 4);
                var numberOfRows = SwapEndianness(BitConverter.ToInt32(buffer, 0));
                bytesRead = inputStream.Read(buffer, 0, 4);
                var numberOfColumns = SwapEndianness(BitConverter.ToInt32(buffer, 0));
                Console.WriteLine($"Image size: {numberOfRows}x{numberOfColumns}");

                buffer = new byte[numberOfColumns];

                for (int i = 0; i < numberOfImages; i++)
                {
                    // read images
                    for (int j = 0; j < numberOfRows; j++)
                    {
                        bytesRead = inputStream.Read(buffer, 0, numberOfColumns);
                        Console.WriteLine(Print(buffer));
                    }

                    // just for giving us time to view the output
                    System.Threading.Thread.Sleep(200);
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

        }
    }
}
