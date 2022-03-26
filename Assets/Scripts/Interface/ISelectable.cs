using UnityEngine;

public interface ISelectable
{
    Transform transform { get; }

    void LeftClick();
    void RightClick();
    void RightRelease();
    void LeftRelease();
    void Dragging();
}
