using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyLibrary
{
    class Program
    {
        static void LockFile(string path,string password,string pp)
        {
            byte[] data = File.ReadAllBytes(path);
            string test = Convert.ToBase64String(data);
            string[] pd = path.Split('\\');
            test = pd[pd.Length - 1] + "?" + test;
            data = Encoding.UTF8.GetBytes(test);
            int w = (int)Math.Ceiling(Math.Sqrt(Math.Ceiling(data.Length / 4f)));
            Bitmap b = new Bitmap(w + 1, w + 1);
            Bitmap pass = (Bitmap)Bitmap.FromFile(path.Split('\\')[0] + "\\" + pp);
            int x = 1, y = 1; int ca, cr, cg, cb; int p = 0;
            byte[] pa = Encoding.UTF8.GetBytes(password);int pai = 0;
            Color dC;
            while (y < b.Height)
            {
                try { ca = data[p]; } catch { ca = 0; }
                try { cr = data[p + 1]; } catch { cr = 0; }
                try { cg = data[p + 2]; } catch { cg = 0; }
                try { cb = data[p + 3]; } catch { cb = 0; }
                dC = pass.GetPixel(x / b.Width * pass.Width, y / b.Height * pass.Height);
                ca = (int)((byte)ca ^ (byte)dC.A ^ pa[pai]);
                cr = (int)((byte)cr ^ (byte)dC.R ^ pa[pai]);
                cg = (int)((byte)cg ^ (byte)dC.G ^ pa[pai]);
                cb = (int)((byte)cb ^ (byte)dC.B ^ pa[pai]);
                b.SetPixel(x, y, Color.FromArgb(ca, cr, cg, cb));
                x++;
                pai++;
                if (pai >= pa.Length) pai = 0;
                if (x > w) { x = 1; y++; }
                p += 4;
            }
            string basepath = "";
            string[] temp = path.Split('\\');
            for (int i = 0; i < temp.Length - 1; i++)
                basepath += temp[i] + "\\";
            b.Save(basepath + Guid.NewGuid().ToString() + ".png");
            File.Delete(path);
            b.Dispose();
            Console.WriteLine("Locked!");
            pass.Dispose();
        }
        static void UnLockFile(string path, string password, string pp)
        {
            Bitmap b = (Bitmap)Bitmap.FromFile(path);
            Bitmap pass = (Bitmap)Bitmap.FromFile(path.Split('\\')[0] + "\\" + pp);
            int x = 1, y = 1; int ca, cr, cg, cb; int p = 0;
            byte[] pa = Encoding.UTF8.GetBytes(password); int pai = 0;
            Color dC;
            string test = "";
            byte[] data = new byte[(int)(Math.Ceiling(Math.Pow((b.Width - 1), 2) * 4))];
            p = 0;
            for (y = 1; y < b.Height; y++)
            {
                for (x = 1; x < b.Width; x++)
                {
                    Color c = b.GetPixel(x, y);
                    ca = c.A; cr = c.R; cg = c.G; cb = c.B;
                    dC = pass.GetPixel(x / b.Width * pass.Width, y / b.Height * pass.Height);
                    ca = (int)((byte)ca ^ (byte)dC.A ^ pa[pai]);
                    cr = (int)((byte)cr ^ (byte)dC.R ^ pa[pai]);
                    cg = (int)((byte)cg ^ (byte)dC.G ^ pa[pai]);
                    cb = (int)((byte)cb ^ (byte)dC.B ^ pa[pai]);
                    data[p] = (byte)ca; data[p + 1] = (byte)cr; data[p + 2] = (byte)cg; data[p + 3] = (byte)cb;
                    p += 4;
                    pai++;
                    if (pai >= pa.Length) pai = 0;
                }
            }
            test = Encoding.UTF8.GetString(data).Trim().Replace(((char)0).ToString(), "");
            string[] pd = test.Split('?');
            data = Convert.FromBase64String(pd[1]);
            string basepath = "";
            string[] temp = path.Split('\\');
            for (int i = 0; i < temp.Length - 1; i++)
                basepath += temp[i] + "\\";
            File.WriteAllBytes(basepath + pd[0], data);
            b.Dispose();
            File.Delete(path);
            pass.Dispose();
            Console.WriteLine("Unlocked!");
        }
        static void Main(string[] args)
        {
            Console.Write("mode(0-lock,1-unlock): ");
            string mode = Console.ReadLine();
            Console.Write("password picture: ");
            string pic = Console.ReadLine();
            Console.Write("password: ");
            Console.ForegroundColor = ConsoleColor.Black;
            string pass = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (string p in args)
            {
                if (mode == "0") { LockFile(p, pass, pic); }
                if (mode == "1") { UnLockFile(p, pass, pic); }
            }
            Console.ReadLine();
            
        }
    }
}
