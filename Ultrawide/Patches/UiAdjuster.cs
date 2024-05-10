using System.Collections.Generic;
using UnityEngine;

namespace Ultrawide.Patches;

internal static class UiAdjuster
{
    internal enum RectAdjustment
    {
        LeftDynamic,
        RightDynamic,
        LeftMost,
        RightMost,
        Disable,
        Expand,
    }
    internal record CanvasPatch(string InstanceName, RectAdjustment Adjustment);

    /// <summary>
    /// Record for keepsake
    /// </summary>
    /// <param name="Original">original *world* position</param>
    /// <param name="Adjusted">adjusted *world* position</param>
    internal record AdjustedPosition(Vector3 Original, Vector3 Adjusted);

    private static readonly Dictionary<int, AdjustedPosition> _adjusted = [];

    internal static void AdjustForUltrawide(this Transform transform, Camera camera, RectAdjustment adjustment)
    {
        var rect = (RectTransform)transform;
        if (adjustment == RectAdjustment.Disable) {
            transform.gameObject.SetActive(false);
            return;
        }
        if (adjustment == RectAdjustment.Expand) {
            rect.sizeDelta = rect.sizeDelta with { x = Display.main.renderingWidth };
            return;
        }

        if (_adjusted.ContainsKey(transform.GetInstanceID())) {
            transform.position = ResolutionOption.IsUsingExtendedResolution
                ? _adjusted[transform.GetInstanceID()].Adjusted
                : _adjusted[transform.GetInstanceID()].Original;
        } else {
            switch (adjustment) {
                case RectAdjustment.LeftDynamic:
                case RectAdjustment.RightDynamic: {
                    var original = camera.WorldToScreenPoint(rect.position);
                    var offset = adjustment switch {
                        RectAdjustment.LeftDynamic => -original.x / 3f,
                        RectAdjustment.RightDynamic => (Display.main.systemWidth - original.x - rect.rect.width) / 3f,
                        _ => 0f,
                    };
                    var adjusted = original with { x = original.x + offset };
                    adjusted = camera.ScreenToWorldPoint(adjusted);
                    _adjusted.TryAdd(transform.GetInstanceID(), new(rect.position, adjusted));
                    break;
                }
                case RectAdjustment.LeftMost:
                case RectAdjustment.RightMost: {

                    break;
                }
                default:
                    break;
            }
        }
    }
}