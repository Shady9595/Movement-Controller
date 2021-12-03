//Shady
using UnityEngine;
using Sirenix.OdinInspector;

[HideMonoScript]
public class InputControls : MonoBehaviour
{
    [Title("INPUT CONTROLS", titleAlignment: TitleAlignments.Centered)]
    [DisplayAsString]
    [SerializeField] bool FingerDown  = false;
    [DisplayAsString]
    [SerializeField] float MoveDeltaX = 0.0f;
    private float LastPosX = 0.0f;
    
    public bool TouchDown   => FingerDown;
    public float Horizontal => MoveDeltaX;

    private void Start() => Input.multiTouchEnabled = false;
    
    private void Update()
    {
        // if(SH_Manager.Instance.GameRunning)
            GetInput();
    }//Update() end

    private void GetInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FingerDown = true;
            LastPosX   = Input.mousePosition.x;
        }//if end
        else if (Input.GetMouseButton(0))
        {
            MoveDeltaX = Input.mousePosition.x - LastPosX;
            MoveDeltaX = Mathf.Clamp(MoveDeltaX, -1f, +1f);
            LastPosX   = Input.mousePosition.x;
        }//else if end
        else if (Input.GetMouseButtonUp(0))
        {
            FingerDown = false;
            MoveDeltaX = 0.0f;
        }//else if end
    }//GetInput() end

}//class end