using UnityEngine;

namespace Items
{
    public class KnifeFactory : ObjectPool<Knife>
    {
        public Knife GetKnife()
        {
            var knife = GetObject();
            
            return knife;
        }

        public void ReturnKnife(Knife knife)
        {
            ReturnObject(knife);
            Transform transform1;
            (transform1 = knife.transform).SetParent(transform);
            transform1.rotation = Quaternion.identity;
            transform1.position = transform.position;
        }
    }
}