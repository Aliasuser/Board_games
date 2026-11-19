using UnityEngine;

public class JiggleScript : MonoBehaviour
{

    [Range(0, 1)]
    public float power = 0.1f;

    [Header("Position Jiggler")]
    public bool jigPosition = true;
    public Vector3 positionJigAmount;

    [Range(0, 120)]
    public float positionFrequency = 10f;
    float positionTime;


    [Header("Rotation Jiggler")]
    public bool jigRotation = true;
    public Vector3 rotationJigAmount;
    [Range(0, 120)]
    public float rotationFrequency;
    float rotationTime;

    [Header("Scale Jiggler")]
    public bool jigScale = false;
    public Vector3 scaleJigAmount;
    [Range(0, 120)]
    public float scaleFrequency = 10f;
    float scaleTime;

    Vector3 basePosition;
    Quaternion baseRotation;
    Vector3 baseScale;
    void Start()
    {
        basePosition = transform.localPosition;
        baseRotation = transform.localRotation;
        baseScale = transform.localScale;
    }

    void Update()
    {
        var dt = Time.unscaledDeltaTime;
        positionTime += dt * positionFrequency;

        if (jigPosition)
        {
            transform.localPosition = basePosition + positionJigAmount * Mathf.Sin(positionTime) * power;
        }

        rotationTime += dt * rotationFrequency;
        if (jigRotation)
        {
            transform.localRotation = baseRotation * Quaternion.Euler(rotationJigAmount * Mathf.Sin(rotationTime) * power);
        }
        scaleTime += dt * scaleFrequency;


        if (jigScale)
        {
            transform.localScale = baseScale + scaleJigAmount * Mathf.Sin(scaleTime) * power;
        }
    }
}
