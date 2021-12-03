//Shady
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public enum Movements{Straight, Right, Left}

[HideMonoScript]
public class PlayerController : MonoBehaviour
{
    [Title("PLAYER CONTROLLER", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] bool           TakeInput = false;
    [DisplayAsString]
    [SerializeField] Movements Movement = Movements.Straight;
    [DisplayAsString]
    [SerializeField] bool IsMoving       = false;
    [SerializeField] float ForwardSpeed  = 1.0f;
    [Space]
    [DisplayAsString]
    [SerializeField] bool CanStrafe      = true;
    [SerializeField] float ClampX        = 3.0f;
    [SerializeField] float StrafeSpeed   = 1.0f;
    [SerializeField] bool ClampRotationX = false;
    [ShowIf(nameof(ClampRotationX), true)]
    [SerializeField] float ClampRotX     = 20.0f;

    [Space]
    [SerializeField] CameraFollow  CamFollow = null;  
    [SerializeField] InputControls Inputs    = null;
    [Space]
    [SerializeField] Transform ChildMesh = null;
    [Space]
    [SerializeField] Transform RayOrigin = null;
    [SerializeField] LayerMask Layer;

    
    private int           Max     = 1;
    private Transform     Self    = null;
    private Rigidbody     RB      = null;
    private Collider      Col     = null;
    private float         InputX  = 0.0f;
    public Vector3 MovePos, StrafePos = Vector3.zero;
    public Vector3 Rot = Vector3.zero;

    private float   _minX, _maxX;
    // For Rotation Clamping
    private float _targetAngleX;
    private float GlobalEulerX;

    private float Forward = 0f;

    private void Start() => Initialize();

    private void Initialize()
    {
        Self    = transform;
        RB      = GetComponent<Rigidbody>();
        Col     = GetComponent<Collider>();
        _minX   = Self.position.x - ClampX;
        _maxX   = Self.position.x + ClampX;
    }//Initialize() end
    
    private void Update()
    {
        Forward = Inputs.TouchDown ? (Mathf.Lerp(Forward, ForwardSpeed, 5f * Time.deltaTime)) : 0.0f;
        InputX  = CanStrafe ? Inputs.Horizontal : 0.0f;
    }//Update() end

    private void FixedUpdate()
    {
        MovePos = transform.forward * Forward * Time.fixedDeltaTime;
        Self.position += MovePos;
        
        StrafePos = ChildMesh.localPosition + Vector3.right * InputX * StrafeSpeed * Time.fixedDeltaTime;
        // StrafePos.x += InputX * StrafeSpeed * Time.fixedDeltaTime;
        StrafePos.x = Mathf.Clamp(StrafePos.x, _minX, _maxX);

        ChildMesh.localPosition = StrafePos;

        Rot = Self.eulerAngles;
        RaycastHit hit;
        Debug.DrawRay(RayOrigin.position, RayOrigin.forward, Color.black);
        if(Physics.Raycast(RayOrigin.position, RayOrigin.forward, out hit, 0.5f, Layer))
        {
            if(hit.transform)
                Rot.x = (Vector3.Angle(hit.normal, Vector3.forward) - 90f) * -1f;
            else
                Rot.x = 0f;
        }//if end
        else
            Rot.x = 0f;
        Self.eulerAngles = new Vector3(Mathf.LerpAngle(Self.eulerAngles.x, Rot.x, Time.fixedDeltaTime * 10f), Rot.y, Rot.z);
    }//FixedUpdate() end

    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "TurnStraight":
                Turn(other.GetComponent<TurnDetector>().GetCenter(), Movements.Straight);
            break;
            case "TurnRight":
                Turn(other.GetComponent<TurnDetector>().GetCenter(), Movements.Right);
            break;
            case "TurnLeft":
                Turn(other.GetComponent<TurnDetector>().GetCenter(), Movements.Left);
            break;
        }//switch end
    }//OnTriggerEnter() end

    private void StopRigidbody()
    {
        RB.isKinematic = true;
        gameObject.SetActive(false);
    }//StopRigidbody() end

    private void Jump(GameObject Obj)
    {
        if(Obj)
            Obj.SetActive(false);
        TakeInput = false;
        IsMoving = true;
        RB.AddForce(new Vector3(0.0f, Mathf.Sqrt(3.0f * -2f * Physics.gravity.y), 8.0f) , ForceMode.VelocityChange);
        Invoke(nameof(Landed), 1.0f);
    }//Jump() end

    private void Landed() => TakeInput = true;

    // private void HandleStrafeRotation()
    // {
    //     Self.rotation = Quaternion.RotateTowards(Self.rotation, 
    //                                              Quaternion.Euler(0f, InputX * 15f, 0f), 
    //                                              Time.deltaTime * 200f);
    // }//HandleStrafeRotation() end

    private void HandleClampRotation()
    {
        GlobalEulerX = Self.eulerAngles.x;

        if(GlobalEulerX > (ClampRotX) && GlobalEulerX < 180f)
            _targetAngleX = ClampRotX;
        else if(GlobalEulerX < (360f-ClampRotX) && GlobalEulerX > 180f)
            _targetAngleX = 360f - ClampRotX;
        else
            return;

        // Self.eulerAngles = new Vector3 (Mathf.LerpAngle(Self.eulerAngles.x, _targetAngleX, Time.deltaTime * 10f), 0.0f, 0.0f);
        Self.eulerAngles = new Vector3 (_targetAngleX, 0.0f, 0.0f);
    }//HandleClampRotation()

    private void Turn(float Center, Movements Direction)
    {
        Movement  = Direction;
        if(Movement == Movements.Straight)
            Self.DOMoveX(Center, 0.3f);
        else
            Self.DOMoveZ(Center, 0.3f);
        Self.DORotate(Vector3.up * (Direction == Movements.Right ? 90f : Direction == Movements.Left ? -90f : 0f), 0.3f);
    }//TurnStraight() end

}//class end