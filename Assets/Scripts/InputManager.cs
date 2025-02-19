using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeReference]
    SpriteRenderer spriteRenderer;

    Transform spriteTransform;

    void Start()
    {
        spriteTransform = spriteRenderer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            return;
        }

        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 localMousePos = spriteTransform.InverseTransformPoint(worldMousePos);

        Vector2 spriteSize = spriteRenderer.size;

        float relativeX = (localMousePos.x + spriteSize.x * 0.5f) / spriteSize.x;
        float relativeY = 1f - (localMousePos.y + spriteSize.y * 0.5f) / spriteSize.y;


        Debug.Log($"Relative Poaition: X: {relativeX}, Y: {relativeY}");
    }
}
