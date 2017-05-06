using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Depa
{
    public static unsafe class Depa
    {
        /// <summary>
        /// Size of the depacking code.
        /// </summary>
        private const short DEPA_SIZE = 0x1A60;

        /// <summary>
        /// Current process module self-reference.
        /// </summary>
        private static readonly ProcessModule PROC_REF = Process.GetCurrentProcess().MainModule;

        private static void Main()
        {
            var currentProcessBytes = File.ReadAllBytes(PROC_REF.FileName);
            var procBufferBytes = new byte[currentProcessBytes.LongLength - DEPA_SIZE];
            var isMasked = currentProcessBytes[0x1A50].Equals(0);

            Array.Copy(currentProcessBytes, DEPA_SIZE, procBufferBytes, 0, currentProcessBytes.LongLength - DEPA_SIZE);

            string randomNameString =
                    isMasked
                        ? Decompress(procBufferBytes)
                        : Decompress(procBufferBytes, true, currentProcessBytes[0x1A50]);
            Process.Start(randomNameString + ".exe");
        }

        /// <summary>
        /// Inflates supplied program bytes, writes to disk as executable.
        /// </summary>
        /// <param name="buf">Raw byte array representing the program to be written to disk.</param>
        /// <param name="mask">Optional: boolean indicating whether or not raw program bytes have been
        /// somehow masked.</param>
        /// <param name="xor">The mask value.</param>
        /// <returns>The randomized name of the program executable.</returns>
        private static string Decompress(byte[] buf, bool mask = false, int xor = 0)
        {
            var ran = RandomString();
            if (!mask)
            {
                try
                {
                    using (var bufMemStream = new MemoryStream(buf))
                    using (var decompressedFileStream = File.Create(ran + ".exe"))
                    using (var decompressionStream = new DeflateStream(bufMemStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                using (FileStream decompressedFileStream = File.Create(ran + ".exe"))
                using (var xorStream = new MemoryStream(buf))
                {
                    var xorBytes = xorStream.ToArray();

                    for (var i = 0; i < xorStream.Length; i++)
                        xorBytes[i] = Convert.ToByte(xorBytes[i] ^ xor);

                    using (var decompressionStream =
                        new DeflateStream(new MemoryStream(xorBytes), CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
            return ran;
        }

        /// <summary>
        /// Selects and returns a randomized alphanumeric string of an arbitrary length.
        /// </summary>
        /// <param name="length">Size of the random string. Defaults to 5.</param>
        /// <returns>A random alphanumeric string of length length.</returns>
        private static string RandomString(int length = 5)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var ran = new Random();

            return new string(Enumerable.Repeat(chars, length).Select(s => s[ran.Next(s.Length)]).ToArray());
        }
    }
}
