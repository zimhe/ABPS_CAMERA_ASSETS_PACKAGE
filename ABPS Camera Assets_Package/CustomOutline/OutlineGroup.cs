using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OutlineEffect
{
    public class OutlineGroup : MonoBehaviour
    {
        public List<OutlineComponent> outlineComponents;

        public bool eraseRenderer;
        public int colorID = 0;

        private void Awake()
        {
            outlineComponents = new List<OutlineComponent>();
            
            var children=gameObject.GetComponentsInChildren<Transform>();

            foreach(var child in children)
            {
                var oc = new OutlineComponent(child.gameObject);
                if (oc.isValid)
                {
                    outlineComponents.Add(new OutlineComponent(child.gameObject));
                }
            }
        }

        // Start is called before the first frame update
      

        void OnEnable()
        {
            OutlineEffectLayer.Instance?.AddOutline(this);
        }

        void OnDisable()
        {
            OutlineEffectLayer.Instance?.RemoveOutline(this);
        }
    }
}