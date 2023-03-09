using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [Header("Бросок")]
    public float throwForce = 1;
    public float throwTorque = 1;
    [Space]

    //Поля для передвижения
    private CharacterController controller;
    private Vector3 moveDirection;
    public float moveSpeed = 3, jumpHeight = 100.0f, gravityScale;

    //Поля для камеры
    public float sensitivity;
    public float distanceToPickUp = 5f;
    private float verticalAngle = 0f;
    public GameObject player, cam, placeForProp, neck;
    private GameObject prop;
    public bool canSprint = true;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player = gameObject;
        controller = GetComponent<CharacterController>();
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 2);
    }

    public void DropProp()
    {
        prop = null;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public IEnumerator SetPropDelay(GameObject newProp, float delay)
    {
        yield return new WaitForSeconds(delay);
        newProp.transform.position = placeForProp.transform.position;
        newProp.transform.rotation = placeForProp.transform.rotation;
        prop = newProp;
    }

    void Update()
    {
        //Перетаскивание предметов
        if (prop != null)
        {
            UIController.instance.SetCornerText($"{ControlsController.drop.ToString()} - отпустить предмет\n{ControlsController.shoot.ToString()} - бросить предмет");
            prop.transform.position = Vector3.Lerp(prop.transform.position, placeForProp.transform.position, Time.deltaTime / 0.075f);
            prop.transform.localEulerAngles = Vector3.Lerp(prop.transform.localEulerAngles, this.transform.localEulerAngles, Time.deltaTime / 0.03f);
            prop.GetComponent<Rigidbody>().Sleep();
            if (Input.GetKeyUp(ControlsController.drop) && !UIController.instance.isPaused) 
            {
                prop.GetComponent<Rigidbody>().AddTorque(cam.transform.right * 0.25f, ForceMode.Impulse);
                prop = null;
            }
            if (Input.GetKeyUp(ControlsController.shoot) && UIController.instance.pauseClosed && !UIController.instance.isPaused) 
            {
                prop.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 5 * throwForce, ForceMode.Impulse);
                prop.GetComponent<Rigidbody>().AddTorque(cam.transform.right * throwTorque, ForceMode.Impulse);
                prop = null;
            }
        }
        else UIController.instance.cornerPanel.SetActive(false);

        //Пауза или нет
        if (UIController.instance.isPaused)
        {
            return;
        } 

        //Передвижение
        float y = moveDirection.y;
        moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));
        moveDirection.y = y;
        if (controller.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = 0f;
        }
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            Debug.Log("Прыжок");
            moveDirection.y = jumpHeight;
        }
        moveDirection.y += Physics.gravity.y * gravityScale * Time.deltaTime;
        float _moveSpeed = (canSprint && Input.GetKey(ControlsController.run) && Input.GetAxisRaw("Vertical") > 0) ? moveSpeed * 2 : moveSpeed;
        controller.Move(moveDirection * _moveSpeed * Time.deltaTime);
        if (moveDirection.x != 0f && moveDirection.z != 0f && controller.isGrounded)
        {
            if (moveSpeed == _moveSpeed) AudioManager.instance.PlayStepSound();
            else AudioManager.instance.PlayRunSound();
        }
        
        //Поворот камеры
        transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensitivity, 0);
        verticalAngle += -Input.GetAxisRaw("Mouse Y") * sensitivity;
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f);
        neck.transform.localEulerAngles = new Vector3(verticalAngle, 0, 0);

        //Взгляд
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distanceToPickUp))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.yellow);
            switch (hit.transform.tag)
            {
                case "Card":
                {
                    UIController.instance.SetText($"{ControlsController.interact.ToString()} - посмотреть карту");
                    if (Input.GetKeyUp(ControlsController.interact)) TurnOverCard(hit.transform.gameObject);
                    break;
                }
                case "Tube":
                {
                    UIController.instance.SetText($"{ControlsController.interact.ToString()} - повернуть трубу");
                    if (Input.GetKeyUp(ControlsController.interact)) hit.transform.gameObject.GetComponent<TubeController>().Turn();
                    break;
                }
                case "Lifted":
                {
                    if (prop == null)
                    {
                        UIController.instance.SetText($"{ControlsController.interact.ToString()} - подобрать предмет");
                        if (Input.GetKeyUp(ControlsController.interact))
                        {
                            prop = hit.transform.gameObject;
                        }
                    }
                    break;
                }
                case "XO Plate":
                {
                    UIController.instance.SetText($"{ControlsController.interact.ToString()} - поставить крестик");
                    if (Input.GetKeyUp(ControlsController.interact)) hit.transform.gameObject.GetComponent<XOPlate>().PlaceX();
                    break;
                }
                case "Bonus":
                {
                    UIController.instance.SetText($"{ControlsController.interact.ToString()} - скачать лекцию");
                    if (Input.GetKeyUp(ControlsController.interact)) hit.transform.gameObject.GetComponent<BonusCollect>().Collect();
                    break;
                }
                case "Level Enter":
                {
                    if (hit.transform.GetComponent<LevelEnter>().isCompleted)
                    {
                        UIController.instance.SetText($"Уже выполнено");
                    }
                    else
                    {
                        UIController.instance.SetText($"{ControlsController.interact.ToString()} - зайти в кабинет");
                        if (Input.GetKeyUp(ControlsController.interact)) StartCoroutine(GameManager.instance.LoadLevel(hit.transform.GetComponent<LevelEnter>().levelNumber));
                    }
                    break;
                }
                case "Bulb":
                {
                    UIController.instance.SetText($"{ControlsController.interact.ToString()} - взять лампочку");
                    if (Input.GetKeyUp(ControlsController.interact)) 
                    {
                        Destroy(hit.transform.gameObject);
                        GameManagerMaze.instance.BulbCollected();
                    }
                    break;
                }
                case "Answer":
                {
                    UIController.instance.SetText($"{ControlsController.interact.ToString()} - выбрать ответ");
                    if (Input.GetKeyUp(ControlsController.interact)) QuizController.instance.Answer(int.Parse(hit.transform.name));
                    break;
                }
                case "Bullets":
                {
                    if (prop == null)
                    {
                        UIController.instance.SetText($"{ControlsController.interact.ToString()} - подобрать ядро");
                        if (Input.GetKeyUp(ControlsController.interact))
                        {
                            var newBullet = Instantiate(GameManagerBoss.instance.bullet, placeForProp.transform.position, placeForProp.transform.rotation);
                            prop = newBullet;
                        }
                    }
                    break;
                }
                default:
                {
                    UIController.instance.textObject.SetActive(false);
                    break;
                }
            }
        }
        else UIController.instance.textObject.SetActive(false);

        if (Input.GetKeyUp(ControlsController.shoot)) UIController.instance.pauseClosed = true;
    }

    public void TurnOverCard(GameObject card)
    {
        StartCoroutine(GameManagerPair.instance.SetCard(card));
    }

    public static MoveController instance;
    public void Awake()
    {
        instance = this;
    }
}
