public abstract class Device
{
    public string Id { get; set; } 
    public string Name { get; set; }
    public bool IsTurnedOn { get; protected set; }

    protected Device(string id, string name, bool isTurnedOn)
    {
        Id = id;
        Name = name;
        IsTurnedOn = isTurnedOn;
    }

    public virtual void TurnOn() => IsTurnedOn = true;
    public virtual void TurnOff() => IsTurnedOn = false;
    public abstract string GetSaveFormat();
}