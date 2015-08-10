using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThomasJepp.SaintsRow;

namespace MakeDummyClothSim
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string arg in args)
                CreateDummyClothSim(arg);
        }

        static void CreateDummyClothSim(string path)
        {
            Console.WriteLine("Creating dummy cloth sim: {0}", path);
            string name = Path.GetFileNameWithoutExtension(path);

            if (name.Length >= 28) // the field in the struct is only 28 bytes long, and needs to include a null
            {
                name = name.Substring(0, 27);
            }

            using (Stream stream = File.Create(path))
            {
                stream.WriteUInt32(0x02); // version
                stream.WriteUInt32(0); // data size
                stream.WriteAsciiNullTerminatedString(name);
                stream.Align(0x24);
                stream.WriteUInt32(0); // num passes
                stream.WriteUInt32(1); // air resistance
                stream.WriteUInt32(1); // wind multiplier
                stream.WriteUInt32(1); // wind constant
                stream.WriteUInt32(1); // gravity multiplier
                stream.WriteUInt32(0); // object velocity inheritance
                stream.WriteUInt32(0); // object position inheritance
                stream.WriteUInt32(0); // object rotation inheritance
                stream.WriteUInt32(0); // wind type
                stream.WriteUInt32(0); // num nodes
                stream.WriteUInt32(0); // num anchor nodes
                stream.WriteUInt32(0); // num node links
                stream.WriteUInt32(0); // num ropes
                stream.WriteUInt32(0); // num colliders
                //stream.WriteUInt32(0); // bounding sphere radius
                stream.WriteUInt32(0); // *nodes
                stream.WriteUInt32(0); // *node_links
                stream.WriteUInt32(0); // *ropes
                stream.WriteUInt32(0); // *colliders
            }
        }
    }
}
