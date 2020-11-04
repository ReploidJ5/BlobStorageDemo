using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StorageApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BlobManager myManager = new BlobManager();

            String result = await myManager.InitContainer();
            Console.WriteLine(result);

            IEnumerable<string> names = await myManager.BlobList();

            foreach(var item in names)
            {
                Console.WriteLine(item);
            }

            string file = "./TextFile1.txt";
            
            FileStream streamer = new FileStream(file, FileMode.Open, FileAccess.Read);
            await myManager.Save(streamer, "newfile");

            Stream streamer2 = await myManager.Load("newfile");
            using (var sr = new StreamReader(streamer2))
            {
                Console.WriteLine(sr.ReadToEnd());
            }

        }
    }
}
