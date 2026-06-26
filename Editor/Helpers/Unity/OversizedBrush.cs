#if DEVELOPMENT_ESSENTIALS_EDITOR_TILEMAPS
using UnityEngine;
using UnityEditor.Tilemaps;

namespace DevelopmentEssentials.Editor.Helpers.Unity {

    [CreateAssetMenu]
    [CustomGridBrush(false, false, false, "Oversized Brush")]
    public class OversizedBrush : GridBrush {

        public Vector2Int dimensions = Vector2Int.one;

        private void OnValidate() => dimensions = Vector2Int.Max(Vector2Int.one, dimensions);

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position) {
            Vector3Int snappedPos = new(
                position.x - position.x % dimensions.x,
                position.y - position.y % dimensions.y,
                position.z
            );

            base.Paint(grid, brushTarget, snappedPos);
        }

    }

}
#endif