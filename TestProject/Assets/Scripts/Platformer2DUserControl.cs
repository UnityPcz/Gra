using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour
{
    private PlatformerCharacter2D m_Character;
    private bool m_Jump;
    public int hitpoints = 100;
    public int[] Ammo;
    int currentGun;
    public GameObject MyGun
    {
        get { return myGun; }
        set
        {
            myGun = value;
            gs = myGun.GetComponent<GunScript>();
            if (gs == null) gs = myGun.GetComponent<ShotgunScript>();
        }
    }
    public GameObject myGun;
    public GameObject[] gunPrefabs;
    public GunScript gs;

    FloatingJoystick joystick;

    private void Awake()
    {
        joystick = GameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
        MyGun = gameObject.GetComponentInChildren<GunScript>().gameObject;
        m_Character = GetComponent<PlatformerCharacter2D>();
    }


    private void Update()
    {
        float aiminput = Input.GetAxis("Vertical");
        gs.Aim(aiminput);

        m_Character.Move(joystick.Horizontal, false, false);
        if (joystick.Vertical != 0)
        {
            if (joystick.Horizontal >= 0)
                gs.AimAt(joystick.Vertical);
            else
                gs.AimAt(-joystick.Vertical);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchGun(false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchGun(true);
        }
        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
    }


    public void SwitchGun(bool plus)
    {
        if (plus)
            currentGun++;
        else
            currentGun--;

        if (currentGun > gunPrefabs.Length - 1) currentGun = 0;
        if (currentGun < 0) currentGun = gunPrefabs.Length - 1;
        Quaternion gunRotation = MyGun.transform.rotation;
        Destroy(MyGun);
        MyGun = Instantiate(gunPrefabs[currentGun], transform);
        MyGun.transform.rotation = gunRotation;
    }

    public void SwitchGun(int gunIndex)
    {
        if (gunIndex >= 0 && gunIndex <= gunPrefabs.Length - 1)
        {
            MyGun = gunPrefabs[gunIndex];
            currentGun = gunIndex;

        }
    }

    public void Shoot()
    {
        if (Ammo[currentGun] > 0)
        {
            if (gs.Shoot())
                Ammo[currentGun]--;
        }
    }

    public void Jump()
    {
        m_Character.Move(joystick.Horizontal, false, true);
    }
    private void FixedUpdate()
    {
        // Read the inputs.
        bool crouch = Input.GetKey(KeyCode.LeftControl);
        float h = Input.GetAxis("Horizontal");
        // Pass all parameters to the character control script.  
        if (h != 0 || crouch || m_Jump)
            m_Character.Move(h, crouch, m_Jump);
        m_Jump = false;
    }
}

