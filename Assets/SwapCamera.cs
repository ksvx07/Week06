using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SwapCamera : MonoBehaviour
{
    // [SerializeField] private GameObject cam1;
    // [SerializeField] private GameObject cam2;

    [SerializeField] private CinemachineInputAxisController cinemachineInputAxisController;

    private void Awake()
    {
        Init();

    }

    private void Init()
    {
        cinemachineInputAxisController.Controllers[0].Enabled = false;
        cinemachineInputAxisController.Controllers[1].Enabled = false;
        cinemachineInputAxisController.Controllers[1].Enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     cam1.SetActive(true);
        //     cam2.SetActive(false);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     cam1.SetActive(false);
        //     cam2.SetActive(true);
        // }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            cinemachineInputAxisController.Controllers[0].Enabled = true;
            cinemachineInputAxisController.Controllers[1].Enabled = true;
            cinemachineInputAxisController.Controllers[1].Enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            cinemachineInputAxisController.Controllers[0].Enabled = false;
            cinemachineInputAxisController.Controllers[1].Enabled = false;
            cinemachineInputAxisController.Controllers[1].Enabled = false;
        }

    }
}
