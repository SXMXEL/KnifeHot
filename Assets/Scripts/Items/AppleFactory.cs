using UnityEngine;

namespace Items
{
    public class AppleFactory : ObjectPool<Apple>
    {
        public Apple GetApple()
        {
            var apple = GetObject();
            return apple;
        }

        public void ReturnApple(Apple apple)
        {
            ReturnObject(apple);
            var appleTransform = apple.transform;
            appleTransform.rotation = Quaternion.identity;
            appleTransform.SetParent(transform);
            appleTransform.position = transform.position;
            apple.Dispose();
        }
    }
}