using UnityEngine;

public class SortingOrder : MonoBehaviour
{
    private SpriteRenderer sr;
    public int sortingOffset = 0; 

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100) + sortingOffset;
    }
}
