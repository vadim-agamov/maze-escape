using UnityEngine;

public class FieldBorder : MonoBehaviour
{
    [SerializeField]
    private PhysicsMaterial2D _physicsMaterial;
    
    private void Start()
    {
        var bounds = GetBounds();

        var edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider2D.points = bounds;
        edgeCollider2D.sharedMaterial = _physicsMaterial;
    }

    private Vector2[] GetBounds()
    {
        var corners = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetLocalCorners(corners);

        return new Vector2[]
        {
            corners[0],
            corners[1],
            corners[2],
            corners[3],
            corners[0]
        };
    }

    private void OnDrawGizmos()
    {
        var corners = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);
    }
}
