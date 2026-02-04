using UnityEngine;

public class PieceSnapper : MonoBehaviour
{
    [System.Serializable]
    public class SnapTarget
    {
        public Transform target;
    }

    [Header("Piece Type")]
    public bool isSquare = false;

    [Header("Allowed Snap Targets")]
    public SnapTarget[] allowedTargets;

    [Header("Snap Settings")]
    public float snapDistance = 1.0f;
    public float snapAngle = 10f;

    [Header("Audio")]
    public AudioClip wowClip;
    public AudioClip rejectClip;

    [Tooltip("Distance within which rejection sound plays")]
    public float rejectDistanceThreshold = 0.4f;

    public AudioSource audioSource;

    [HideInInspector]
    public bool isSnapped = false;

    //Attempt snapping pieces to template blocks
    public bool TrySnap()
    {
        if (isSnapped)
            return false;

        float closestDist = float.MaxValue;

        foreach (SnapTarget snap in allowedTargets)
        {
            Transform target = snap.target;
            if (target == null)
                continue;

            SnapSlot slot = target.GetComponent<SnapSlot>();
            if (slot != null && slot.isOccupied)
                continue;

            // Planar distance
            Vector3 a = transform.position;
            Vector3 b = target.position;
            a.y = b.y;

            float dist = Vector3.Distance(a, b);

            if (dist < closestDist)
                closestDist = dist;

            if (dist > snapDistance)
                continue;

            if (!RotationMatch(target))
                continue;

            //Snap success
            transform.position = target.position;
            transform.rotation = target.rotation;

            isSnapped = true;

            if (slot != null)
                slot.isOccupied = true;

            Renderer r = target.GetComponent<Renderer>();
            if (r != null)
                r.enabled = false;

            PlayWow();
            return true;
        }

        //Snap failed
        if (closestDist <= rejectDistanceThreshold)
        {
            PlayReject();
        }

        return false;
    }

    //Match rotation of blocks and pieces for snapping
    bool RotationMatch(Transform target)
    {
        Quaternion relative =
            Quaternion.Inverse(target.rotation) *
            transform.rotation;

        float angle =
            Quaternion.Angle(Quaternion.identity, relative);

        if (isSquare)
        {
            angle = angle % 90f;
            angle = Mathf.Min(angle, 90f - angle);
        }

        return angle <= snapAngle;
    }

    //Audio for correct and incorrect snap
    void PlayWow()
    {
        if (audioSource && wowClip)
            audioSource.PlayOneShot(wowClip);
    }

    void PlayReject()
    {
        if (audioSource && rejectClip)
            audioSource.PlayOneShot(rejectClip);
    }
}
