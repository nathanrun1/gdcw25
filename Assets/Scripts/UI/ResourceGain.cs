using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceGain : MonoBehaviour
{
    [SerializeField] private Sprite _woodIcon;
    [SerializeField] private Sprite _metalIcon;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _count;

    [Header("Anim config")]
    [SerializeField] private Vector3 _animStartPos = new Vector3(0, 90, 0);
    [SerializeField] private Vector3 _animEndPos = new Vector3(0, 120, 0);
    [SerializeField] private float _animLength = 0.2f;

    private Dictionary<ResourceType, Sprite> rscToSprite;

    private void Awake()
    {
        rscToSprite = new Dictionary<ResourceType, Sprite>
        {
            {ResourceType.Wood, _woodIcon },
            {ResourceType.Metal, _metalIcon }
        };
        _icon.gameObject.SetActive(false);
        _count.gameObject.SetActive(false);
    }

    public void GainResourceAnim(ResourceType rsc, int amnt)
    {
        if (amnt > 0)
        {
            // Positive
            _icon.gameObject.SetActive(true);
            _count.gameObject.SetActive(true);
            Debug.Log("amnt positive");
            _icon.sprite = rscToSprite[rsc];
            _count.text = $"+{amnt}";
            Sequence seq = DOTween.Sequence();
            transform.localPosition = _animStartPos;
            seq.Append(transform.DOLocalMove(_animEndPos, _animLength));
            Color initialIconColor = _icon.color;
            Color initialCountColor = _count.color;
            //seq.Join(_icon.DOColor(new Color(initialIconColor.r, initialIconColor.g, initialIconColor.b, 0), ANIM_LENGTH));
            //seq.Join(_icon.DOColor(new Color(initialCountColor.r, initialCountColor.g, initialCountColor.b, 0), ANIM_LENGTH));
            seq.OnComplete(() =>
            {
                Debug.Log("completed");
                // Set inactive again
                _icon.gameObject.SetActive(false);
                _count.gameObject.SetActive(false);

                // Reset values
                _icon.color = initialIconColor;
                _count.color = initialCountColor;
                transform.localPosition = _animStartPos;
            });
            seq.Play();
        }
        // add func for negative?
    }
}
