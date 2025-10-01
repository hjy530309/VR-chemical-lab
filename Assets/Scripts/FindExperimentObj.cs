using UnityEngine;

public static class FindExperimentObj
{
    public static Transform FindDeepChild(this Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = child.FindDeepChild(childName);
            if (result != null)
                return result;
        }
        return null;
    }
}
