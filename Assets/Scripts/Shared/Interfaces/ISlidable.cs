using System.Collections;
using UnityEngine;

namespace Portfolio.Shared
{
    public interface ISlidable
    {
        IEnumerator Slide(Vector3 startPoint, Vector3 endPoint, float duration);
    }
}
