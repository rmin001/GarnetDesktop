namespace GarnetDesktop.Core.Helpers;

public static class GarnetMemoryPolicy
{
    private const long MB = 1024L * 1024L;

    // Garnet requirement: page size baseline
    public const long DefaultPageSizeBytes = 32L * MB;
    public const long MinMemoryBytes = DefaultPageSizeBytes * 2; // 64MB

    public static long ToBytes(int memoryMb) => memoryMb * MB;

    public static long AlignToPowerOfTwo(long value)
    {
        if (value <= 1)
            return 1;

        long power = 1;

        while (power * 2 <= value)
            power *= 2;

        return power;
    }

    public static long Normalize(int memoryMb)
    {
        var bytes = ToBytes(memoryMb);

        bytes = AlignToPowerOfTwo(bytes);

        if (bytes < MinMemoryBytes)
            bytes = MinMemoryBytes;

        return bytes;
    }
}