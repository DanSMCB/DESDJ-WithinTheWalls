using Alteruna;
using UnityEngine;
using System.Collections.Generic;

public class GhostStairsManager : AttributesSync
{
    [Header("Roots")]
    public GameObject normalRoot;
    public GameObject ghostRoot;  

    [Header("Fade")]
    public float fadeSpeed = 2f;

    [SynchronizableField] public bool player0Looking;

    [SynchronizableField] public bool player1Looking;

    [SynchronizableField] public bool player0OnStairs;

    [SynchronizableField] public bool player1OnStairs;

    [SynchronizableField] public bool showGhostForPlayer0;

    [SynchronizableField] public bool showGhostForPlayer1;

    private Collider ghostCollider;
    private Renderer[] ghostRenderers;
    private Renderer[] normalRenderers;
    private float currentAlpha = 0f;

    private void Start()
    {
        ghostCollider = ghostRoot.GetComponent<Collider>();
        ghostRenderers = ghostRoot.GetComponentsInChildren<Renderer>(true);
        normalRenderers = normalRoot.GetComponentsInChildren<Renderer>(true);

        //foreach (var r in normalRenderers)
        //{
        //    Debug.Log($"Ghost Renderer: {r.gameObject.name}");
        //}

        normalRoot.SetActive(true);
        ghostRoot.SetActive(true);

        SetGhostAlpha(0f);
    }

    private void Update()
    {
        UpdateLocalLooking();
        UpdateVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        var avatar = other.GetComponentInParent<Alteruna.Avatar>();
        if (avatar == null) return;

        ushort userIndex = avatar.Owner.Index;

        if (userIndex == 0 && showGhostForPlayer0)
            player0OnStairs = true;
        else if (userIndex == 1 && showGhostForPlayer1)
            player1OnStairs = true;

        Commit();
    }

    private void OnTriggerExit(Collider other)
    {
        var avatar = other.GetComponentInParent<Alteruna.Avatar>();
        if (avatar == null) return;

        ushort userIndex = avatar.Owner.Index;

        if (userIndex == 0)
            player0OnStairs = false;
        else if (userIndex == 1)
            player1OnStairs = false;

        Commit();
    }

    void UpdateLocalLooking()
    {
        var avatar = Multiplayer.Instance.GetAvatar();
        var user = Multiplayer.Instance.GetUser();
        if (avatar == null || user == null) return;

        Camera cam = avatar.GetComponentInChildren<Camera>();
        if (cam == null) return;

        bool isLooking = false;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 6f))
        {
            if (hit.collider != null && hit.collider.transform == transform)
            {
                isLooking = true;
                Debug.Log($"Player {user.Index} is looking at the ghost stairs.");
            }
        }

        if (user.Index == 0 && player0Looking != isLooking)
        {
            player0Looking = isLooking;
            Commit();
        }
        else if (user.Index == 1 && player1Looking != isLooking)
        {
            player1Looking = isLooking;
            Commit();
        }

        if (Multiplayer.Instance.Me.IsHost)
            ResolveVisibility();
    }

    void ResolveVisibility()
    {
        if (player0OnStairs || player1OnStairs)
        {
            showGhostForPlayer0 = true;
            showGhostForPlayer1 = true;
        }
        else
        {
            if (player0Looking && player1Looking)
            {
                showGhostForPlayer0 = true;
                showGhostForPlayer1 = true;
            }
            else if (player0Looking)
            {
                showGhostForPlayer0 = false;
                showGhostForPlayer1 = true;
            }
            else if (player1Looking)
            {
                showGhostForPlayer0 = true;
                showGhostForPlayer1 = false;
            }
            else
            {
                showGhostForPlayer0 = false;
                showGhostForPlayer1 = false;
            }
        }

        Commit();
    }

    void UpdateVisuals()
    {
        var user = Multiplayer.Instance.GetUser();
        if (user == null) return;

        bool shouldSeeGhost = (showGhostForPlayer0 && showGhostForPlayer1) ? true :
            user.Index == 0 ? showGhostForPlayer0 :
            user.Index == 1 ? showGhostForPlayer1 :
            false;

        float target = shouldSeeGhost ? 1f : 0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, target, Time.deltaTime * fadeSpeed);
        SetGhostAlpha(currentAlpha);

        normalRoot.SetActive(currentAlpha < 0.01f);

        ghostCollider.enabled = shouldSeeGhost;
    }

    void SetGhostAlpha(float alpha)
    {
        foreach (var r in ghostRenderers)
        {
            foreach (var mat in r.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
            r.enabled = alpha > 0.01f;
        }
    }
}
