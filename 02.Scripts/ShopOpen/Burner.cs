using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Burner : MonoBehaviour
{
    [SerializeField]
    private Slider _fireSlider;
    [SerializeField]
    private Image _fireImage;
    [SerializeField]
    private ParticleSystem _smokeParticle;
    [SerializeField]
    private Image _bowlCover;
    [SerializeField]
    private Bowl _bowl;

    [SerializeField]
    private float _cookingTime;

    private bool _isFireOn = false;

    public float FireTimer;

    private void Start()
    {
        _fireSlider.maxValue = _cookingTime;
        _fireSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(_isFireOn)
        {
            FireTimer += Time.deltaTime;
            _fireSlider.gameObject.SetActive(true);
            _fireSlider.value = FireTimer;

            if(FireTimer >= _cookingTime)
            {
                _bowl.IsCooked = true;
                BowlMeatChecker.Instance.ReplacePutMeatSprites();//고기 익은 걸로 바꾸기

                _fireSlider.gameObject.SetActive(false);
                OnToggleBurner();
                _smokeParticle.Play();
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.completeSound); 
                FireTimer = 0;
            }
        }

        else
        {
            _fireSlider.value = 0;//0418
            FireTimer = 0;//0418
           // _bowl.IsCooked = false;
        }

        //Debug.Log("냄비 뚜껑 위치" + _bowlCover.transform.position.x);
    }

    public void InitBurner()
    {
        _isFireOn = false;
        _fireImage.gameObject.SetActive(false);
        //FireTimer = 0;

        _fireSlider.value = 0;//0418
        FireTimer = 0;//0418
        _bowl.IsCooked = false;
    }

    public void OnToggleBurner()
    { 

        if(_isFireOn)
        {
            _isFireOn = false;
            _fireImage.gameObject.SetActive(false);
            _bowlCover.gameObject.SetActive(true);
            _bowlCover.transform.DOMoveX(-10f, 1f);//-10 //1f    //-0.08364047이거보다 커야 함
            SoundManager.Instance.StopBoillingSound();
            //SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.FireOnSound);
        }
        else
        {
            _isFireOn = true;
            _fireImage.gameObject.SetActive(true);
            _bowlCover.gameObject.SetActive(true);
            _bowlCover.transform.DOMoveX(-0.02f, 1f);//DOMoveX(0, 1f);
            SoundManager.Instance.PlayBoillingSound();
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.FireOnSound);
        }
    }
}
