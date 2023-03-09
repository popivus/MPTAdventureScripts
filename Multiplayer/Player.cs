using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour, IPunObservable
{
    public int lastPlungerId = 0;
    [SerializeField] private int hp = 100;
    public int HP => hp;

    //Поля для передвижения
    private CharacterController controller;
    private Vector3 moveDirection;
    public float moveSpeed = 3, jumpHeight = 100.0f, gravityScale;

    //Поля для камеры
    public float sensitivity;
    private float verticalAngle = 0f;
    [SerializeField] private GameObject neck, screen, model, currentCamera, gunModel, upper, lower;
    public bool canSprint = true;
    private bool isDead = false;
    public bool IsDead => isDead;
    public bool isReady = false;

    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource damageSound, deadSound;

    [SerializeField] private PhotonView view;

    [Header("Статистика")]
    public int kills;
    public int deaths;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        MultiplayerManager.instance.AddPlayer(this);

        if (view.IsMine)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            sensitivity = PlayerPrefs.GetFloat("Sensitivity", 2);
        }
    }

    public void MakeDamage(int hp, int playerProjectileId)
    {
        this.hp -= hp;

        if (view.IsMine) UI.instance.DamageScreen();

        if (this.hp <= 0)
        {
            StartCoroutine(Respawn());
            MultiplayerManager.instance.KilledPlayer(playerProjectileId, view.ViewID);
            UI.instance.Feed($"{MultiplayerManager.instance.GetPlayerByViewId(playerProjectileId).GetView().Owner.NickName} устранил {view.Owner.NickName}");
        }
        else
        {
            animator.SetTrigger("Damaged");
            damageSound.Play();
        }
    }

    public void Heal(int hpPlus, int healNumber)
    {
        if (hp + hpPlus > 100) hp = 100;
        else hp += hpPlus;
        

        if (view.IsMine)
        {
            AudioManager.instance.PlaySound(2);
            view.RPC("PickUpHeal", RpcTarget.All, healNumber);
        }
    }

    [PunRPC]
    private void PickUpHeal(int healNumber)
    {
        StartCoroutine(WaitHeal(healNumber));
    }

    private IEnumerator WaitHeal(int healNumber)
    {
        MultiplayerManager.instance.heals[healNumber].GetComponent<HealthPickup>().HealModel.SetActive(false);
        yield return new WaitForSeconds(15f);
        MultiplayerManager.instance.heals[healNumber].GetComponent<HealthPickup>().HealModel.SetActive(true);
    }

    public PhotonView GetView()
    {
        return view;
    }

    private IEnumerator Respawn()
    {
        isDead = true;
        animator.SetBool("Is Alive", false);
        deadSound.Play();
        controller.enabled = false;
        if (view.IsMine) UI.instance.EnableBlackScreen();
        yield return new WaitForSeconds(1.5f);
        HideModel(true);
        if (view.IsMine) MultiplayerManager.instance.Respawn(gameObject);
        if (view.IsMine) UI.instance.DisableBlackScreen();
        hp = 100;
        isDead = false;
        yield return new WaitForSeconds(0.5f);
        HideModel(false);
        animator.SetBool("Is Alive", true);
        controller.enabled = true;
    }

    private void HideModel(bool isHide)
    {
        upper.SetActive(!isHide);
        lower.SetActive(!isHide);
    }

    private void SetReady(bool ready)
    {
        isReady = ready;
        UI.instance.SetReady(ready);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H)) Debug.Log($"{view.ViewID}: {hp}");
        
        if (!view.IsMine) return;

        //UI приколы
        UI.instance.SetHPValue(hp);
        RaycastHit hit;
        if (Physics.Raycast(currentCamera.transform.position, currentCamera.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Player")
            {
                UI.instance.ShowNickname(hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName);
            }
            else UI.instance.HideNickname();
        }
        else UI.instance.HideNickname();

        if (isDead || UI.instance.isPaused) return;

        //Поворот камеры
        transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensitivity, 0);
        verticalAngle += -Input.GetAxisRaw("Mouse Y") * sensitivity;
        verticalAngle = Mathf.Clamp(verticalAngle, -80f, 80f);
        neck.transform.localEulerAngles = new Vector3(verticalAngle, 0, 0);
        screen.transform.localEulerAngles = new Vector3(verticalAngle, 0, 0);

        if (MultiplayerManager.instance.isTimer) return;

        //Управление       
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
        
        //Разминка
        if (MultiplayerManager.instance.isWarmup && MultiplayerManager.instance.CanVote)
        {
            if (Input.GetKeyUp(KeyCode.F1)) SetReady(!isReady);
        }
    }

    public void Awake()
    {
        if (!view.IsMine)
        {
            currentCamera.SetActive(false);
            gunModel.SetActive(false);
        }
        else 
        {
            model.SetActive(false);
            UI.instance.SetCurrentPlayer(this);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lastPlungerId);
            stream.SendNext(hp);
            stream.SendNext(kills);
            stream.SendNext(deaths);
            stream.SendNext(isReady);
        }
        else
        {
            lastPlungerId = (int)stream.ReceiveNext();
            hp = (int)stream.ReceiveNext();
            kills = (int)stream.ReceiveNext();
            deaths = (int)stream.ReceiveNext();
            isReady = (bool)stream.ReceiveNext();
        }
    }
}
