using UvARescue.Agent;
using UvARescue.Math;
using UvARescue.Tools;

namespace UvARescue.Slam
{

    public interface IScanMatcher
    {

        void ApplyConfig(Config config);

        MatchResult Match(Manifold manifold, Patch patch1, Patch patch2, Pose2D seed);

    }

}

