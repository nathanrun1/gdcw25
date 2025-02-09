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
    [SerializeField] private float _animYDelta = 30f;
    [SerializeField] private float _animLength = 0.2f;
    [SerializeField] private Color _positiveColorWood = new Color(99f / 255f, 73f / 255f, 66f / 255f, 1);
    [SerializeField] private Color _positiveColorMetal = new Color(110f / 255f, 116f / 255f, 121f / 255f);
    [SerializeField] private Color _negativeColor = new Color(217f / 255f, 5f / 255f, 5f / 255f, 1);

    private static Dictionary<ResourceType, Sprite> rscToSprite;

    private void Awake()
    {
        if (rscToSprite == null)
        {
            rscToSprite = new Dictionary<ResourceType, Sprite>
            {
                {ResourceType.Wood, _woodIcon },
                {ResourceType.Metal, _metalIcon }
            };
        }
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

            _icon.sprite = rscToSprite[rsc];
            _count.text = $"+{amnt}";
            _count.color = rsc == ResourceType.Wood ? _positiveColorWood : _positiveColorMetal;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMoveY(transform.localPosition.y + _animYDelta, _animLength));
            Color initialIconColor = _icon.color;
            Color initialCountColor = _count.color;
            //seq.Join(_icon.DOColor(new Color(initialIconColor.r, initialIconColor.g, initialIconColor.b, 0), ANIM_LENGTH));
            //seq.Join(_icon.DOColor(new Color(initialCountColor.r, initialCountColor.g, initialCountColor.b, 0), ANIM_LENGTH));
            seq.OnComplete(() =>
            {
                Destroy(gameObject);
            });
            seq.Play();
        }
        else if (amnt < 0)
        {
            // Negative
            _icon.gameObject.SetActive(true);
            _count.gameObject.SetActive(true);

            _icon.sprite = rscToSprite[rsc];
            _count.text = $"-{-amnt}";
            _count.color = _negativeColor;
            Sequence seq = DOTween.Sequence();
            transform.localPosition += new Vector3(0, _animYDelta, 0);
            seq.Append(transform.DOLocalMoveY(transform.localPosition.y - _animYDelta, _animLength));
            Color initialIconColor = _icon.color;
            Color initialCountColor = _count.color;
            //seq.Join(_icon.DOColor(new Color(initialIconColor.r, initialIconColor.g, initialIconColor.b, 0), ANIM_LENGTH));
            //seq.Join(_icon.DOColor(new Color(initialCountColor.r, initialCountColor.g, initialCountColor.b, 0), ANIM_LENGTH));
            seq.OnComplete(() =>
            {
                Destroy(gameObject);
            });
            seq.Play();
        }
        // add func for negative?
    }
}
