using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script à attacher aux objets qui doivent "attirer" l'oeil de la camera
/// </summary>
public class Landmark : MonoBehaviour
{
    const int maxRank = 4;
    public int MaxRank { get { return maxRank; } }
    /// <summary>
    /// A quel point le point d'intérêt va attirer l'oeil de la caméra
    /// </summary>
    [SerializeField] [Range(1, maxRank)] private int rank;
    public int Rank {  get { return rank != 0 ? rank : 1; } }
    /// <summary>
    /// A quelle distance le point d'intérêt va attirer l'oeil de la caméra
    /// </summary>
    [SerializeField] [Range(0.5f, 500.0f)] private float size = 10.0f;
    public float Radius { get { return size; } }
    /// <summary>
    /// Renderer de l'objet qui attire la caméra
    /// </summary>
    [SerializeField] private Renderer landmark;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, size);
    }

    public bool IsVisible()
    {
        if (landmark == null)
        {
            Debug.LogWarning("Landmark has no renderer");
            return false;
        }

        return landmark.isVisible;
    }

    public bool IsNearby(Vector3 _playerPos)
    {
        return Vector3.Distance(transform.position, _playerPos) < size;           
    }


}
