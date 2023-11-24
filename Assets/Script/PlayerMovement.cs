using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Variables

    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private int pointVie;

    private Vector3 MoveDirection;
    private int count;
    public float fadeInSpeed = 2f;  // Vitesse d'opacité lors de l'augmentation
    public float fadeOutSpeed = 2f; // Vitesse d'opacité lors de la diminution
    public float visibleTime = 2f;  // Temps en secondes que l'image reste visible

    public TextMeshProUGUI countText;
    public TextMeshProUGUI pointVieText;
    public Image bloodImage;
    public GameObject winTextObject;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float jumpHeight;

    [SerializeField] private float turnSpeed;




    // Références

    private CharacterController controller;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        controller = GetComponent<CharacterController>(); // Récupère le Character Controller
        anim = GetComponentInChildren<Animator>();
    }




    void Walk()
    {
        moveSpeed = walkSpeed;
        anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }


    void Run()
    {
        moveSpeed = runSpeed;
        anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    }

    void Idle()
    {
        anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);

    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
    }


    void Move()
    {

        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveZ = Input.GetAxis("Vertical");
        MoveDirection = new Vector3(0, 0, moveZ);
        MoveDirection = transform.TransformDirection(MoveDirection);

        if (isGrounded)
        {

            if (MoveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            {
                Walk();
            }
            else if (MoveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            {
                Run();
            }
            else if (MoveDirection == Vector3.zero)
            {
                //Idle
                Idle();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }

        }

        MoveDirection *= moveSpeed;
        controller.Move(MoveDirection * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // rotation du personnage 
        transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);

    }

    // Update is called once per frame
    void Update()
    {
        Move();

        // on déclanche l'attaque si on clique sur la souris 
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }

        if (pointVie == 0)
        {
            SceneManager.LoadScene(2);
        }

        if (count == 5)
        {
            SceneManager.LoadScene(3);
        }

        SetPVText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);

            count = count + 1;

            SetCountText();
        }

        if (other.gameObject.CompareTag("Trap"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        StartCoroutine(FadeImage(true, fadeInSpeed));
    }

    IEnumerator FadeImage(bool fadeIn, float speed)
    {
        Color targetColor = fadeIn ? new Color(1, 0, 0, 0.8f) : new Color(1, 0, 0, 0); // 0.8f pour 80% d'opacité
        Color startColor = bloodImage.color;

        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime * speed;

            bloodImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        if (fadeIn)
        {
            yield return new WaitForSeconds(visibleTime);
            StartCoroutine(FadeImage(false, fadeOutSpeed));
        }
        else
        {
            pointVie -= 20;
            SetPVText();
        }
    }

    void SetCountText()
    {
        countText.text = "Or : " + count.ToString();

        if (count >= 12)
        {
            winTextObject.SetActive(true);
        }
    }

    void SetPVText()
    {
        pointVieText.text = "PV : " + pointVie.ToString();
    }


}