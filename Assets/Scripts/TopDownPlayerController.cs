using System.Collections;
using UnityEngine;

public enum PlayerState
{
    IDLE, CaminarDown, CaminarUp, CaminarLeft, CaminarRight, Correr, Dash, Dashing
}

public class TopDownPlayerController : MonoBehaviour
{
    public float Velocidad;
    public float FuerzaDash;

    private const float MIN_DELTA = 0.2f;
    private bool Dashing;
    private Vector2 Movimiento;
    private Animation animation;

    private Rigidbody2D rigidbody2D;
    private PlayerState estado;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animation = GetComponent<Animation>();
        Dashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        Mover();

        switch(estado)
        {
            case PlayerState.IDLE:
                animation.Play("IDLE");
                break;

            case PlayerState.CaminarDown:
            case PlayerState.CaminarUp:
            case PlayerState.CaminarRight:
            case PlayerState.CaminarLeft:

                animation.Play(estado.ToString());
                rigidbody2D.velocity = Movimiento * Velocidad;
                break;

            case PlayerState.Correr:
                rigidbody2D.velocity = Movimiento * Velocidad;
                break;

            case PlayerState.Dash:
                rigidbody2D.AddForce(Movimiento * FuerzaDash, ForceMode2D.Impulse);
                estado = PlayerState.Dashing;
                break;
        }
    }

    private void Mover()
    {
        Movimiento = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (estado != PlayerState.Dashing)
        {
            if (Input.GetKey(KeyCode.J))
            {
                StartCoroutine(Dash());
                return;
            }

            if (Mathf.Abs(Movimiento.x + Movimiento.y) > MIN_DELTA)
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

        Debug.Log(estado.ToString());
    }

    private IEnumerator Dash()
    {
        estado = PlayerState.Dash;
        yield return new WaitUntil(() => Mathf.Abs(rigidbody2D.velocity.x + rigidbody2D.velocity.y) < Velocidad + MIN_DELTA);
        estado = PlayerState.IDLE;
    }
}
