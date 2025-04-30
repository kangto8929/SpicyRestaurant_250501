using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 1.5f;
    [SerializeField] private Ease _fadeEaseType = Ease.InOutQuad;

    private void Start()
    {
        // 시작 시 검은색 이미지로 화면을 완전히 가립니다
        if (_fadeImage != null)
        {
            // 이미지가 검은색이고 완전히 불투명한지 확인
            _fadeImage.color = new Color(0, 0, 0, 1);

            // 페이드 인 애니메이션 시작 (알파값을 0으로 변경)
            _fadeImage.DOFade(0f, _fadeDuration)
                .SetEase(_fadeEaseType)
                .OnComplete(() =>
                {
                    _fadeImage.raycastTarget = false;
                    _fadeImage.maskable = false;
                });
        }
    }
}