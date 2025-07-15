using UnityEngine;

namespace utilities 
{
    public static class GameObjectExtensions
    {
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        public static void ReplaceAllMaterials(GameObject root, Material newMaterial)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = newMaterial;
                }
                renderer.materials = newMaterials;
            }
        }
    }

    public static class VectorExtension
    {
        public static Vector3 V2ToV3(Vector2 vector, float z = 0)
        { 
            return new Vector3(vector.x, vector.y, z);
        }
    }
}