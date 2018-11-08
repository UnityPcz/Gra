using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knefle : MonoBehaviour
{
    private PlatformerCharacter2D Character;
    GunScript gun;
    Button leftbutton;
    FloatingJoystick joystick;


    // Use this for initialization
    void Start()
    {
        Character = GameObject.Find("CharacterRobotBoy").GetComponent<PlatformerCharacter2D>();
        joystick = GameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
        gun = GameObject.Find("Gun").GetComponent<GunScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Jump()
    {
        Character.Move(joystick.Horizontal, false, true);
    }


    public void Move(bool right)
    {
        if (right)
        {
            Character.Move(1, false, false);
        }
        else
        {
            Character.Move(-1, false, false);
        }

    }

    public void Shoot()
    {
        gun.Shoot();
    }

    public void Restart()
    {
        Application.LoadLevel("main");
    }
}
