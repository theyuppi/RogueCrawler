using UnityEngine;

namespace Assets.Scripts
{
    public class SpikeScript : MonoBehaviour
    {
        private Animator[] childAnimators;
        private SpriteRenderer[] childSR;
        private int myDmg = 10;

        void Start()
        {
            childAnimators = GetComponentsInChildren<Animator>();
            childSR = GetComponentsInChildren<SpriteRenderer>();
        }

        void Update()
        {
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<PlayerScript>().stunned = true;
                StartCoroutine(other.GetComponent<PlayerScript>().GetHit(myDmg));
                childSR[1].sortingOrder = 4;
            }
            if (other.tag == "Enemy")
            {
                other.GetComponent<EnemyScript>().stunned = true;
                StartCoroutine(other.GetComponent<EnemyScript>().GetHit(myDmg));
                childSR[1].sortingOrder = 4;
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player" || other.tag == "Enemy")
            {
                foreach (Animator a in childAnimators)
                {
                    a.SetBool("isActive", true);
                }
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player" || other.tag == "Enemy")
            {
                foreach (Animator a in childAnimators)
                {
                    a.SetBool("isActive", false);
                }
                childSR[1].sortingOrder = 1;
            }
        }
    }
}