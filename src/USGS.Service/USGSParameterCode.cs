namespace Tudormobile.USGS.Service;

/// <summary>
/// Represents a USGS parameter code used to identify measured phenomena.
/// Provides strongly-typed constants for supported parameters, with a
/// <see cref="Custom"/> factory for codes not explicitly defined here.
/// </summary>
public sealed class USGSParameterCode
{
    /// <summary>Represents a 'missing' code.</summary>

    public static readonly USGSParameterCode Missing = new("00000", "[No Parameter Code Provided]");

    /// <summary>Depth to water level, feet below land surface</summary>
    public static readonly USGSParameterCode WaterLevel = new("72019", "Depth to water level, feet below land surface");

    /// <summary>Discharge (streamflow), in cubic feet per second.</summary>
    public static readonly USGSParameterCode Discharge = new("00060", "Discharge, cubic feet per second");

    /// <summary>Gage height (stage), in feet.</summary>
    public static readonly USGSParameterCode GageHeight = new("00065", "Gage height, feet");

    /// <summary>Water temperature, in degrees Celsius.</summary>
    public static readonly USGSParameterCode WaterTemperature = new("00010", "Temperature, water, degrees Celsius");

    /// <summary>Dissolved oxygen, in milligrams per liter.</summary>
    public static readonly USGSParameterCode DissolvedOxygen = new("00300", "Dissolved oxygen, milligrams per liter");

    /// <summary>pH, in standard units.</summary>
    public static readonly USGSParameterCode pH = new("00400", "pH, standard units");

    /// <summary>Specific conductance, in microsiemens per centimeter at 25 degrees Celsius.</summary>
    public static readonly USGSParameterCode SpecificConductance = new("00095", "Specific conductance, microsiemens per centimeter");

    /// <summary>Turbidity, in Formazin Nephelometric Units (FNU).</summary>
    public static readonly USGSParameterCode Turbidity = new("63680", "Turbidity, water, unfiltered, FNU");

    /// <summary>The 5-digit USGS parameter code string.</summary>
    public string Code { get; }

    /// <summary>A human-readable description of the parameter.</summary>
    public string Description { get; }

    private USGSParameterCode(string code, string description)
    {
        Code = code;
        Description = description;
    }

    /// <summary>
    /// Creates a <see cref="USGSParameterCode"/> for a code not explicitly defined in this class.
    /// </summary>
    /// <param name="code">The 5-digit USGS parameter code.</param>
    /// <param name="description">An optional description. Defaults to an empty string.</param>
    /// <returns>A new <see cref="USGSParameterCode"/> instance.</returns>
    public static USGSParameterCode Custom(string code, string description = "")
        => new(code, description);

    /// <inheritdoc/>
    public override string ToString() => Code;
}
