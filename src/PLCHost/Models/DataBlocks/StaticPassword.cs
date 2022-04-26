namespace PLCHost.Models.DataBlocks;

public class StaticPassword
{
    public ushort StaticPassword1 { get; set; }
    public ushort StaticPassword2 { get; set; }
    public ushort StaticPassword3 { get; set; }
    public ushort StaticPassword4 { get; set; }
    public ushort StaticPassword5 { get; set; }
    public ushort StaticPassword6 { get; set; }
    public ushort StaticPasswordAnalogHigh { get; set; }
    public ushort StaticPasswordAnalogLow { get; set; }

    public bool AreEqual(StaticPassword that)
    {
        return StaticPassword1 == that.StaticPassword1 &&
               StaticPassword2 == that.StaticPassword2 &&
               StaticPassword3 == that.StaticPassword3 &&
               StaticPassword4 == that.StaticPassword4 &&
               StaticPassword5 == that.StaticPassword5 &&
               StaticPassword6 == that.StaticPassword6 &&
               StaticPasswordAnalogHigh == that.StaticPasswordAnalogHigh &&
               StaticPasswordAnalogLow == that.StaticPasswordAnalogLow;
    }
}
