using UnityEngine;
using System;
using System.Collections;

namespace CavlonUtils {
    public static class AnimUtils {

        public delegate float EasingFunction(float x);

        // From https://nicmulvaney.com/easing?ref=blog.febucci.com#easeInOutElastic
        public static float ElasticInOut(float x) {
            const float c5 = 2 * (float)Math.PI / 4.5f;
            if (x == 0) return 0;
            else if (x == 1) return 1;
            else if (x < 0.5f) return (float)(-(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2);
            else return (float)(Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5) / 2 + 1);
        }

        // From https://nicmulvaney.com/easing?ref=blog.febucci.com#easeOutCubic
        public static float CubicOut(float x) {
            return (float)(1 - Math.Pow(1 - x, 3));
        }

        // Edit of function from https://stackoverflow.com/questions/27119906/animate-move-translate-tween-image-in-unity-4-6-from-c-sharp-code
        public static IEnumerator TweenPos(Transform transform, Vector3 targetPos, float duration, EasingFunction easingFunction)
        {
            float elapsed_time = 0; //Elapsed time
            Vector3 pos = transform.localPosition; //Start object's position
            do 
            {
                elapsed_time += Time.deltaTime; //Adds to the elapsed time the amount of time needed to skip/wait one frame
                pos = Vector3.Lerp(pos, targetPos, easingFunction(elapsed_time / duration)); //Changes and interpolates the position's "y" value
                Debug.Log(pos);
                transform.localPosition = pos;//Changes the object's position
                yield return 0;
            } while (elapsed_time <= duration); //Inside the loop until the time expires
        }

        // Edit of function from https://stackoverflow.com/questions/27119906/animate-move-translate-tween-image-in-unity-4-6-from-c-sharp-code
        public static IEnumerator TweenPos(Transform transform, Vector2 targetPos, float duration, EasingFunction easingFunction)
        {
            float elapsed_time = 0; //Elapsed time
            Vector3 pos = transform.localPosition; //Start object's position
            do 
            {
                elapsed_time += Time.deltaTime; //Adds to the elapsed time the amount of time needed to skip/wait one frame
                pos = Vector3.Lerp(pos, targetPos, easingFunction(elapsed_time / duration)); //Changes and interpolates the position's "y" value
                transform.localPosition = pos;//Changes the object's position
                yield return 0;
            } while (elapsed_time <= duration); //Inside the loop until the time expires
        }
    }
}
