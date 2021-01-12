using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OutlineEffect
{
    [Serializable]
    public class OutlineComponent 
    {
        public MeshFilter meshFilter;
        public Renderer renderer;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public SpriteRenderer spriteRenderer;

        private Material[] _SharedMaterials;

        public GameObject gameObject;

        public bool isValid;


        public OutlineComponent(GameObject obj)
        {
            renderer = obj.GetComponent<Renderer>();
            skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            spriteRenderer = obj.GetComponent<SpriteRenderer>();
            meshFilter = obj. GetComponent<MeshFilter>();
            gameObject = obj;

            isValid = true;
            if (renderer == null && skinnedMeshRenderer == null && spriteRenderer == null && meshFilter == null)
            {
                isValid = false;
            }
            if (renderer!=null&&renderer.enabled==false)
            {
                isValid = false;
            }
          
        }



        public Material[] SharedMaterials
        {
            get
            {
                if (_SharedMaterials == null)
                    _SharedMaterials = renderer.sharedMaterials;

                return _SharedMaterials;
            }
        }

    }
}