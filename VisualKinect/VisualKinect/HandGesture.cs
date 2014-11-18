using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Text;

namespace VisualKinect
{
    class HandGesture
    {
        static public bool buttonsHidden { get; set; }
        static Boolean handscrossed = false;

        // Boolean for switching music genre
        static Boolean[] claps = new Boolean[4];
        static Boolean switchGenre = false;
        static Boolean matrixMode = false;
        static Boolean[] swipeParts = new Boolean[3];
        static Boolean pauseGame = false;
        static Boolean intersected = false;

        public static Boolean PauseGame()
        {
            Vector2 right = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandRight);
            Vector2 left = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandLeft);
            Vector2 rightKnee = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.KneeRight);
            Vector2 leftKnee = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.KneeLeft);


            Rectangle leftHand = new Rectangle((int)left.X, (int)left.Y, 1, 1);
            Rectangle rightHand = new Rectangle((int)right.X, (int)right.Y, 1, 1);
            int tol = 80;

            Rectangle triggerZone = new Rectangle((int)(leftKnee.X) - tol, (int)(leftKnee.Y) - tol, Math.Abs((int)(rightKnee.X - leftKnee.X)) + 160, 600);
            Vector2 tmp = new Vector2(0, 0);
            if (right == tmp)
                return false;
            if ((triggerZone.Intersects(leftHand) && triggerZone.Intersects(rightHand)) && !intersected)
            {
                pauseGame = !pauseGame;
                intersected = true;
                Console.WriteLine("Pause: " + pauseGame);
            }
            else if ((!triggerZone.Intersects(leftHand) || !triggerZone.Intersects(rightHand)) && intersected)
            {
                intersected = false;
            }
            return pauseGame;
        }

        public static Boolean HideButtons()
        {
            Vector2 right = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandRight);
            Vector2 left = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandLeft);
            Vector2 hip = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HipRight);
            int tolerance = 20;
            Boolean isHidden = false;
            // Console.WriteLine("buttonshidden: " + buttonsHidden);

            if (!handscrossed && right.X < left.X)
            {
                if (right.Y > hip.Y - tolerance && right.Y < hip.Y + tolerance && right.Y > hip.Y - tolerance && right.Y < hip.Y + tolerance)
                {
                    handscrossed = true;
                    //Console.WriteLine(">> 1. if >> Handscrossed? " + handscrossed + " buttonshidden? " + buttonsHidden);
                }
            }

            else if (!buttonsHidden && handscrossed && right.X > left.X)
            {
                handscrossed = false;
                buttonsHidden = true;
                isHidden = true;
                //Console.WriteLine(">> 2. if >> Handscrossed? " + handscrossed + " buttonshidden? " + buttonsHidden);
            }
            else if (handscrossed && buttonsHidden && right.X > left.X)
            {
                buttonsHidden = false;
                handscrossed = false;
                //Console.WriteLine(">> 3. if >> Handscrossed? " + handscrossed + " buttonshidden? " + buttonsHidden);
            }
            return isHidden;
        }

        public static Boolean switchModi()
        {
            Vector2 right = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandRight);
            Vector2 left = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandLeft);
            Vector2 hip = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HipRight);

            Vector2 distance = right - left;

            float distX = Math.Abs(right.X - left.X);
            float distY = Math.Abs(right.Y - left.Y);
            float tol = 10;

            switchGenre = false;
            if (!claps[0] && distX >= 30)
            {
                claps[0] = true;
            }

            if (claps[0] && !claps[1] && distX <= tol && distY <= tol && right.X != 0 && right.Y != 0 && left.X != 0 && left.Y != 0)
            {
                claps[1] = true;
            }

            if (claps[1] && !claps[2] && !claps[3])
            {
                processFirstClap();
            }
            else if (claps[2] && !claps[3])
            {
                waitForSecondClap();
            }
            else if (claps[3])
            {
                switchGenre = true;
                fireNewGenre();
            }
            return switchGenre;
        }

        private static void processFirstClap()
        {
            Vector2 right = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandRight);
            Vector2 left = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandLeft);
            Vector2 hip = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HipRight);

            Vector2 distance = right - left;

            float distX = Math.Abs(right.X - left.X);
            float distY = Math.Abs(right.Y - left.Y);

            if (distX >= 20)
            {
                claps[2] = true;
                //Console.WriteLine("spread arms after first Clap. Distance: " + distX + " ,Array: " + claps.ToString());
            }
        }

        private static void waitForSecondClap()
        {
            Vector2 right = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandRight);
            Vector2 left = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HandLeft);
            Vector2 hip = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HipRight);

            Vector2 distance = right - left;

            float distX = Math.Abs(right.X - left.X);
            float distY = Math.Abs(right.Y - left.Y);

            if (distX <= 10 && distY <= 10)
            {
                claps[3] = true;
                //Console.WriteLine("Second Clap" + distX + " ,Array: " + claps.ToString());
            }
        }

        private static void fireNewGenre()
        {
            for (int i = 0; i < claps.Length; i++)
            {
                claps[i] = false;
            }
            // Console.WriteLine("Fire new Genre***************************************************************************");
        }

        //****************************************************************************************************************
        public static Boolean SwitchGenre()
        {
            Vector2 right = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.ElbowRight);
            Vector2 left = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.ElbowLeft);
            Vector2 hipCenter = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HipCenter);
            Vector2 hipLeft = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.HipLeft);
            Vector2 head = SkeletonData.GetVector2Absolute(Microsoft.Kinect.JointType.Head);

            float distCenter = Math.Abs(right.X - hipCenter.X);
            bool handAtHip = false;
            float tol = 20;
            if (Math.Abs(left.X - hipLeft.X) <= 100 && Math.Abs(left.Y - hipLeft.Y) <= 70 && Math.Abs(hipCenter.Y - right.Y) >= 60)
            {
                handAtHip = true;
            }
            //Console.WriteLine("left x:" + Math.Abs(left.X - hipLeft.X) + ", left Y:" + Math.Abs(left.Y - hipLeft.Y) + " right hand  y: " + Math.Abs(hipCenter.Y - left.Y));
            // Console.WriteLine("befor" + distCenter);
            if (!swipeParts[0] && handAtHip && distCenter >= 140)
            {
                //Console.WriteLine("start" + distCenter);
                swipeParts[0] = true;
                return false;
            }
            if (swipeParts[0] && !swipeParts[1] && handAtHip && distCenter <= 10)
            {
                //Console.WriteLine("middle" + distCenter);
                swipeParts[1] = true;
                return false;
            }
            if (swipeParts[0] && swipeParts[1] && !swipeParts[2] && handAtHip && distCenter >= 140)
            {
                swipeParts[2] = true;
                matrixMode = true;
                fireSwipe();
                return true;
            }
            else
            {
                return false;
            }

        }

        private static void fireSwipe()
        {
            //Console.WriteLine("Swiped");
            for (int i = 0; i < swipeParts.Length; i++)
            {
                swipeParts[i] = false;
            }
        }
    }
}
