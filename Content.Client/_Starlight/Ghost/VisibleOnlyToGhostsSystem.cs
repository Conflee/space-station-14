using Content.Shared._Starlight.Ghost;
using Content.Shared.Ghost;
using Robust.Client.GameObjects;
using Robust.Client.Player;

namespace Content.Client._Starlight.Ghost;
public sealed class VisibleOnlyToGhostsSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Update(float frameTime)
    {
        var local = _player.LocalEntity;

        var visibleQuery = EntityQueryEnumerator<VisibleToGhostsOnlyComponent, SpriteComponent>();

        bool isGhost = HasComp<GhostComponent>(local);

        while (visibleQuery.MoveNext(out var uid, out var visible, out var sprite))
        {
            if (!_sprite.LayerMapTryGet((uid, sprite), GhostVisibleOnlyVisualLayers.Base, out var layer, true))
                continue;

            _sprite.LayerSetVisible((uid, sprite), layer, isGhost);
        }
    }
}
//credit for code to Vermidia @ https://github.com/RMC-14/RMC-14/pull/8488