using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThomasJepp.SaintsRow.Saves.SaintsRowIV;

namespace GenerateSavegameDirectory
{


    class Program
    {
        static void Main(string[] args)
        {
            string[] saves = Directory.GetFiles(@"D:\Steam\userdata\3512696\206420\remote", "*.sr3s_pc");

            SavegameDirectory directory = new SavegameDirectory();

            int slotIndex = 0;
            foreach (string save in saves)
            {
                using (Stream s = File.OpenRead(save))
                {
                    SavegameDirectorySlot slot = new SavegameDirectorySlot();
                    slot.SlotInUse = 1;
                    slot.Cash = 300000;
                    slot.NumMissionsCompleted = 2;
                    directory.Slots[slotIndex] = slot;
                }
                slotIndex++;
            }

            directory.SlotsUsed = (uint)slotIndex;

            using (Stream s = File.Create("savedir.sr3d_pc"))
            {
                directory.Save(s);
            }
        }
    }
}
