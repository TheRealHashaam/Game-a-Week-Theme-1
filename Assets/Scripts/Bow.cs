using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using StarterAssets;
using UnityEngine.UI;
using Cinemachine;
using System.Security.Claims;
public class Bow : MonoBehaviour
{
    public GameObject DefaultView;
    public GameObject AimView;
    public LineRenderer String;
    public Vector3 Defaultstringpos;
    public Vector3 Aimstringpos;
    Vector3 currentPosition;
    public StarterAssetsInputs starterAssetsInputs;
    public float stringspeed;
    public Animator _animator;
    bool _hasArrow = false;
    bool _canAim;
    bool _canRotate;
    bool _canfire = false;
    public float rotationOffset = 45.0f;
    public float desiredRotationSpeed = 1f;
    public ProjectileCalculator projectileCalculator;
    public Image Crosshair;
    public float duration;
    bool LineInUse = false;
    public float LineDuration;
    public GameObject projectilePrefab;
    public float shootForce;
    public float dipFactor;
    public Transform Chicken;
    public GameObject ChickenParent;
    public bool CanAim = true;
    public CinemachineVirtualCamera ChickenCamera;
    public CinemachineBrain brain;
    public bool firingchicken = false;
    public AudioSource AimSound;
    public AudioSource FireSound;
    public AudioSource Arrow;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    void Aim()
    {
        _canAim = false;
        StartCoroutine(AimDelay());
       
    }
    public void HasArrow()
    {
        _canfire = true;
        LineInUse = false;
       
    }

    public void StringFire()
    {
        LineInUse = true;
        //StartCoroutine(Reset_Delay());
    }

    IEnumerator Reset_Delay()
    {
        yield return new WaitForSeconds(LineDuration);
        LineInUse = false;
    }


    IEnumerator AimDelay()
    {
        if(!_hasArrow)
        {
            yield return new WaitForSeconds(0.1f);
            _hasArrow = true;
            Arrow.Play();
            _animator.SetBool("HasArrow", true);
            yield return new WaitForSeconds(0.5f);
            DefaultView.SetActive(false);
            AimView.SetActive(true);
            _canRotate = true;
            HasArrow();
            AimSound.Play();
        }
        else
        {
            DefaultView.SetActive(false);
            AimView.SetActive(true);
            _canRotate = true;
            HasArrow();
            AimSound.Play();
        }
       
    }
    
    void OnShoot()
    {
        if(CanAim)
        {
            if(_canfire)
            {

                ShootProjectile();
                _canfire = false;
                _animator.SetTrigger("Shoot");
                _animator.SetBool("HasArrow", false);
                _hasArrow = false;
            }
            Debug.LogError("Fire");
        }
    }

    private void Update()
    {
        if(CanAim)
        {
            if(starterAssetsInputs.aim)
            {
           
                _animator.SetBool("Aim", true);
                Quaternion targetRotation;

                targetRotation = Quaternion.LookRotation(Camera.main.transform.forward + (Camera.main.transform.right * 0.1f)) * Quaternion.Euler(0, rotationOffset, 0);
                if(_canRotate)
                {
                    float f;
                    f = Camera.main.transform.localEulerAngles.x;
                    if (f > 180)
                    {
                        f -= 360;
                    }
                    f = Mathf.Clamp(f, -10, 0);
                
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(f, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z), desiredRotationSpeed);
                    if(!LineInUse)
                    {
                        currentPosition = Vector3.Lerp(currentPosition, Aimstringpos, stringspeed * Time.deltaTime);
                    }
                    //projectileCalculator.CalculateTrajectory();
                    CrossHairfade(1);
                }

                if (_canAim)
                {
                    Aim();
                }
           
            }
            else
            {
                projectileCalculator.ResetLine();
                currentPosition = Vector3.Lerp(currentPosition, Defaultstringpos, stringspeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.identity,0.2f);
                DefaultView.SetActive(true);
                AimView.SetActive(false);
                _animator.SetBool("Aim", false);
                _canAim = true;
                _canRotate = false;
                CrossHairfade(0);
            }
            if(LineInUse)
            {
                currentPosition = Vector3.Lerp(currentPosition, Defaultstringpos, stringspeed * Time.deltaTime);
            }
            String.SetPosition(1, currentPosition);
        }
    }
    public void CrossHairfade(float val)
    {
        Crosshair.DOFade(val, duration);
    }
    void ShootProjectile()
    {
        StartCoroutine(Shoot_Delay());
    }

    IEnumerator Shoot_Delay()
    {
        firingchicken = true;
        Camera mainCamera = Camera.main;
        Vector3 centerScreen = new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, 0);
        Ray ray = mainCamera.ScreenPointToRay(centerScreen);
        ChickenParent.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        StringFire();
        GameObject projectile = Instantiate(projectilePrefab, Chicken);
        projectile.GetComponent<ChickenController>().bow = this;
        projectile.GetComponent<ChickenController>().ChickenCamera = ChickenCamera;
        projectile.transform.parent = null;
        projectile.transform.DORotateQuaternion(Quaternion.identity, 0.5f);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        Vector3 direction = ray.direction;
        projectileRb.AddForce(direction * shootForce, ForceMode.Impulse);
        projectileRb.AddForce(Vector3.down * dipFactor, ForceMode.Impulse);
        projectile.GetComponent<ChickenController>().ShiftToChicken();
        ChickenCamera.Follow = projectile.transform;
        ChickenCamera.LookAt = projectile.transform;
        brain.m_DefaultBlend.m_Time = 1f;
        ChickenCamera.gameObject.SetActive(true);
        DefaultView.SetActive(false);
        AimView.SetActive(false);
        CrossHairfade(0);
        FireSound.Play();
    }
    public void Normalize()
    {
        firingchicken = false;
        CanAim = true;
        ChickenCamera.gameObject.SetActive(false);
        DefaultView.SetActive(true);
        _animator.SetBool("Normalize", true);
    }

}
