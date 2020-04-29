using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Velocidad;
    public float FuerzaSalto;

    private Animator animator;
    private const int MAX_SALTOS = 2;
    private int Saltos;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider;

    void Start()
    {
        Saltos = MAX_SALTOS;
        rigidbody2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Mover();
    }

    private void Mover()
    {
        Vector2 movimiento = Vector2.zero;
        movimiento.x = Input.GetAxis("Horizontal");

        if (EnSuelo())
        {
            Saltos = MAX_SALTOS;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Saltos > 0)
        {
            Saltos--;
            movimiento.y = FuerzaSalto;
        }

        rigidbody2d.AddForce(movimiento, ForceMode2D.Impulse);
        rigidbody2d.velocity = new Vector2(Mathf.Clamp(rigidbody2d.velocity.x, -Velocidad, Velocidad), rigidbody2d.velocity.y);
    }

    private bool EnSuelo()
    {
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        return Physics2D.OverlapArea(bounds.min, bounds.max, LayerMask.GetMask("Suelo"));
    }
}
