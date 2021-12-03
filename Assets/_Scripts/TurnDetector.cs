//Shady
using UnityEngine;
using Sirenix.OdinInspector;

[HideMonoScript]
public class TurnDetector : MonoBehaviour
{
    [OnInspectorGUI(nameof(CalculateBounds))]
    [OnValueChanged(nameof(CalculateBounds))]
    [SerializeField] MeshRenderer Mesh = null;
    [DisplayAsString]
    [SuffixLabel("(GLOBAL)")]
    [SerializeField] float Center = 0;

    private Collider _Col = null;

    private void Start()
    {
        _Col = GetComponent<Collider>();
        _Col.isTrigger = true;
        CalculateBounds();
    }//Start() end

    private void CalculateBounds()
    {
        if(Mesh)
        {
            switch(this.tag)
            {
                case "TurnStraight":
                    Center = Mesh.bounds.center.x;
                break;
                default:
                    Center = Mesh.bounds.center.z;
                break;
            }//switch end
        }//if end
        else
        {
            switch(this.tag)
            {
                case "TurnStraight":
                    Center = transform.position.x;
                break;
                default:
                    Center = transform.position.z;
                break;
            }//switch end
        }//else end
    }//CalculateMinMax() end

    public float GetCenter()
    {
        gameObject.SetActive(false);
        return Center;
    }//GetCenter() end

}//class end