using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console
{
    internal class DSA
    {
        int h0 = 100;

        public static ushort Power(int a, int n, int p)
        {
            // Initialize result
            ushort res = 1;

            // Update 'a' if 'a' >= p
            a = a % p;
            uint k = (uint)a;
            while (n > 0)
            {
                // If n is odd, multiply 'a' with result
                if ((n & 1) == 1)
                    res = (ushort)((res * k) % p);

                // n must be even now
                n = n >> 1; // n = n/2
                k = (uint)(k * k % p);
            }

            return (ushort)(res % p);
        }

        public static bool IsPrime(int n, int k)
        {
            // Corner cases
            if (n <= 1 || n == 4) return false;
            if (n <= 3) return true;

            // Try k times
            while (k > 0)
            {
                // Pick a random number in [2..n-2]    
                // Above corner cases make sure that n > 4
                Random rand = new Random();
                int a = 2 + (int)(rand.Next() % (n - 4));

                // Fermat's little theorem
                if (Power(a, n - 1, n) != 1)
                    return false;

                k--;
            }

            return true;
        }

        static int HashFunc(int h, int m, int q)
        {
            return Power((h+m), 2, q);
        }

        public static int GetHash(byte[] data, int mod)
        {
            int h = 100;
            foreach (byte m in data)
            {
                h = HashFunc(h, m, mod);
            }
            return h;
        }

        public static (int, int, int) GetSignature(byte[] data, int q, int p, int x, int k, int g)
        {
            int h = GetHash(data, q);
            int r = Power(g, k, p) % q;
            int s = (Power(k, q - 2, q) * (h + x * r)) % q;

            return (h, r, s);     
        }
    }
}
