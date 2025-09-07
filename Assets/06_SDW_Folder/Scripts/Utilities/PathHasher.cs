using System;
using System.Text;

namespace SDW
{
    public static class PathHasher
    {
        // 간단하고 빠른 xxHash64 대체: FNV-1a 64bit
        public static ulong Hash(string s)
        {
            const ulong fnvOffset = 14695981039346656037UL;
            const ulong fnvPrime = 1099511628211UL;

            ulong hash = fnvOffset;
            var span = s.AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                hash ^= span[i];
                hash *= fnvPrime;
            }
            return hash;
        }
    }
}