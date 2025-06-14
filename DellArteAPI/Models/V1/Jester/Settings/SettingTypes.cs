namespace DellArteAPI.Models.V1.Jester.Settings;

public enum SettingTypes
{
    /// <summary>
    /// Disables exp receiving
    /// </summary>
    ExpDisabling = 1,

    /// <summary>
    /// Auto takes new quests in the start of the day
    /// </summary>
    AutoQuestsTake = 2,

    /// <summary>
    /// Automatically extends boosts after they expire (if available)
    /// </summary>
    AutoBoostsExtend = 3,

    /// <summary>
    /// Prevents a user from being selected as an `/interact` command target
    /// </summary>
    RestrictInteractions = 4,

    /// <summary>
    /// Prevents the user and their duo from using the `/duet dispose` command
    /// </summary>
    RestrictDuetDispose = 5
}