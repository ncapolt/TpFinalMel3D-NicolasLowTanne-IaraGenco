using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryBotScript : MonoBehaviour
{
    public Transform[] waypoints; 
    public float speed = 3.0f;    
    public float stoppingDistance = 0.5f; 

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return; 

       
        Transform targetWaypoint = waypoints[currentWaypointIndex];

       
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

       
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; 
        }

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
        }
    }

} 