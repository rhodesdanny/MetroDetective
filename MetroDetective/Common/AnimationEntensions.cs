using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace MetroDetective.Common
{
    public static class AnimationEntensions
    {

        public static T AnimateProperty<T>(this FrameworkElement control, string path) where T : Timeline, new()
        {

            var animation = new T();

            Storyboard.SetTarget(animation, control);

            Storyboard.SetTargetProperty(animation, path);

            return animation;

        }


        public static DoubleAnimationUsingKeyFrames AddEasingKeyFrame(

            this DoubleAnimationUsingKeyFrames animation,

            double seconds, double value,

            EasingFunctionBase easingFunction = null)
        {

            var keyFrame = new EasingDoubleKeyFrame

            {

                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(seconds)),

                Value = value,

                EasingFunction = easingFunction

            };

            animation.KeyFrames.Add(keyFrame);

            return animation;

        }


        public static ColorAnimation AnimateColorProperty(this FrameworkElement control, string path)
        {

            var animation = new ColorAnimation();

            Storyboard.SetTarget(animation, control);

            Storyboard.SetTargetProperty(animation, path);

            return animation;

        }

    }
}
