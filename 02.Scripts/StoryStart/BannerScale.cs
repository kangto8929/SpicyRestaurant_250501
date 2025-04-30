using DG.Tweening;
using UnityEngine;

public class BannerScale : MonoBehaviour
{
    [SerializeField] private RectTransform _bannerRectTransform;
    [SerializeField] private float _scaleUp = 1.1f;
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private Ease _ease = Ease.OutBack;

    private void Awake()
    {
        _bannerRectTransform.localScale = Vector3.zero;
    }

    public void AnimateBanner()
    {
        _bannerRectTransform.localScale = Vector3.one * 0.8f;
        _bannerRectTransform.DOScale(_scaleUp, _duration)
            .SetEase(_ease)
            .OnComplete(() =>
            {
                _bannerRectTransform.DOScale(1f, _duration).SetEase(_ease);
            });
    }
}
