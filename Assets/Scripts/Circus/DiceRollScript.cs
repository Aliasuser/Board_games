using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rb;
    Vector3 pos;
    [SerializeField] float maxRandforce, startRollingForce;
    float forceX, forceY, forceZ;
    public string diceFaceNumb;
    public bool isLanded = false;
    public bool firstThrow = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        pos = transform.position;
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null)
        {
            if(Input.GetMouseButtonDown(0) && isLanded || Input.GetMouseButtonDown(0) && !firstThrow)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                    {
                        if (!firstThrow)
                        {
                            firstThrow = true;
                        }
                        RollDice();
                    }
                }
                
            }
        }
    }

    private void RollDice()
    {
        rb.isKinematic = false;
        forceX = Random.Range(0, maxRandforce);
        forceY = Random.Range(0, maxRandforce);
        forceZ = Random.Range(0, maxRandforce);
        rb.AddForce(Vector3.up * Random.Range(800, startRollingForce));
        rb.AddTorque(forceX, forceY, forceZ);
    }

    public void ResetDice()
    {
        rb.isKinematic = true;
        firstThrow = false;
        isLanded = false;
        transform.position = pos;
    }
}
