using UnityEngine;

namespace Items
{
    public class ObstacleKnifeFactory : ObjectPool<Knife>
    {
        public Knife GetKnife()
        {
            var knife = GetObject();
            return knife;
        }

        public void ReturnKnife(Knife knife)
        {
            ReturnObject(knife);
            var knifeTransform = knife.transform;
            knifeTransform.localRotation = Quaternion.identity;
            knifeTransform.SetParent(transform);
            knifeTransform.position = transform.position;
            knife.Dispose();
        }
    }
}