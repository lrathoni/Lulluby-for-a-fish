
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;


namespace IO.Behaviour
{
    
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    
    public Transform cameraTransform;
    
    // Speed according to movement direction.
    [Range(0.005f, 0.03f)]
    public float speedMultiplier   = 0.015f;
    [Range(0.1f, 1.0f)]
    public float forwardSpeed      = 1.0f;
    [Range(0.1f, 1.0f)]
    public float backwardSpeed     = 0.5f;
    [Range(0.1f, 1.0f)]
    public float lateralSpeed      = 0.3f;
    // 

    public bool IsMoving()
    {
        return m_dir.sqrMagnitude > 0;
    }
    
    void Start()
    {
        m_playerAgent = GetComponent<NavMeshAgent>();
        Assert.IsNotNull(m_playerAgent);

        // m_playerAgent.acceleration = 1.0f;
        m_playerAgent.updateRotation = false;
        m_playerAgent.updateUpAxis = false;
        // m_playerAgent.updatePosition = false;

        // Hide mouse cursor.
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    void Update()
    {
        // Movement
        int lateralDir = (Input.GetKey(right) ? 1 : 0) - (Input.GetKey(left) ? 1 : 0);
        int longitudinalDir = (Input.GetKey(forward) ? 1 : 0) - (Input.GetKey(backward) ? 1 : 0);
        
        m_dir = new Vector2Int(lateralDir, longitudinalDir);

        if (IsMoving())
        {
            Vector3 move = cameraTransform.right * lateralDir + cameraTransform.forward * longitudinalDir;
            m_playerAgent.Move(move * GetSpeed());
        }
        
    }
    

    /// <summary>
    /// Get speed dependent on direction.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    float GetSpeed()
    {
        if (!IsMoving())
        {
            return 0.0F;
        }
        float longSpeed = m_dir.y > 0 
            ? forwardSpeed 
            : m_dir.y < 0
            ? backwardSpeed
            : 0;
        float latSpeed = lateralSpeed * Mathf.Abs(m_dir.x);
        return speedMultiplier * (longSpeed + latSpeed) / m_dir.sqrMagnitude;
    }
    
    NavMeshAgent m_playerAgent;
    
    Vector2Int m_dir;
}

}
