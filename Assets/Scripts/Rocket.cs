using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip MainEngine;
    [SerializeField] AudioClip Success;
    [SerializeField] AudioClip Death;

    [SerializeField] ParticleSystem MainEngineParticles;
    [SerializeField] ParticleSystem SuccessParticles;
    [SerializeField] ParticleSystem DeathParticles;

	static int currentLevel = 0;
    int sceneCount = 0;

    Rigidbody rigidBody;
    AudioSource audioSource;
    
    enum  State { Alive, Dying, Transcending};
    State state = State.Alive;

 
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentLevel = SceneManager.GetActiveScene().buildIndex;

        sceneCount = SceneManager.sceneCountInBuildSettings;
        print("current Level:" + currentLevel.ToString());
        print("scene count :" + sceneCount.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive)
        {
            HandleLevelInput();
            HandleReset();
            HandleThrust();
            HandleRotate();
            CleanYRotation();
        }
    }

    private void HandleReset()
    {
        if(Input.GetButton("Cancel"))
        {
            SceneManager.LoadScene(currentLevel);
        }
    }


    private void CleanYRotation()
    {
        Quaternion currentQuat = transform.rotation;
            
        //print(currentQuat);
        currentQuat.y = 0f;
        transform.rotation = currentQuat;
        //transform.rotation = Quaternion.AngleAxis(0f, Vector3.up);   
    }

    private void HandleRotate()
    {
        HandleForwardRotation();
        HandleLeftRotation(); //not used, and held by rigidbody constraints
    }

    private void HandleLevelInput()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(2);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            SceneManager.LoadScene(3);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            SceneManager.LoadScene(4);
        }
        if (Input.GetKey(KeyCode.Alpha6))
        {
            SceneManager.LoadScene(5);
        }
    }


    private void HandleForwardRotation()
    {

        float rotationThisFrame = Time.deltaTime * rcsThrust;
        if (Input.GetKey(KeyCode.A))
        {
            //print("Rotate right");
            rigidBody.freezeRotation = true;
            transform.Rotate(Vector3.forward * rotationThisFrame);
            rigidBody.freezeRotation = false;

        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidBody.freezeRotation = true;
            transform.Rotate(-Vector3.forward * rotationThisFrame);
            rigidBody.freezeRotation = false;
            //print("Rotate left");
        }
    }

    private void HandleLeftRotation()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //print("Rotate forward");
            rigidBody.freezeRotation = true;
            transform.Rotate(Vector3.left);
            rigidBody.freezeRotation = false;

        }
        else if (Input.GetKey(KeyCode.S))
        {
            rigidBody.freezeRotation = true;
            transform.Rotate(-Vector3.left);
            rigidBody.freezeRotation = false;
            //print("Rotate back");
        }
    }

    private void HandleThrust()
    {
        if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button15)) //can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            MainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = Time.deltaTime * mainThrust;
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(MainEngine);
        }
        MainEngineParticles.Play();

    }

    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }


    private void LoadNextScene()
    {
        if (currentLevel < sceneCount -1 )
        {
            currentLevel++;
        }
        else
        {
            currentLevel = sceneCount - 1;
        }
        SceneManager.LoadScene(currentLevel);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("collided Friendly");
                break;
            case "Fuel":
                print("Refueled");
                break;
            case "Finish":
                print("Finish");
                HandleFinish();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                HandleExplode();
                Invoke("LoadCurrentLevel", levelLoadDelay);
                break;
        }
    }

    private void HandleFinish()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(Success);
        SuccessParticles.Play();
    }

    private void HandleExplode()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(Death);
        DeathParticles.Play();
        //nada
    }
}
