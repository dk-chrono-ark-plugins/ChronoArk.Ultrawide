using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ultrawide.Helper;

#nullable enable

internal static class ChildrenIterator
{
    internal static IEnumerable<Transform> GetAllChildren(this Transform parent, bool active = false)
    {
        for (var i = 0; i < parent.childCount; ++i) {
            var child = parent.GetChild(i);
            if (active && !child.gameObject.activeInHierarchy) {
                continue;
            }
            yield return child;
        }
    }

    internal static Transform? GetFirstChildWithName(this Transform parent, string name)
    {
        var children = parent
            .GetAllChildren()
            .Where(t => t.name == name);
        return children.Any() ? children.First() : null;
    }
}
