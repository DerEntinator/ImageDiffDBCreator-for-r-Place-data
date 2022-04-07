using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Text;

string folderPath = Console.ReadLine();
string[] files = Directory.GetFiles(folderPath);

int n = 0;
if (File.Exists(folderPath + "\\#diffdata.bin"))
    n = 1;

FileStream diffStream = new(folderPath + "\\#diffdata.bin", FileMode.Create, FileAccess.Write);
using BinaryWriter diffFile = new BinaryWriter(diffStream);

string[][] colorTable = {
    new string[] { "00", "000000" },
    new string[] { "01", "00756F" },
    new string[] { "02", "009EAA" },
    new string[] { "03", "00A368" },
    new string[] { "04", "00CC78" },
    new string[] { "05", "00CCC0" },
    new string[] { "06", "2450A4" },
    new string[] { "07", "3690EA" },
    new string[] { "08", "493AC1" },
    new string[] { "09", "515252" },
    new string[] { "0A", "51E9F4" },
    new string[] { "0B", "6A5CFF" },
    new string[] { "0C", "6D001A" },
    new string[] { "0D", "6D482F" },
    new string[] { "0E", "7EED56" },
    new string[] { "0F", "811E9F" },
    new string[] { "10", "898D90" },
    new string[] { "11", "94B3FF" },
    new string[] { "12", "9C6926" },
    new string[] { "13", "B44AC0" },
    new string[] { "14", "BE0039" },
    new string[] { "15", "D4D7D9" },
    new string[] { "16", "DE107F" },
    new string[] { "17", "E4ABFF" },
    new string[] { "18", "FF3881" },
    new string[] { "19", "FF4500" },
    new string[] { "1A", "FF99AA" },
    new string[] { "1B", "FFA800" },
    new string[] { "1C", "FFB470" },
    new string[] { "1D", "FFD635" },
    new string[] { "1E", "FFF8B8" },
    new string[] { "1F", "FFFFFF" }
};

foreach (string[] color in colorTable)
    diffFile.Write(Convert.FromHexString(color[0] + color[1]));

bool newSet = true;
ConcurrentQueue<byte[]> changes = new ConcurrentQueue<byte[]>();


//var timer = new Stopwatch(); timer.Start();

for (int i = n; i < files.Length - 1; i++)
{
    string sourceImagePath = files[i];
    string targetImagePath = files[i + 1];

    if (newSet == true)
    {
        changes.Enqueue(Encoding.ASCII.GetBytes(@":RefImage:" + sourceImagePath.Substring(sourceImagePath.Length - 16) + @":"));
        changes.Enqueue(Encoding.ASCII.GetBytes(@"ImageData:"));
        changes.Enqueue(File.ReadAllBytes(sourceImagePath));
        newSet = false;
    }

    if (Convert.ToByte(sourceImagePath.Substring(sourceImagePath.Length - 16).Substring(0, 1)) != Convert.ToByte(targetImagePath.Substring(targetImagePath.Length - 16).Substring(0, 1)))
    {
        newSet = true;
    }
    else
    {
        changes.Enqueue(Encoding.ASCII.GetBytes(":diff:"));
        changes.Enqueue(Convert.FromHexString(
            (
            /*source creation time*/Convert.ToUInt32(targetImagePath.Substring(sourceImagePath.Length - 14).Substring(0, 10)) -
            /*target creation time*/Convert.ToUInt32(sourceImagePath.Substring(targetImagePath.Length - 14).Substring(0, 10))
            ).ToString("X4")));
        changes.Enqueue(Encoding.ASCII.GetBytes(":"));

        Image<Rgb24> sourceImage = Image.Load<Rgb24>(sourceImagePath);
        Image<Rgb24> targetImage = Image.Load<Rgb24>(targetImagePath);

        int offsetX = 0, offsetY = 0;
        if (sourceImagePath.Substring(0, 1) != "0")
        {
            switch (Convert.ToByte(sourceImagePath.Substring(sourceImagePath.Length - 16).Substring(0, 1)))
            {
                case 0:
                    offsetX = 0; offsetY = 0; break;
                case 1:
                    offsetX = sourceImage.Width; offsetY = 0; break;
                case 2:
                    offsetX = 0; offsetY = sourceImage.Height; break;
                case 3:
                    offsetX = sourceImage.Width; offsetY = sourceImage.Height; break;
            }
        }

        if (GC.TryStartNoGCRegion((long)Math.Pow(2, 27)-1, true))
        {
            try
            {
                Parallel.For(0, sourceImage.Height, y =>
               {
                   Span<Rgb24> sourceRow = sourceImage.DangerousGetPixelRowMemory(y).Span;
                   Span<Rgb24> targetRow = targetImage.DangerousGetPixelRowMemory(y).Span;

                   for (int x = 0; x < sourceRow.Length; x++)
                   {
                       if (sourceRow[x] != targetRow[x])
                       {
                           switch (targetRow[x].R.ToString("X2") + targetRow[x].G.ToString("X2") + targetRow[x].B.ToString("X2"))
                           {
                               case "000000":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "00")); break;
                               case "00756F":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "01")); break;
                               case "009EAA":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "02")); break;
                               case "00A368":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "03")); break;
                               case "00CC78":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "04")); break;
                               case "00CCC0":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "05")); break;
                               case "2450A4":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "06")); break;
                               case "3690EA":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "07")); break;
                               case "493AC1":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "08")); break;
                               case "515252":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "09")); break;
                               case "51E9F4":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "0A")); break;
                               case "6A5CFF":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "0B")); break;
                               case "6D001A":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "0C")); break;
                               case "6D482F":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "0D")); break;
                               case "7EED56":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "0E")); break;
                               case "811E9F":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "0F")); break;
                               case "898D90":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "10")); break;
                               case "94B3FF":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "11")); break;
                               case "9C6926":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "12")); break;
                               case "B44AC0":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "13")); break;
                               case "BE0039":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "14")); break;
                               case "D4D7D9":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "15")); break;
                               case "DE107F":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "16")); break;
                               case "E4ABFF":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "17")); break;
                               case "FF3881":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "18")); break;
                               case "FF4500":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "19")); break;
                               case "FF99AA":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "1A")); break;
                               case "FFA800":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "1B")); break;
                               case "FFB470":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "1C")); break;
                               case "FFD635":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "1D")); break;
                               case "FFF8B8":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "1E")); break;
                               case "FFFFFF":
                                   changes.Enqueue(Convert.FromHexString((x + offsetX).ToString("X3") + (y + offsetY).ToString("X3") + "1F")); break;
                           }
                       }
                   }
               });
            }
            finally
            {
                GC.EndNoGCRegion();
                if (i % 5 == 0)
                {
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
        }
    }

    byte[] result;
    while (changes.TryDequeue(out result))
    {
        diffFile.Write(result);
    };


    if (i % 100 == 0)
    {
        Console.WriteLine(i);
        //timer.Stop();
        //Console.WriteLine(timer.ElapsedTicks);
        //timer.Reset();
        //timer.Start();
    }
}


static byte[] CombineBArrays(byte[] first, byte[] second)
{
    byte[] ret = new byte[first.Length + second.Length];
    Buffer.BlockCopy(first, 0, ret, 0, first.Length);
    Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
    return ret;
}