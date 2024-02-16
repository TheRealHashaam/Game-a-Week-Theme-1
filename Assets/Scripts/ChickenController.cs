using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public Animator Animator;
    public Bow bow;
    public float damping;
    public float upwardForce = 5;
    public float rotationSpeed = 5;
    public float cooldownDuration = 1.0f;
    private float lastShotTime;
    public float MaxHeight;
    public CinemachineVirtualCamera ChickenCamera;
    GameManager GameManager;
    bool canFly=true;
    public AudioSource Wind, Impact, Fly;
    private void Awake()
    {
        GameManager = FindAnyObjectByType<GameManager>();
    }
    public void ShiftToChicken()
    {
        bow.CanAim = false;
        bow._animator.SetBool("Normalize", false);
        bow._animator.SetBool("Aim", false);
        StartCoroutine(DampProjectileSpeed());
    }
    IEnumerator DampProjectileSpeed()
    {
        yield return new WaitForSeconds(0.5f);
        while (Rigidbody.velocity.magnitude > 0.2f)
        {
            Rigidbody.AddForce(-Rigidbody.velocity.normalized * damping, ForceMode.Force);
            transform.Rotate(new Vector3(1,0,0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
        Rigidbody.useGravity = true;
    }

    private void Update()
    {
        if(canFly)
        {
            if(transform.position.y > MaxHeight)
            {
                return;
            }
            else if(Input.GetKeyDown(KeyCode.Space)&& Time.time > lastShotTime + cooldownDuration)
            {
                Animator.SetTrigger("Jump");
                transform.DORotateQuaternion(Quaternion.Euler(-30f, 0, 0), .5f);
                Rigidbody.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
                lastShotTime = Time.time;
                Fly.Play();
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(canFly)
        {
            if(collision.gameObject.CompareTag("Target"))
            {
                Debug.LogError("Score");
                GameManager.score += 10;
                GameManager.UpdateScore();
                GameManager.ScoreSound.Play();
            }
            else
            {
                Debug.LogError("NoScore");
                Impact.Play();
            }
            GameManager.ScoreCam.gameObject.SetActive(true);
            ChickenCamera.gameObject.SetActive(false);
            ChickenCamera.LookAt = null;
            ChickenCamera.Follow = null;
            canFly = false;
            Rigidbody.useGravity = true;
            StartCoroutine(Normalize());
            Wind.Stop();
        }
    }

    IEnumerator Normalize()
    {
        yield return new WaitForSeconds(5f);
        bow.Normalize();
        GameManager.ScoreCam.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        bow.brain.m_DefaultBlend.m_Time = 0.2f;
        Destroy(Rigidbody);
        Destroy(this);
    }




}
