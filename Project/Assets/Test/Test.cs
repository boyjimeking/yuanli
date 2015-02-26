using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    private void OnDrawGizmos()
    {
        for (int i = 0; i < 80; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
                var pos = new Vector3(i, 0, j);
                Gizmos.DrawWireCube(pos, new Vector3(1.0f, 0, 1.0f));
            }
        }
    }
}
