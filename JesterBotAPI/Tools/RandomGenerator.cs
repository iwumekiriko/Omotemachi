namespace DellArteAPI.Tools;

public class RandomGenerator(uint seed)
{
    private uint _state = seed;

    public int NextInt(int min, int max)
    {
        _state ^= _state << 13;
        _state ^= _state >> 17;
        _state ^= _state << 5;
        return min + (int)(_state % (max - min + 1));
    }
}