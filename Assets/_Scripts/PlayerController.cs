//Shady
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public enum Movements{Straight, Right, Left}

[HideMonoScript]
public class PlayerController : MonoBehaviour
{
    [Title("PLAYER CONTROLLER", titleAlignment: TitleAlignments.Centered)]
    [DisplayAsString]
    [SerializeField] Movements Movement = Movements.Straight;
    [DisplayAsString]
    [SerializeField] bool CanMove, CanStrafe = false;
    [SerializeField] bool MoveOnFingerDown   = false;
    [SerializeField] float ForwardSpeed      = 1.0f;
    [SerializeField] float StrafeSpeed       = 1.0f;
    [SerializeField] float ClampX            = 3.0f;
    [SerializeField] bool HandleSlopes       = true;
    [ShowIf("HandleSlopes", true)]
    [SerializeField] Transform RayOrigin     = null;
    [ShowIf("HandleSlopes", true)]
    [SerializeField] LayerMask Layer;
    [Space]
    [SerializeField] bool CanJump = false;
    [ShowIf("CanJump", true)]
    [SerializeField] Vector3 JumpForce = new Vector3(0f, 3f, 8f);
    [Space]
    [SerializeField] InputControls Inputs    = null;
    [Space]
    [SerializeField] bool debug = false;
    [ShowIf("debug", true)]
    [DisplayAsString]
    [SerializeField] float Horizontal, Vertical  = 0f;
    [ShowIf("debug", true)]
    [DisplayAsString]
    [SerializeField] Vector3 MovePos, StrafePos, SlopeRot = Vector3.zero;

    // Private Variables
    private Transform Self = null;
    private Transform Mesh = null;
    private Rigidbody RB   = null;
    private Collider  Col  = null;
    private RaycastHit hit = new RaycastHit();

    private void Start() => Initialize();

    private void Initialize()
    {
        Self = transform;
        Mesh = Self.GetChild(0);
        RB   = GetComponent<Rigidbody>();
        Col  = GetComponent<Collider>();

        CanMove   = true;
        CanStrafe = true;
    }//Initialize() end
    
    private void Update()
    {
        if(CanMove)
            Vertical = Mathf.Lerp(Vertical, (MoveOnFingerDown ? (Inputs.TouchDown ? ForwardSpeed : 0.0f) : ForwardSpeed), 5f * Time.deltaTime);
        Horizontal = (CanStrafe ? Inputs.Horizontal : 0.0f) * StrafeSpeed;
        if(Input.GetKeyDown(KeyCode.Space) && CanJump)
            Jump();
    }//Update() end

    private void FixedUpdate()
    {
        HandleMovement();

        if(HandleSlopes)
            HandleSlopeRotation();
    }//FixedUpdate() end

    private void HandleMovement()
    {
        // Forward Movement
        MovePos        = Self.forward * Vertical * Time.fixedDeltaTime;
        Self.position += MovePos;
        
        // Strafe Movement
        StrafePos          = Mesh.localPosition + Vector3.right * Horizontal * Time.fixedDeltaTime;
        StrafePos.x        = Mathf.Clamp(StrafePos.x, -ClampX, ClampX);
        Mesh.localPosition = StrafePos;
    }//handleMovement() end

    private void HandleSlopeRotation()
    {
        if(debug)
            Debug.DrawRay(RayOrigin.position, RayOrigin.forward * 0.75f, Color.black);
        SlopeRot = Self.eulerAngles;
        if(Physics.Raycast(RayOrigin.position, RayOrigin.forward, out hit, 0.75f, Layer))
        {
            if(hit.transform)
            {
                switch(Movement)
                {
                    case Movements.Straight:
                        SlopeRot.x = (Vector3.Angle(hit.normal, Vector3.forward) - 90f) * -1f;
                    break;
                    case Movements.Right:
                        SlopeRot.x = (Vector3.Angle(hit.normal, Vector3.right) - 90f) * -1f;
                    break;
                    case Movements.Left:
                        SlopeRot.x = (Vector3.Angle(hit.normal, Vector3.right) - 90f);
                    break;
                }//switch end
            }//if end
            else
                SlopeRot.x = 0f;
        }//if end
        else
            SlopeRot.x = 0f;

        Self.eulerAngles = new Vector3(Mathf.LerpAngle(Self.eulerAngles.x, SlopeRot.x, Time.fixedDeltaTime * 10f), SlopeRot.y, SlopeRot.z);
    }//HandleSlopeRotation() end

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

    private void Jump() => RB.AddForce(new Vector3(JumpForce.x, Mathf.Sqrt(JumpForce.y * -2f * Physics.gravity.y), JumpForce.z) , ForceMode.VelocityChange);

    private void Turn(float Center, Movements Direction)
    {
        Movement = Direction;
        if(Movement == Movements.Straight)
            Self.DOMoveX(Center, 0.3f);
        else
            Self.DOMoveZ(Center, 0.3f);
        Self.DORotate(Vector3.up * (Direction == Movements.Right ? 90f : Direction == Movements.Left ? -90f : 0f), 0.3f);
    }//TurnStraight() end

}//class end