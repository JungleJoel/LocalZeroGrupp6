namespace backend.Settings;

// This class acts as our singleton. It only allows one instance of itself.
// It is used to define some system wide settings like maximum eco points an initiative can have per participant.
// It's used in InitiativeService and EcoPointService.

public class EcoPointsConfig
{
    private static EcoPointsConfig? _instance;

    private EcoPointsConfig() { }

    public static EcoPointsConfig Instance()
    {
        if (_instance == null)
        {
            _instance = new EcoPointsConfig();
        }

        return _instance;
    }

    public int MaxEcoPointsPerParticipant { get; } = 500;
    public int MaxEcoPointsPerAction { get; } = 100;
}
