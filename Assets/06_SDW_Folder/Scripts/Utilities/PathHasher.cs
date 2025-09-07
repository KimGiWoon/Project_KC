using System;
using System.Text;

namespace SDW
{
    public static class PathHasher
    {
        // 간단하고 빠른 xxHash64 대체: FNV-1a 64bit
        /// <summary>
        /// FNV-1A 64bit 알고리즘을 적용하여 파일의 경로를 hash 값으로 생성
        /// </summary>
        /// <param name="path">hash 값으로 생성하기 위핸 경로 이름</param>
        /// <returns>Path로 생성한 hash 값을 반환</returns>
        public static ulong Hash(string path)
        {
            const ulong fnvOffset = 14695981039346656037UL;
            const ulong fnvPrime = 1099511628211UL;

            ulong hash = fnvOffset;
            var span = path.AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                hash ^= span[i];
                hash *= fnvPrime;
            }
            return hash;
        }
    }
}