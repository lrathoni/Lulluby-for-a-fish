
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;


namespace Behaviour
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
    public float speedMultiplier   = 2.0f;
    public float forwardSpeed      = 1.5f;
    public float backwardSpeed     = 0.5f;
    public float lateralSpeed      = 1.0f;
    // 
    public float stepSize = 0.5f;

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
            m_playerAgent.speed = GetSpeed();
            m_playerAgent.Move(move * stepSize);
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
        float longitudinalSpeed = m_dir.x > 0 ? forwardSpeed : backwardSpeed;
        return (longitudinalSpeed + lateralSpeed) * (speedMultiplier / m_dir.magnitude);
    }
    
    NavMeshAgent m_playerAgent;
    
    Vector2Int m_dir;
}

}
