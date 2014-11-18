using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualKinect
{
    class SkeletonData
    {
        static float skeletonScale = 1.4f;
        static Skeleton[] skeletonData = null;
        public static Skeleton skeleton { get; set; }
        static Vector2 dimension = new Vector2(1, 1);

        public static void calcSkeletonScale()
        {
            //todo find function to define your scale factor depending on height
            dimension.Y = dimension.Y * skeletonScale;
        }

        public static void ScreenScale(float width, float height)
        {
            dimension = new Vector2(width, height);
        }

        public static float GetX(JointType joint)
        {
            if (skeleton != null)
                return skeleton.Joints[joint].Position.X;
            return 0;
        }
        public static float GetY(JointType joint)
        {
            if (skeleton != null)
                return skeleton.Joints[joint].Position.Y;
            return 0;
        }
        public static float GetZ(JointType joint)
        {
            if (skeleton != null)
                return skeleton.Joints[joint].Position.Z;
            return 0;
        }

        public static Vector2 GetVector2Absolute(JointType joint)
        {
            if (skeleton != null)
            {
                float x = (0.5f * skeleton.Joints[joint].Position.X + 0.5f) * dimension.X;
                float y = (-0.5f * skeleton.Joints[joint].Position.Y + 0.5f) * dimension.Y;
                return new Vector2(x, y);
            }
            return new Vector2(0, 0);
        }

        public static Vector3 GetVector3Absolute(JointType joint)
        {
            if (skeleton != null)
            {
                float x = (0.5f * skeleton.Joints[joint].Position.X + 0.5f) * dimension.X;
                float y = (-0.5f * skeleton.Joints[joint].Position.Y + 0.5f) * dimension.Y;
                float z = skeleton.Joints[joint].Position.Z;
                return new Vector3(x, y, z);
            }
            return new Vector3(0, 0, 0);
        }

        public static void Update(SkeletonFrameReadyEventArgs skeletonFrames)
        {
            using (SkeletonFrame skeletonFrame = skeletonFrames.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                }
            }

            if (skeletonData != null)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeleton = skel;
                    }
                }
            }
        }
    }
}
