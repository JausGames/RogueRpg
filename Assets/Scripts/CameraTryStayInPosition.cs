using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTryStayInPosition : MonoBehaviour
{
    [SerializeField] bool goBackInPosition = true;
    [SerializeField] LayerMask mask = 1;
    [SerializeField] Vector3 defaultPosition;
    [SerializeField] Vector3 debugPos;
    [SerializeField] Rigidbody body;
    [SerializeField] Transform canISeePlayer;
    [SerializeField] float goBackStartTime;
    [SerializeField] float maxGoBackTime = .3f;
    private void Awake()
    {
        defaultPosition = transform.localPosition;
    }
    private void OnCollisionEnter(Collision collision)
    {
        goBackInPosition = false;
    }
    private void OnCollisionExit(Collision collision)
    {
        goBackInPosition = true;
        body.velocity = Vector3.zero;
        goBackStartTime = Time.time;
    }
    private void Update()
    {
        /*debugPos = (transform.position - transform.parent.position);
        if (goBackInPosition == true && (transform.position - transform.parent.position) != defaultPosition)
            Vector3.Lerp(transform.position, transform.parent.position + defaultPosition, 0.1f);*/
        var ray = new Ray(transform.position, canISeePlayer.position - transform.position);
        RaycastHit hit;
        var playerHidden = Physics.Raycast(ray, (transform.position - canISeePlayer.position).magnitude, mask);
        Debug.DrawRay(ray.origin, ray.direction.normalized * (transform.position - canISeePlayer.position).magnitude, Color.red);

        Debug.Log("CameraTryStayInPosition, Update : playerHidden = " + playerHidden);
        var updateDefaultPos = defaultPosition;
        if (playerHidden)
        {
            updateDefaultPos = defaultPosition + Vector3.forward;
        }

        if (goBackInPosition && (transform.position - transform.parent.position) != updateDefaultPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, updateDefaultPos, Mathf.Max((Time.time - goBackStartTime) * (1 / maxGoBackTime), 1f));
        }
    }
}
