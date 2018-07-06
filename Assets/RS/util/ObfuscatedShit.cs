using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS
{
    public static class ObfuscatedShit
    {
        public static float[] method2424(int i, int i_547_, int i_548_, int i_549_, float f, float f_550_, float f_551_)
        {
            float[] fs = new float[9];
            float f_552_ = 1.0F;
            float f_553_ = 0.0F;
            float f_554_ = i_547_ / 32767.0F;
            float f_555_ = -(float)Math.Sqrt(1.0F - f_554_ * f_554_);
            float f_556_ = 1.0F - f_554_;
            float f_557_ = (float)Math.Sqrt(i * i + i_548_ * i_548_);
            if (f_557_ != 0.0F)
            {
                f_552_ = -i_548_ / f_557_;
                f_553_ = i / f_557_;
            }
            fs[0] = f_554_ + f_552_ * f_552_ * f_556_;
            fs[1] = f_553_ * f_555_;
            fs[2] = f_553_ * f_552_ * f_556_;
            fs[3] = -f_553_ * f_555_;
            fs[4] = f_554_;
            fs[5] = f_552_ * f_555_;
            fs[6] = f_552_ * f_553_ * f_556_;
            fs[7] = -f_552_ * f_555_;
            fs[8] = f_554_ + f_553_ * f_553_ * f_556_;
            float[] fs_558_ = new float[9];
            f_554_ = (float)Math.Cos(i_549_ * 0.024543693F);
            f_555_ = (float)Math.Sin(i_549_ * 0.024543693F);
            f_556_ = 1.0F - f_554_;
            fs_558_[0] = f_554_;
            fs_558_[1] = 0.0F;
            fs_558_[2] = f_555_;
            fs_558_[3] = 0.0F;
            fs_558_[4] = 1.0F;
            fs_558_[5] = 0.0F;
            fs_558_[6] = -f_555_;
            fs_558_[7] = 0.0F;
            fs_558_[8] = f_554_;
            float[] fs_559_ = new float[9];
            fs_559_[0] = fs_558_[0] * fs[0] + fs_558_[1] * fs[3] + fs_558_[2] * fs[6];
            fs_559_[1] = fs_558_[0] * fs[1] + fs_558_[1] * fs[4] + fs_558_[2] * fs[7];
            fs_559_[2] = fs_558_[0] * fs[2] + fs_558_[1] * fs[5] + fs_558_[2] * fs[8];
            fs_559_[3] = fs_558_[3] * fs[0] + fs_558_[4] * fs[3] + fs_558_[5] * fs[6];
            fs_559_[4] = fs_558_[3] * fs[1] + fs_558_[4] * fs[4] + fs_558_[5] * fs[7];
            fs_559_[5] = fs_558_[3] * fs[2] + fs_558_[4] * fs[5] + fs_558_[5] * fs[8];
            fs_559_[6] = fs_558_[6] * fs[0] + fs_558_[7] * fs[3] + fs_558_[8] * fs[6];
            fs_559_[7] = fs_558_[6] * fs[1] + fs_558_[7] * fs[4] + fs_558_[8] * fs[7];
            fs_559_[8] = fs_558_[6] * fs[2] + fs_558_[7] * fs[5] + fs_558_[8] * fs[8];
            fs_559_[0] *= f;
            fs_559_[1] *= f;
            fs_559_[2] *= f;
            fs_559_[3] *= f_550_;
            fs_559_[4] *= f_550_;
            fs_559_[5] *= f_550_;
            fs_559_[6] *= f_551_;
            fs_559_[7] *= f_551_;
            fs_559_[8] *= f_551_;
            return fs_559_;
        }

        public static void method2416(int i, int i_214_, int i_215_, int i_216_, int i_217_, int i_218_, int i_219_, float[] fs, int i_220_, float f, float f_221_, float f_222_)
        {
            i -= i_216_;
            i_214_ -= i_217_;
            i_215_ -= i_218_;
            float f_223_ = i * fs[0] + i_214_ * fs[1] + i_215_ * fs[2];
            float f_224_ = i * fs[3] + i_214_ * fs[4] + i_215_ * fs[5];
            float f_225_ = i * fs[6] + i_214_ * fs[7] + i_215_ * fs[8];
            float f_226_;
            float f_227_;
            if (i_219_ == 0)
            {
                f_226_ = f_223_ + f + 0.5F;
                f_227_ = -f_225_ + f_222_ + 0.5F;
            }
            else if (i_219_ == 1)
            {
                f_226_ = f_223_ + f + 0.5F;
                f_227_ = f_225_ + f_222_ + 0.5F;
            }
            else if (i_219_ == 2)
            {
                f_226_ = -f_223_ + f + 0.5F;
                f_227_ = -f_224_ + f_221_ + 0.5F;
            }
            else if (i_219_ == 3)
            {
                f_226_ = f_223_ + f + 0.5F;
                f_227_ = -f_224_ + f_221_ + 0.5F;
            }
            else if (i_219_ == 4)
            {
                f_226_ = f_225_ + f_222_ + 0.5F;
                f_227_ = -f_224_ + f_221_ + 0.5F;
            }
            else
            {
                f_226_ = -f_225_ + f_222_ + 0.5F;
                f_227_ = -f_224_ + f_221_ + 0.5F;
            }
            if (i_220_ == 1)
            {
                float f_228_ = f_226_;
                f_226_ = -f_227_;
                f_227_ = f_228_;
            }
            else if (i_220_ == 2)
            {
                f_226_ = -f_226_;
                f_227_ = -f_227_;
            }
            else if (i_220_ == 3)
            {
                float f_229_ = f_226_;
                f_226_ = f_227_;
                f_227_ = -f_229_;
            }
            tmpUMapping1 = f_226_;
            tmpVMapping1 = f_227_;
        }

        public static float tmpUMapping1;
        public static float aFloat3907;
        public static float tmpVMapping1;
        public static float aFloat3899;
        public static float aFloat3903;
        public static float aFloat3902;

        public static int method2437(float f, float f_715_, float f_716_)
        {
            float f_717_ = f < 0.0F ? -f : f;
            float f_718_ = f_715_ < 0.0F ? -f_715_ : f_715_;
            float f_719_ = f_716_ < 0.0F ? -f_716_ : f_716_;
            if (f_718_ > f_717_ && f_718_ > f_719_)
            {
                if (f_715_ > 0.0F)
                {
                    return 0;
                }
                return 1;
            }
            if (f_719_ > f_717_ && f_719_ > f_718_)
            {
                if (f_716_ > 0.0F)
                {
                    return 2;
                }
                return 3;
            }
            if (f > 0.0F)
            {
                return 4;
            }
            return 5;
        }

        public static void method2434(int i, int i_676_, int i_677_, int i_678_, int i_679_, int i_680_, float[] fs, int i_681_, float f)
        {
            i -= i_678_;
            i_676_ -= i_679_;
            i_677_ -= i_680_;
            float f_682_ = i * fs[0] + i_676_ * fs[1] + i_677_ * fs[2];
            float f_683_ = i * fs[3] + i_676_ * fs[4] + i_677_ * fs[5];
            float f_684_ = i * fs[6] + i_676_ * fs[7] + i_677_ * fs[8];
            float f_685_ = (float)Math.Sqrt(f_682_ * f_682_ + f_683_ * f_683_ + f_684_ * f_684_);
            float f_686_ = (float)Math.Atan2(f_682_, f_684_) / 6.2831855F + 0.5F;
            float f_687_ = (float)Math.Asin(f_683_ / f_685_) / 3.1415927F + 0.5F + f;
            if (i_681_ == 1)
            {
                float f_688_ = f_686_;
                f_686_ = -f_687_;
                f_687_ = f_688_;
            }
            else if (i_681_ == 2)
            {
                f_686_ = -f_686_;
                f_687_ = -f_687_;
            }
            else if (i_681_ == 3)
            {
                float f_689_ = f_686_;
                f_686_ = f_687_;
                f_687_ = -f_689_;
            }
            aFloat3907 = f_686_;
            aFloat3902 = f_687_;
        }

        public static void method2431(int i, int i_644_, int i_645_, int i_646_, int i_647_, int i_648_, float[] fs, float f, int i_649_, float f_650_)
        {
            i -= i_646_;
            i_644_ -= i_647_;
            i_645_ -= i_648_;
            float f_651_ = i * fs[0] + i_644_ * fs[1] + i_645_ * fs[2];
            float f_652_ = i * fs[3] + i_644_ * fs[4] + i_645_ * fs[5];
            float f_653_ = i * fs[6] + i_644_ * fs[7] + i_645_ * fs[8];
            float f_654_ = (float)Math.Atan2(f_651_, f_653_) / 6.2831855F + 0.5F;
            if (f != 1.0F)
            {
                f_654_ *= f;
            }
            float f_655_ = f_652_ + 0.5F + f_650_;
            if (i_649_ == 1)
            {
                float f_656_ = f_654_;
                f_654_ = -f_655_;
                f_655_ = f_656_;
            }
            else if (i_649_ == 2)
            {
                f_654_ = -f_654_;
                f_655_ = -f_655_;
            }
            else if (i_649_ == 3)
            {
                float f_657_ = f_654_;
                f_654_ = f_655_;
                f_655_ = -f_657_;
            }
            aFloat3899 = f_654_;
            aFloat3903 = f_655_;
        }
    }
}
