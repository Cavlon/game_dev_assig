using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CavlonUtils {

    public static class ListUtils {

        // From https://stackoverflow.com/questions/24644846/random-shuffle-listing-in-unity-3d
        public static List<T> Shuffle<T>(List<T> _list)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                T temp = _list[i];
                int randomIndex = UnityEngine.Random.Range(i, _list.Count);
                _list[i] = _list[randomIndex];
                _list[randomIndex] = temp;
            }

            return _list;
        }
    }
    public static class AnimUtils {

        public delegate float EasingFunction(float x);

        public static float Identity(float x) {
            return x;
        }

        // From https://nicmulvaney.com/easing?ref=blog.febucci.com#easeInOutElastic
        public static float ElasticInOut(float x) {
            const float c5 = 2 * (float)Math.PI / 4.5f;
            if (x == 0) return 0;
            else if (x == 1) return 1;
            else if (x < 0.5f) return (float)(-(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2);
            else return (float)(Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5) / 2 + 1);
        }

        // From https://nicmulvaney.com/easing?ref=blog.febucci.com#easeOutCubic
        public static float CubicIn(float x) {
            return x * x * x;
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
                pos = Vector2.Lerp(pos, targetPos, easingFunction(elapsed_time / duration)); //Changes and interpolates the position's "y" value
                transform.localPosition = pos;//Changes the object's position
                yield return 0;
            } while (elapsed_time <= duration); //Inside the loop until the time expires
        }

        public static IEnumerator TweenScale(Transform transform, Vector3 targetScale, float duration, EasingFunction easingFunction)
        {
            float elapsed_time = 0; //Elapsed time
            Vector3 scale = transform.localScale; //Start object's position
            do 
            {
                elapsed_time += Time.deltaTime; //Adds to the elapsed time the amount of time needed to skip/wait one frame
                scale = Vector3.Lerp(scale, targetScale, easingFunction(elapsed_time / duration)); //Changes and interpolates the position's "y" value
                transform.localScale = scale;//Changes the object's position
                yield return 0;
            } while (elapsed_time <= duration); //Inside the loop until the time expires
        }

        public static IEnumerator TweenRotZ(Transform transform, float targetAngle, float duration, EasingFunction easingFunction)
        {
            float elapsed_time = 0; //Elapsed time
            float angleZ = transform.eulerAngles.z; //Start object's position
            do 
            {
                elapsed_time += Time.deltaTime; //Adds to the elapsed time the amount of time needed to skip/wait one frame
                angleZ = Mathf.LerpAngle(angleZ, targetAngle, easingFunction(elapsed_time / duration)); //Changes and interpolates the position's "y" value
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleZ);//Changes the object's position
                yield return 0;
            } while (elapsed_time <= duration); //Inside the loop until the time expires
        }
    }
}
