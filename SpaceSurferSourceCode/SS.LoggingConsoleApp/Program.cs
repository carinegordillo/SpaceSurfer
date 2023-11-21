using SS.Logging;
using SS.Logging.DataAccess;
using SS.SharedNamespace;

internal class Program
{
    private static void Main(string[] args)
    {
        Credential SAUser = Credential.CreateSAUser();

        var dataAccess = new SqlDAO(SAUser);
        var logger = new Logger(dataAccess);
        var log = new LogEntry
        {
            level = "Debug",
            username = "test@email",
            category = "View",
            description = "Amidst the vibrant tapestry of life, where the relentless " +
                "march of time intertwines with the intricate threads of human experience, " +
                "a kaleidoscope of emotions, aspirations, and challenges unfolds, weaving a " +
                "narrative that spans the spectrum of joy, sorrow, triumph, and introspection, " +
                "resonating through the corridors of existence as individuals navigate the " +
                "labyrinth of their own stories, seeking meaning and connection in the complex " +
                "mosaic of relationships, dreams, and the ever-evolving tapestry of the cosmos, " +
                "where each moment is a brushstroke on the canvas of eternity, a fleeting expression " +
                "in the grand symphony of life that echoes across the vast expanse of the universe, " +
                "harmonizing with the cosmic dance of celestial bodies and the whispers of the unseen " +
                "forces that shape the destiny of worlds, creating an intricate ballet that transcends " +
                "the boundaries of time and space, inviting contemplation on the ephemeral nature of our " +
                "earthly sojourn and the profound interconnectedness that binds every sentient being to " +
                "the cosmic heartbeat, pulsating with the rhythm of creation, echoing the timeless refrain " +
                "of existence itself."
        };

        logger.SaveData(log);

        Console.WriteLine(log.getDescriptionLength);

        Console.WriteLine("Finished.");

    }

}