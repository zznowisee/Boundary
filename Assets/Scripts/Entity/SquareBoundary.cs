using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBoundary : MonoBehaviour
{

    private BoundaryType boundaryType;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material dottedMat;
    [SerializeField] private Material solidMat;
    [HideInInspector] public Direction direction;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Direction direction_, Color color_)
    {
        direction = direction_;
        spriteRenderer.color = color_;
        boundaryType = BoundaryType.SOLID;
        SetBoundaryVisual();
    }

    private void SetBoundaryVisual()
    {
        switch (boundaryType)
        {
            case BoundaryType.DOTTED:
                spriteRenderer.material = dottedMat;
                break;
            case BoundaryType.SOLID:
                spriteRenderer.material = solidMat;
                break;
        }
    }

    private void UpdateBoundaryType()
    {

        SetBoundaryVisual();
    }
}
