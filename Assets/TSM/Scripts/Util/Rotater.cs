using UnityEngine;
namespace TSMSampleUtil
{
    public class Rotater : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;
        private Transform thisTransform;

        private void Awake()
        {
            thisTransform = this.transform;
        }

        private void Update()
        {
            thisTransform.Rotate(Vector3.up * speed * Time.deltaTime);
        }

    }
}