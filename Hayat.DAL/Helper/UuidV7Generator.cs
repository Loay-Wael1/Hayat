using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Security.Cryptography;

namespace Hayat.DAL.Data.Configurations
{
    public class UuidV7Generator : ValueGenerator<Guid>
    {
        public override bool GeneratesTemporaryValues => false;

        public override Guid Next(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            return CreateVersion7();
        }

        public static Guid CreateVersion7()
        {
            var unixTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var guidBytes = new byte[16];
            RandomNumberGenerator.Fill(guidBytes);

            // Set the first 48 bits (6 bytes) to the timestamp
            guidBytes[0] = (byte)(unixTimeMs >> 40);
            guidBytes[1] = (byte)(unixTimeMs >> 32);
            guidBytes[2] = (byte)(unixTimeMs >> 24);
            guidBytes[3] = (byte)(unixTimeMs >> 16);
            guidBytes[4] = (byte)(unixTimeMs >> 8);
            guidBytes[5] = (byte)unixTimeMs;

            // Set version 7 (4 bits)
            guidBytes[6] = (byte)(guidBytes[6] & 0x0F | 0x70);
            // Set variant (2 bits)
            guidBytes[8] = (byte)(guidBytes[8] & 0x3F | 0x80);

            // To map to standard UUID layout correctly since C# interprets Guids in mixed-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(guidBytes, 0, 4);
                Array.Reverse(guidBytes, 4, 2);
                Array.Reverse(guidBytes, 6, 2);
            }

            return new Guid(guidBytes);
        }
    }
}
