using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using ThomasJepp.SaintsRow.MiscTypes;

namespace ThomasJepp.SaintsRow.ClothSimulation.Version02
{
    [StructLayout(LayoutKind.Explicit, Size = 0x24)]
    public struct SimulatedNodeInfo
    {
        [FieldOffset(0x00)]
        public byte BoneIndex;

        [FieldOffset(0x01)]
        public byte ParentNodeIndex;

        [FieldOffset(0x02)]
        public byte GravityLink;

        [FieldOffset(0x03)]
        public byte Anchor;

        [FieldOffset(0x04)]
        public UInt32 Collide;

        [FieldOffset(0x08)]
        public float WindMultiplier;

        [FieldOffset(0x0C)]
        public FLVector Pos;

        [FieldOffset(0x18)]
        public FLVector LocalSpacePos;
    }
}
