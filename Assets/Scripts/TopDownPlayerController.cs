using System.Collections;
using UnityEngine;

public enum PlayerState
{
    IDLE, CaminarDown, CaminarUp, CaminarLeft, CaminarRight,
    AtacarDown, AtacarUp, AtacarLeft, AtacarRight,
    Correr, Dashing
}

public class TopDownPlayerController : MonoBehaviour
{
    public float Velocidad;
    public float FuerzaDash;

    private const float MIN_DELTA = 0.2f;
    private const float MAX_DASH_COOLDOWN = 2.0f;

    private bool Dashing;
    private Vector2 Movimiento;
    private Animator animator;
    private float DashCooldown;

    private Rigidbody2D rigidbody2D;
    private PlayerState estado;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Dashing = false;
        DashCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Mover();
        CheckState();
    }

    private void CheckState()
    {
        switch (estado)
        {
            case PlayerState.IDLE:
                break;

            case PlayerState.CaminarDown:
            case PlayerState.CaminarUp:
            case PlayerState.CaminarRight:
            case PlayerState.CaminarLeft:
                rigidbody2D.velocity = Movimiento * Velocidad;
                break;

            case PlayerState.AtacarDown:
            case PlayerState.AtacarUp:
            case PlayerState.AtacarRight:
            case PlayerState.AtacarLeft:
                break;

            case PlayerState.Correr:
                rigidbody2D.velocity = Movimiento * Velocidad;
                break;
        }
        animator.SetTrigger(estado.ToString());
    }

    private void Mover()
    {
        Movimiento = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (estado != PlayerState.Dashing)
        {
            if (Input.GetKey(KeyCode.Joystick1Button0) && DashCooldown <= 0) // TODO: La comprobación del cooldown no funciona
            //if (Input.GetKey(KeyCode.Joystick1Button0))
            {
                rigidbody2D.AddForce(Movimiento * FuerzaDash, ForceMode2D.Impulse);
                StartCoroutine(Dash());
                return;
            }

            if (GetMovimientoAbsoluto() > MIN_DELTA)
            {
                CheckDireccionCaminar();
            }
            else
            {
                estado = PlayerState.IDLE;
            }
        }
    }

    private void CheckDireccionCaminar()
    {
        if (Movimiento.x > MIN_DELTA)
        {
            estado = PlayerState.CaminarRight;
        }
        else if (Movimiento.x < -MIN_DELTA)
        {
            estado = PlayerState.CaminarLeft;
        }
        else if (Movimiento.y > MIN_DELTA)
        {
            estado = PlayerState.CaminarUp;
        }
        else if (Movimiento.y < -MIN_DELTA)
        {
            estado = PlayerState.CaminarDown;
        }
    }

    private float GetMovimientoAbsoluto()
    {
        return Mathf.Abs(Movimiento.x) + Mathf.Abs(Movimiento.y);
    }

    private float GetVelocidadAbsoluta()
    {
        return Mathf.Abs(rigidbody2D.velocity.x) + Mathf.Abs(rigidbody2D.velocity.y);
}

    private IEnumerator Dash()
    {
        DashCooldown = MAX_DASH_COOLDOWN;

        estado = PlayerState.Dashing;
        yield return new WaitUntil(() => GetVelocidadAbsoluta() <= Velocidad);
        estado = PlayerState.IDLE;

        while (DashCooldown > 0)
        {
            yield return new WaitForSeconds(0.1f);
            DashCooldown -= 0.1f;
        }
    }
}
