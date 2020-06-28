
namespace ScanMatchers.Tools.Structures
{
    public interface ILaserRangeData
    {
        float MaxRange { get; }
        float MinRange { get; }
        float OffsetX { get; }
        float OffsetY { get; }

        double FieldOfView { get; }
        double[] Range { get; }
        double[] RangeTheta { get; }
        bool[] RangeFilters { get; }
        double Resolution { get; }
    }
}
