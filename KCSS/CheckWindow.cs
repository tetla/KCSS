using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace KCSS
{
    class CheckWindow
    {
        private Bitmap tmpBmp;
        private Bitmap scrBmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        public Point KanCollePosition()
        {
            var i = 0;
            string[] files = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + @"\" + "check", "*bmp");
            foreach (var item in files)
            {
                var Wp = CheckPosition(item);
                if (Wp.X != 0 || Wp.Y != 0)
                {

                    string[] stArrayData = item.Split('\\');
                    string[] hoge = stArrayData[stArrayData.Length - 1].Split('.');
                    string[] hoge2 = hoge[0].Split('-');

                    Wp.X -= int.Parse(hoge2[0]);
                    Wp.Y -= int.Parse(hoge2[1]);
                    return Wp;
                }
                i++;
            }
            return new Point(0, 0);
        }

        private Point CheckPosition(string ofd)
        {
            //画像を開く

            Bitmap bmp = new Bitmap(ofd);
            tmpBmp = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(tmpBmp))
            {
                g.DrawImage(bmp, 0, 0);
            }

            //スクリーンキャプチャ
            using (Graphics g = Graphics.FromImage(scrBmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, scrBmp.Size);
            }

            //テンプレート画像の検出
            Rectangle rect = ChkImg(scrBmp, tmpBmp);

            return new Point(rect.X, rect.Y);

        }

        //画像位置の検索
        private Rectangle ChkImg(Bitmap scrBmp, Bitmap tmpBmp)
        {
            IplImage tmpImg = OpenCvSharp.Extensions.BitmapConverter.ToIplImage(tmpBmp);
            IplImage scrImg = OpenCvSharp.Extensions.BitmapConverter.ToIplImage(scrBmp);

            CvSize resSize = new CvSize(scrImg.Width - tmpImg.Width + 1, scrImg.Height - tmpImg.Height + 1);
            IplImage resImg = Cv.CreateImage(resSize, BitDepth.F32, 1);

            Cv.MatchTemplate(scrImg, tmpImg, resImg, MatchTemplateMethod.CCorrNormed);

            double minVal;
            double maxVal;
            CvPoint minLoc;
            CvPoint maxLoc;

            Cv.MinMaxLoc(resImg, out minVal, out maxVal, out minLoc, out maxLoc);

            if ((maxVal >= 1))
            {
                return new Rectangle(maxLoc.X, maxLoc.Y, tmpImg.Width, tmpImg.Height);
            }

            return new Rectangle(0, 0, 0, 0);
        }
    }
}
