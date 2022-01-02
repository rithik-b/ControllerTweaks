using SiraUtil.Affinity;

namespace ControllerTweaks.AffinityPatches
{
    public class SaberTypePatch : IAffinity
    {
        [AffinityPrefix]
        [AffinityPatch(typeof(SaberTypeExtensions), nameof(SaberTypeExtensions.Node))]
        private void Patch(ref SaberType saberType)
        {
            if (saberType == SaberType.SaberA)
            {
                saberType = SaberType.SaberB;
            }
            else
            {
                saberType = SaberType.SaberA;
            }
        }
    }
}
