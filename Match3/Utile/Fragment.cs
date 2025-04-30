using DG.Tweening;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    [SerializeField] private EObjectType type;

    public float xForce;    // x축 힘 (더 멀리)
    public float yForce;    // Y축 힘 (더 높이)
    public float gravity;   // 중력 (떨어지는 속도 제어)
    

    private Vector2 direction;
    private bool isGrounded = true;

    private float maxHeight;    
    private float currentheight;

    public Transform cartridge;
    public GameObject childImage;

    private Color _originColor;

    [SerializeField]
    private AudioSource _audioSource;

    private void Awake()
    {
        _originColor = cartridge.GetComponent<SpriteRenderer>().color;
    }

    void OnEnable()
    {
        if (_audioSource != null)
            _audioSource?.Stop();

        InitVariable();
        currentheight = Random.Range(yForce - 1, yForce);
        maxHeight = currentheight;
        childImage.gameObject.transform.position = Vector3.zero;
        cartridge.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-50, 50), ForceMode2D.Impulse);
        Initialize(new Vector2(Random.Range(-xForce, xForce), Random.Range(-xForce, xForce)));
    }

    private void InitVariable()
    {
        isGrounded = false;
        cartridge.GetComponent<SpriteRenderer>().color = _originColor;
    }

    void Update()
    {
        if (!isGrounded)
        {
            currentheight += -gravity * Time.deltaTime;
            cartridge.position += new Vector3(0, currentheight, 0) * Time.deltaTime;
            transform.position += (Vector3)direction * Time.deltaTime;

            if (cartridge.position.y <= transform.position.y)
            {
                isGrounded = true;
                if (_audioSource != null)
                    _audioSource?.Play();
                cartridge.GetComponent<Rigidbody2D>().angularVelocity = 0;
                cartridge.GetComponent<SpriteRenderer>().DOColor(new Color(0, 0, 0, 0), 30f).SetEase(Ease.InExpo).OnComplete(() => ReleaseToPool());
            }
        }
    }

    void Initialize(Vector2 _direction)
    {
        isGrounded = false;
        maxHeight /= 1.5f;
        direction = _direction;
        currentheight = maxHeight;
    }

    void ReleaseToPool()
    {
        PoolManager.Instance.ReturnObject(gameObject, type);
    }

    public EObjectType GetObjectType()
    {
        return type;
    }

    public void SetImage(string type)
    {
        var ingredientImage = Resources.Load<Sprite>($"Sprites/{type}");
        if (ingredientImage != null)
        {
            var spriteRenderer = childImage.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = ingredientImage;
            }
        }
    }
}