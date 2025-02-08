using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Axe : MonoBehaviour
{
    [SerializeField] private GameObject _axe;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _axeSwing;
    [SerializeField] private GameObject _axeStationary;
    [Header("Hitbox visualizer")]
    [SerializeField] private bool _visualizerEnabled = true;
    [SerializeField] private float _axeVisualizerTime = 0.2f;
    private LineRenderer _hitboxLineRenderer;
    [Header("Axe config")]
    [SerializeField] private float _axeDamage = 10f; // Potentially changed in future for multiple axes
    /// <summary>
    /// Cooldown between axe swings
    /// </summary>
    [SerializeField] private float _axeCooldown = 0.5f; 

    private SpriteRenderer _axeSprite;
    private BoxCollider2D _axeCollider;
    private PlayerInput _input;

    private bool _isCooldown = false;

    private void Start()
    {
        InitAxeHitboxVisualizer();

        _axeSprite = _axe.GetComponent<SpriteRenderer>();
        _axeCollider = _axe.GetComponent<BoxCollider2D>();

        _axeStationary.SetActive(true);
        _axeSwing.SetActive(false);

        Debug.Log(GameManager.Instance.playerInput.Player.Fire.enabled);
        GameManager.Instance.playerInput.Player.Fire.started += OnAxeSwing;
    }

    /// <summary>
    /// Executes axe swing (starting point)
    /// </summary>
    private void OnAxeSwing(InputAction.CallbackContext context)
    {
        if (_isCooldown) return;
        StartCoroutine(SwingCooldown(_axeCooldown));
        StartCoroutine(AxeSwingAnim());
        Collider2D[] hitList = AxeDetectHit();
        foreach (Collider2D hit in hitList)
        {
            // Check if hit is destroyable, if so take damage
            Destroyable destroyableComp = hit.gameObject.GetComponent<Destroyable>();
            if (destroyableComp != null)
            {
                destroyableComp.TakeDamage(_axeDamage);
            }
        }
    }

    /// <summary>
    /// Runs collision detection for axe on 'Destoryable' layer. Returns all hit gameobjects
    /// </summary>
    /// <returns></returns>
    private Collider2D[] AxeDetectHit()
    {
        Vector2 cornerA = _axe.transform.position - new Vector3(
                (_axe.transform.lossyScale.x / 2) * Mathf.Cos(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                (_axe.transform.lossyScale.x / 2) * Mathf.Sin(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                0
            )
            - new Vector3(
                -(_axe.transform.lossyScale.y / 2) * Mathf.Sin(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                (_axe.transform.lossyScale.y / 2) * Mathf.Cos(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                0
            );
        Vector2 cornerB = _axe.transform.position + new Vector3(
                (_axe.transform.lossyScale.x / 2) * Mathf.Cos(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                (_axe.transform.lossyScale.x / 2) * Mathf.Sin(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                0
            )
            + new Vector3(
                -(_axe.transform.lossyScale.y / 2) * Mathf.Sin(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                (_axe.transform.lossyScale.y / 2) * Mathf.Cos(_axe.transform.eulerAngles.z * Mathf.Deg2Rad),
                0
            );
        Collider2D[] hitList = Physics2D.OverlapAreaAll(cornerA, cornerB, layerMask: 1 << 6);

        if (_visualizerEnabled) StartCoroutine(VisualizeAxeHitbox(cornerA, cornerB));

        return hitList;
    }

    /// <summary>
    /// Prevents axe swing for given amount of seconds
    /// </summary>
    private IEnumerator SwingCooldown(float seconds)
    {
        _isCooldown = true;
        yield return new WaitForSeconds(seconds);
        _isCooldown = false;
    }

    private IEnumerator VisualizeAxeHitbox(Vector3 cornerA, Vector3 cornerB)
    {
        _axeSprite.enabled = true; // temp
        _hitboxLineRenderer.enabled = true;
        _hitboxLineRenderer.SetPositions(new Vector3[] { cornerA, cornerB });
        yield return new WaitForSeconds(_axeVisualizerTime);
        _axeSprite.enabled = false; // temp
        _hitboxLineRenderer.enabled = false;
    }

    private void InitAxeHitboxVisualizer()
    {
        _hitboxLineRenderer = gameObject.AddComponent<LineRenderer>();
        _hitboxLineRenderer.enabled = false;
        _hitboxLineRenderer.startWidth = 0.1f;
        _hitboxLineRenderer.endWidth = 0.1f;
        _hitboxLineRenderer.positionCount = 2;
    }


    private IEnumerator AxeSwingAnim()
    {
        _axeStationary.SetActive(false);
        _axeSwing.SetActive(true);

        float animDuration = 0.15f;

        Vector3 swingStartPos = new Vector3(0.25f, 0.3f, 0f);
        Vector3 swingEndPos = new Vector3(-0.25f, 0.3f, 0f);

        Quaternion swingStartRot = Quaternion.Euler(new Vector3(0, 0, 0));
        Quaternion swingEndRot = Quaternion.Euler(new Vector3(0, 0, 90));

        _axeSwing.transform.localPosition = swingStartPos;
        _axeSwing.transform.localRotation = swingStartRot;


        Sequence sequence = DOTween.Sequence();
        sequence.Append(_axeSwing.transform.DOLocalRotateQuaternion(swingEndRot, animDuration));
        sequence.Join(_axeSwing.transform.DOLocalMove(swingEndPos, animDuration));

        sequence.OnComplete(() =>
        {
            _axeStationary.SetActive(true);
            _axeSwing.SetActive(false);
        });
        sequence.Play();
        yield return null;
    }
}
