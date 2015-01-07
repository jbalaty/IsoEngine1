using UnityEngine;
using System.Collections;

public class CharacterControllerScript : MonoBehaviour
{
    public float speed = 0.5f;

    Animator[] AnimControllers;
    int _lastDirection = 3;
    // Use this for initialization
    void Start()
    {
        AnimControllers = this.GetComponentsInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var currentDirection = AnimControllers[0].GetInteger("Direction");
        var currentIsIdle = AnimControllers[0].GetBool("IsIdle");
        var newDirection = _lastDirection;
        var newIsIdle = true;
        var translation = new Vector2();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newDirection = 0;
            newIsIdle = false;
            translation = Vector2.right * -1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            newDirection = 2;
            newIsIdle = false;
            translation = Vector2.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            newDirection = 1;
            newIsIdle = false;
            translation = Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            newDirection = 3;
            newIsIdle = false;
            translation = Vector2.up * -1;
        }

        var v = new Vector3(translation.x, 0f, translation.y) * speed * Time.deltaTime;
        this.transform.position += v;
        SetDirection(newDirection);
        SetIsIdle(newIsIdle);
        _lastDirection = newDirection;
        //Debug.Log("Axis values: " + horizontal + " | " + vertical + " (speed: " + speed + ", dir: " + direction + ")");
    }

    void SetDirection(int dir)
    {
        foreach (Animator ac in AnimControllers)
        {
            ac.SetInteger("Direction", dir);
        }
    }

    void SetIsIdle(bool isIdle)
    {
        foreach (Animator ac in AnimControllers)
        {
            ac.SetBool("IsIdle", isIdle);
        }
    }
}
