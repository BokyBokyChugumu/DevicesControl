public class PersonalComputer : Device
{
    public string? OperatingSystem { get; set; }

    public PersonalComputer(string id, string name, bool isTurnedOn, string? os = null) // Changed Id type to string
        : base(id, name, isTurnedOn)
    {
        OperatingSystem = os;
    }

    public override void TurnOn()
    {
        if (OperatingSystem == null)
            throw new EmptySystemException("No OS installed. Cannot turn on.");
        base.TurnOn();
    }

    public override string GetSaveFormat() => $"{Id},{Name},{IsTurnedOn},{OperatingSystem ?? "null"}"; // Removed type prefix from ID in save format
}