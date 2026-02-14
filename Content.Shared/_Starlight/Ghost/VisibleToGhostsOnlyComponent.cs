using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.Ghost;

[RegisterComponent, NetworkedComponent]
public sealed partial class VisibleToGhostsOnlyComponent : Component;

[Serializable, NetSerializable]
public enum GhostVisibleOnlyVisualLayers
{
    Base,
}
//credit for code to Vermidia @ https://github.com/RMC-14/RMC-14/pull/8488