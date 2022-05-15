using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float radius = 5f; // No more set up by combatdata, to correct
    [SerializeField] float range = 5f;
    [SerializeField] Vector3 startPosition = Vector3.zero;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask willDestroyLayer;
    [SerializeField] CombatData data;
    [SerializeField] Transform visual;
    [SerializeField] SphereCollider collider;

    public float Speed { get => speed; set => speed = value; }
    public float Radius { get => radius; 
        set
        {
            radius = value;
            visual.localScale = radius * Vector3.right + radius * Vector3.up + radius * Vector3.forward;
            collider.radius = radius;
        }
    }
    public float Range { get => range; set => range = value; }
    public LayerMask EnemyLayer { get => enemyLayer; set => enemyLayer = value; }
    public LayerMask WillDestroyLayer { get => willDestroyLayer; set => willDestroyLayer = value; }
    public CombatData Data { get => data; set => data = value; }

    private void Awake()
    {
        startPosition = transform.position;
    }
    // Start is called before the first frame update
    void Update()
    {
        if ((transform.position - startPosition).magnitude >= range) Destroy(gameObject);

        transform.position += transform.forward * speed * Time.deltaTime;

        var cols = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        var touchedEnnemy = new List<Hitable>();
        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                Hitable minion = col.gameObject.GetComponent<Hitable>();
                if (!touchedEnnemy.Contains(minion) && minion != null)
                {
                    Debug.Log("Minion touched by projectile : " + minion);
                    touchedEnnemy.Add(minion);
                }
            }

            var distance = Mathf.Infinity;
            var closestEnnemyId = 0;
            for (int i = 0; i < touchedEnnemy.Count; i++)
            {
                var checkedDistance = (touchedEnnemy[i].transform.position - transform.position).sqrMagnitude;
                if (checkedDistance < distance) closestEnnemyId = i;
            }
            if (touchedEnnemy[closestEnnemyId]) data.HitTarget(touchedEnnemy[closestEnnemyId], this.transform.position);
            Destroy(gameObject);
        }


        cols = Physics.OverlapSphere(transform.position, radius, WillDestroyLayer);
        if(cols.Length != 0)
            Destroy(gameObject);


    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius * 0.5f);
    }
}
