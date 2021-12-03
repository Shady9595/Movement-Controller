//Shady
using UnityEngine;
using Sirenix.OdinInspector;

[HideMonoScript]
public class CameraFollow : MonoBehaviour
{
    [Title("SHADY", titleAlignment: TitleAlignments.Centered)]
    public enum UpdateIn{Update, FixedUpdate, LateUpdate}
    [SerializeField] UpdateIn updateIn;
    [SerializeField] Transform Car;
    [Tooltip("How closely the camera follows the car's position. The lower the value, the more the camera will lag behind.")]
    public float CameraFollowSpeed = 10.0f;
    [Tooltip("How closely the camera matches the car's velocity vector. The lower the value, the smoother the camera rotations, but too much results in not being able to see where you're going.")]
    public float CameraRotationSpeed = 5.0f;
    private Transform Self;
    private Quaternion look;

    void Start()
    {
        Self = GetComponent<Transform>();
        if(!Car)
            Car = GameObject.FindGameObjectWithTag("Player").transform;
    }//Start() end

    void Update()
    {
        if(updateIn == UpdateIn.Update)
        {
            // Moves the camera to match the car's position.
            Self.localPosition = Vector3.Lerp(Self.localPosition, Car.localPosition, CameraFollowSpeed * Time.deltaTime);
            look = Quaternion.LookRotation(Car.forward);
            // Rotate the camera towards the velocity vector.
            look = Quaternion.Slerp(Self.rotation, look, CameraRotationSpeed * Time.deltaTime);                
            Self.rotation = look;
        }//if end
    }//Update() end

    void FixedUpdate()
    {
        if(updateIn == UpdateIn.FixedUpdate)
        {
            // Moves the camera to match the car's position.
            Vector3 pos = new Vector3(Car.localPosition.x, Car.localPosition.y, Car.localPosition.z);
            Self.localPosition = Vector3.Lerp(Self.localPosition, pos, CameraFollowSpeed * Time.fixedDeltaTime);
            look = Quaternion.LookRotation(Car.forward);
            // Rotate the camera towards the velocity vector.
            look = Quaternion.Slerp(Self.rotation, look, CameraRotationSpeed * Time.fixedDeltaTime);                
            Self.rotation = look;
        }//if end
    }//FixedUpdate() end

    void LateUpdate()
    {
        if(updateIn == UpdateIn.LateUpdate)
        {
            // Moves the camera to match the car's position.
            Self.position = Vector3.Lerp(Self.position, Car.position, CameraFollowSpeed * Time.deltaTime);
            look = Quaternion.LookRotation(Car.forward);
            // Rotate the camera towards the velocity vector.
            look = Quaternion.Slerp(Self.rotation, look, CameraRotationSpeed * Time.deltaTime);                
            Self.rotation = look;
        }//if end
    }//LateUpdate() end

}//class end