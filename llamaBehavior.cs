using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class llamaBehavior : MonoBehaviour
{
    public Animator anim;
    Vector3 herdCenter = new Vector3(500, 0, 100);
    Vector3 randPos;
    Vector3 waterpos = new Vector3(542, 0, 233);
    bool isDrinking = false;
    private void Start()
    {
        //Adjust positions to be on top of terrain
        herdCenter.y = Terrain.activeTerrain.SampleHeight(herdCenter);
        transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position), transform.position.z);
        randPos = herdCenter;
        waterpos.y = Terrain.activeTerrain.SampleHeight(herdCenter);
    }

    private void Update()
    {
        if (!isDrinking)
        {
            //Play walk animation
            if (anim.GetBool("isWalking"))
            {
                transform.Translate(0, 0, 1f * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, transform.position.z);
            }           
            if (Random.Range(1, 15000) < 100 && !anim.GetBool("isGrazing"))
            {
                //Walk
                anim.Play("walk");
                anim.SetBool("isWalking", true);
            }
            else if (Random.Range(1, 20000) < 50 && !anim.GetBool("isWalking"))
            {
                //Graze
                anim.Play("graze");
                anim.SetBool("isGrazing", true);
            }
            else if (Random.Range(1, 100) < 3)
            {
                //Group
                Group();
            }
            else if (Random.Range(1, 10000) < 2)
            {
                //Drink Water
                isDrinking = true;
                StartCoroutine(DrinkWater());
            }
            //Generate random position within herd
            else if (Random.Range(1, 20000) < 50)
            {
                randPos = new Vector3(Random.Range(450, 550), Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, Random.Range(50, 150));
            }
        }
    }

    private void Group()
    {
        //Turn back if too far away from herd
        if (Vector3.Distance(transform.position, herdCenter) > 25f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(herdCenter - transform.position), 0.1f);
            transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, transform.position.z);
            return;
        }
        //Rotate towards randpos
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(randPos - transform.position), 0.1f);
    }

    //Coroutine to drink water
    IEnumerator DrinkWater() {
        //Turn towards waterpos
        Vector3 currentPos = transform.position;
        float elapsedTime = 0f;
        float waitTime = 10f;
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(waterpos - transform.position), elapsedTime / waitTime * Time.deltaTime);
            //Debug.Log(elapsedTime + " " + waitTime);
            yield return null;
        }
        //Move towards waterpos
        while (transform.position.x < waterpos.x && transform.position.z < waterpos.z)
        {
            if (!anim.GetBool("isWalking"))
            {
                anim.Play("walk");
                anim.SetBool("isWalking", true);
            }
            transform.Translate(0, 0, 1f * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, transform.position.z);
            yield return null;
        }
        //Drink
        anim.SetBool("isGrazing", true);
        anim.Play("graze");
        while (anim.GetBool("isGrazing"))
        {
            yield return null;
        }
        //Rotate towards herd
        elapsedTime = 0f;
        waitTime = 10f;
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentPos - transform.position), elapsedTime / waitTime * Time.deltaTime);
            //Debug.Log(elapsedTime + " " + waitTime);
            yield return null;
        }
        //Move towards herd
        while (transform.position.x > currentPos.x && transform.position.z > currentPos.z)
        {
            if (!anim.GetBool("isWalking"))
            {
                anim.Play("walk");
                anim.SetBool("isWalking", true);
            }
            transform.Translate(0, 0, 1f * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, transform.position.z);
            yield return null;
        }
        //Get ready to do normal herd stuff again
        isDrinking = false;
        yield return null;
    }

    void AnimEnded() {
        anim.SetBool("isWalking", false);
        anim.SetBool("isGrazing", false);
    }
}
