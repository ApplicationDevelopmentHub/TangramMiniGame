using UnityEngine;

[RequireComponent(typeof(PieceSnapper))]
public class PieceInteraction : MonoBehaviour
{
    private static PieceInteraction current;

    [Header("Preview Anchor")]
    public Transform previewAnchor;

    [Header("Board Surface")]
    public Transform boardSurface;

    [Header("Rotation")]
    public float rotationStep = 45f;

    [Header("Selection Animation")]
    public float selectedScale = 1.08f;
    public float scaleSpeed = 10f;

    private Camera cam;
    private bool isDragging;
    private Vector3 dragOffset;
    private Plane dragPlane;
    private float fixedY;

    private Vector3 baseScale;
    private Vector3 targetScale;

    private Vector3 clusterPos;
    private Quaternion clusterRot;

    private PieceSnapper snapper;

    void Start()
    {
        cam = Camera.main;
        snapper = GetComponent<PieceSnapper>();

        clusterPos = transform.position;
        clusterRot = transform.rotation;

        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        HandleSelection();
        HandleDragging();
        HandleRotation();
        AnimateScale();
    }

    //Selecting a piece
    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PieceInteraction piece =
                    hit.collider.GetComponent<PieceInteraction>();

                if (piece != null &&
                    !piece.snapper.isSnapped)   // ← GUARD
                {
                    Select(piece);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && current == this)
            EndDrag();
    }

    void Select(PieceInteraction piece)
    {
        if (current != null && current != piece)
            current.ReturnToCluster();

        current = piece;
        piece.MoveToPreview();
    }

    //Previewing of pieces
    void MoveToPreview()
    {
        if (snapper.isSnapped)
            return;

        transform.position = previewAnchor.position;
        targetScale = baseScale * selectedScale;
    }

    void ReturnToCluster()
    {
        if (snapper.isSnapped)
            return;

        transform.position = clusterPos;
        transform.rotation = clusterRot;
        targetScale = baseScale;
    }

    // ==============================
    // Drag
    // ==============================
    void HandleDragging()
    {
        if (current != this || snapper.isSnapped)
            return;

        if (Input.GetMouseButton(0))
        {
            if (!isDragging)
                BeginDrag();

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hit = ray.GetPoint(enter);
                Vector3 pos = hit + dragOffset;
                pos.y = fixedY;

                transform.position = pos;
            }
        }
    }

    void BeginDrag()
    {
        isDragging = true;

        fixedY = transform.position.y;
        dragPlane = new Plane(Vector3.up, boardSurface.position);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        dragPlane.Raycast(ray, out float enter);
        Vector3 hit = ray.GetPoint(enter);

        dragOffset = transform.position - hit;
    }

    void EndDrag()
    {
        isDragging = false;

        if (!snapper.TrySnap())
            MoveToPreview();
    }

    // ==============================
    // Rotation
    // ==============================
    void HandleRotation()
    {
        if (current != this || snapper.isSnapped)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            transform.Rotate(Vector3.up, -rotationStep, Space.World);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            transform.Rotate(Vector3.up, rotationStep, Space.World);
    }

    //Animation of selected pieces 
    void AnimateScale()
    {
        transform.localScale =
            Vector3.Lerp(transform.localScale,
                         targetScale,
                         Time.deltaTime * scaleSpeed);
    }
}
