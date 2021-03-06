﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : BaseObject
{
    Vector3 prevDirToContact;
    Vector3 contactPoint; //every bullet knows the end point of its trajectory
    Vector3 shootDir;

    // Use this for initialization
    protected override void Start ()
    {
		
	}

    public void Fire(Ray shootRay, float force)
    {
        RaycastHit[] hits = Physics.RaycastAll(shootRay);

        //create a raycast in the direction
        //we are always guaranteed to make contact with a surface in the level

        RaycastHit closestHit = new RaycastHit();  //allow us to track the closest hit object to the ray
        float shortestDistance = Mathf.Infinity;

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
                    {
                        float distanceToHit = Vector2.Distance(transform.position, hit.point);
                        if(distanceToHit < shortestDistance)
                        {
                            closestHit = hit;
                            shortestDistance = distanceToHit;
                        }
                    }
                }
            }
        }

        if (closestHit.collider != null)
        {
            //Debug.Log("RAY CAST HIT [" + closestHit.collider.gameObject.name + "]");
            contactPoint = closestHit.point;
            Vector3 bulletVelocity = shootRay.direction.normalized;
            prevDirToContact = (transform.position - contactPoint).normalized;
            _rigidBody.velocity = bulletVelocity * force;

            //damage if enemy
            if(closestHit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                BaseEnemy enemy = closestHit.collider.gameObject.GetComponent<BaseEnemy>();
                enemy.ChangeHealth(-Main.GetInstance().currentPlayer.weapon.WEAPON_DAMAGE);
                enemy._aggro = true;    //now the player has caught enemy attention
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 dirToContact = (transform.position - contactPoint).normalized;
        Vector3 shootDir = _rigidBody.velocity.normalized;

        //if any of the direction axis flipped from positive to negative or vice versa, we have moved past our contact point
        if(dirToContact.x > 0 && prevDirToContact.x < 0 || dirToContact.x < 0 && prevDirToContact.x > 0
            || dirToContact.z > 0 && prevDirToContact.z < 0 || dirToContact.z < 0 && prevDirToContact.z > 0)
        {
            Kill();
        }

        prevDirToContact = dirToContact;
    }

   
}
