using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mercadeiro : MonoBehaviour {
    public enum Resources { a, b, c, d, e, f, g, h};
    public int idleAction;
   
    public Resources typeOfResourceProduced;

    private Animator animator;
    private DNA _dna;
    public int[] dna { get { return _dna.dna; } set { _dna.dna = value; } }
    private float[] _chemicals = new float[3];
    public float[] chemicals { get { return _chemicals; } set { _chemicals = value; } }
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    Crowd.GajoCitizen _client;

    void Awake () {
        //Controls what type of animation does he play
        animator = gameObject.GetComponent<Animator>();
        animator.SetInteger("Num_Mercadeiro", idleAction);
        originalRotation = gameObject.transform.rotation;
        originalPosition = gameObject.transform.position;
        //Type of resources being produced by this guy
        gameObject.AddComponent<DNA>();
        _dna= gameObject.GetComponent<DNA>();
        for (int i = 0; i < _dna.dna.Length; i++)
            _dna.dna[i] = 0;
    }

    void Start() {       
        switch (typeOfResourceProduced)
        {   case Resources.a: _dna.dna[10] = 0; _dna.dna[11] = 0; _dna.dna[12] = 0; break;
            case Resources.b: _dna.dna[10] = 0; _dna.dna[11] = 0; _dna.dna[12] = 1; break;
            case Resources.c: _dna.dna[10] = 0; _dna.dna[11] = 1; _dna.dna[12] = 0; break;
            case Resources.d: _dna.dna[10] = 0; _dna.dna[11] = 1; _dna.dna[12] = 1; break;
            case Resources.e: _dna.dna[10] = 1; _dna.dna[11] = 0; _dna.dna[12] = 0; break;
            case Resources.f: _dna.dna[10] = 1; _dna.dna[11] = 0; _dna.dna[12] = 1; break;
            case Resources.g: _dna.dna[10] = 1; _dna.dna[11] = 1; _dna.dna[12] = 0; break;
        }
       
        StartCoroutine("beginFountainOfResources");
    }

    IEnumerator beginFountainOfResources()
    {
        chemicals[0] += 100;
        chemicals[1] += 100;
        chemicals[2] += 100;
        yield return new WaitForSeconds(100f);
    }
   
    public void startInteraction(Crowd.GajoCitizen gajo)
    {
      _client = gajo;
gameObject.transform.LookAt(_client.transform.position);
        StartCoroutine("interactionAnimation");
gameObject.transform.LookAt(_client.transform.position);
     }

    IEnumerator interactionAnimation()
    {
      animator.Rebind();
      animator.SetInteger("Num_Mercadeiro", 0);      
      yield return new WaitForSeconds(1f); 
      endInteraction();
    }

    public void endInteraction()
    {
        animator.SetInteger("Num_Mercadeiro", idleAction);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        if(_client!=null)
            if (_client.behavior!=null)
                _client.behavior.Interacao.isInteracting = false;
        _client = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if(_client!=null)
            Gizmos.DrawWireSphere(transform.position, 2f);
        if (_client != null)
            if (_client.behavior != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _client.gameObject.transform.position);
            }
    }

}//eo class
