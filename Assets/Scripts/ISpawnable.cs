
// Interface for all spawnable objects in the game.
// Implements the Strategy pattern, allowing different spawning behaviors
// to be interchangeable while maintaining a consistent interface.
//
// Separates the initialization of spawn parameters from
// the actual spawn execution.

public interface ISpawnable
{
    void InitializeFromWave(WaveSettings settings);
    void Spawn();
}
