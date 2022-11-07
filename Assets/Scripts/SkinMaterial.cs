using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRunner
{
    public class SkinMaterial : MonoBehaviour
    {
        [SerializeField] private Material m_DefaultMaterial;
        [SerializeField] private Material m_ClashMaterial;
        [SerializeField] private SkinnedMeshRenderer m_MeshRenderer;

        public Material DefaultMaterial => m_DefaultMaterial;
        public Material ClashMaterial => m_ClashMaterial;

        public void SetMaterial(bool isInvul)
        {
            m_MeshRenderer.material = isInvul ? m_ClashMaterial : m_DefaultMaterial;
        }
    }
}
