using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks,IDamageable
{
    [Header("Rotation")]
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity;
    float verticalLookRotation;
    float mouseX, mouseY;
    [Space(2)]

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float runningSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float fallSpeed;
    float inputX, inputY;
    Rigidbody rb;
    [Space(2)]

    [Header("Weapon")]
    [SerializeField] Camera cam;
    [SerializeField] Weapon[] weapons;
    int currentWeaponIndex;
    int previousWeaponIndex = -1;
    [Space(2)]

    [Header("Health")]
    [SerializeField] float maxHealth;
    float currentHealth;

    [Header("PlayerModel")]
    [SerializeField] GameObject playerModel;

    Animator ani;
    PlayerState ps;
    PlayerManager pm;
    PhotonView pv;
    AudioController audi;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<PlayerState>();
        ani = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
        audi = GetComponent<AudioController>();
        pm = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        PlayerInfoUI.instance.UpdateText(1, currentHealth.ToString());
        if (pv.IsMine)
        {
            if (weapons.Length > 0) EquipWeapon(0);
            foreach (Transform child in playerModel.transform)
            {
                child.gameObject.layer = 7;
            }
            foreach(Weapon weapon in weapons)
            {
                foreach(Transform child in weapon.transform)
                {
                    child.gameObject.layer = 6;
                }
            }
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }
    private void Update()
    {
        if (pv.IsMine && RoomManager.Instance.gs != GameState.prepare)
        {
            GetInput();
            weapons[currentWeaponIndex].GetInput();

            Movement();
            Rotation();
            weapons[currentWeaponIndex].Fire(cam);
        }
        
    }
    void Rotation()
    {
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);//Rotate around yAxis

        verticalLookRotation += mouseY * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;//Convert yAxis into localEuler through vector3(localeulerangle is angle)
    }
    void Movement()
    {
        Vector3 moveDir = (transform.forward*inputY+transform.right*inputX)*(ps.isRunning?runningSpeed:speed);
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
    }
    void Jump()
    {
        rb.AddForce(transform.up * jumpForce);
    }
    public void SetGrounded(bool _ground)
    {
        ps.isGrounded = _ground;
        ani.SetBool("IsGrounded", _ground);
    }
    void EquipWeapon(int _index)
    {
        if (_index == previousWeaponIndex)
            return;
        currentWeaponIndex = _index;
        weapons[currentWeaponIndex].gameObject.SetActive(true);

        if (previousWeaponIndex != -1)
        {
            weapons[previousWeaponIndex].gameObject.SetActive(false);
        }

        previousWeaponIndex = currentWeaponIndex;

        if (pv.IsMine)
        {
            HashTransactionPlayer("weaponIndex", currentWeaponIndex);
        }
    }
    void GetInput()
    {
        
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        if ((Input.GetKeyDown(KeyCode.LeftShift)|| Input.GetKey(KeyCode.LeftShift)) && inputX==0 && inputY>0 && ps.isGrounded && weapons[currentWeaponIndex].IsIndexState())
        {
            audi.PlaySound("Running");
            ps.isRunning = true;
            ani.SetBool("IsRunning", true);
            audi.HashTransaction("SoundPlay", "Running");
        }
        if (!Input.GetKey(KeyCode.LeftShift) || inputX!=0 || inputY <= 0 || !ps.isGrounded || !weapons[currentWeaponIndex].IsIndexState())
        {
            audi.StopSound("Running");
            ani.SetBool("IsRunning", false);
            ps.isRunning = false;
            audi.HashTransaction("SoundStop", "Running");
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && ps.isGrounded)
        {
            Jump();
        }
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipWeapon(i);
                break;
            }
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (currentWeaponIndex >= weapons.Length - 1)
            {
                EquipWeapon(0);
            }
            else
            {
                EquipWeapon(currentWeaponIndex + 1);
            }
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (currentWeaponIndex <= 0)
            {
                EquipWeapon(weapons.Length - 1);
            }
            else
            {
                EquipWeapon(currentWeaponIndex - 1);
            }
        }
    }
    void HashTransactionPlayer(string key,object o)
    {
        Hashtable hash = new Hashtable();
        hash.Add(key, o);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!pv.IsMine && targetPlayer == pv.Owner)
        {
            if (changedProps["weaponIndex"] != null)
            {
                EquipWeapon((int)changedProps["weaponIndex"]);
            }
        }
    }
    public void TakeDamage(float damage, Player damageDealer)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage, damageDealer);
    }
    [PunRPC]
    void RPC_TakeDamage(float damage, Player damageDealer)
    {
        if (!pv.IsMine) return;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, Mathf.Infinity);
        PlayerInfoUI.instance.UpdateText(1, currentHealth.ToString());
        if (currentHealth == 0)
        {
            pm.Die(damageDealer);
            
        }
    }
}
