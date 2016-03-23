using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThomasJepp.SaintsRow.ClothSimulation.Version05
{
    public class ClothSimulationFile
    {
        public ClothSimulationHeader Header;
        public List<SimulatedNodeInfo> Nodes;
        public List<SimulatedNodeLinkInfo> NodeLinks;
        public List<ClothSimRopeInfo> Ropes;
        public List<List<UInt32>> RopeNodes;
        public List<List<UInt32>> RopeLinks;
        public List<ClothSimCollisionPrimitiveInfo> CollisionPrimitives;

        public ClothSimulationFile(Stream s)
        {
            Header = s.ReadStruct<ClothSimulationHeader>();

            Nodes = new List<SimulatedNodeInfo>();
            NodeLinks = new List<SimulatedNodeLinkInfo>();
            Ropes = new List<ClothSimRopeInfo>();
            RopeNodes = new List<List<uint>>();
            RopeLinks = new List<List<uint>>();
            CollisionPrimitives = new List<ClothSimCollisionPrimitiveInfo>();


            for (int i = 0; i < Header.NumNodes; i++)
            {
                SimulatedNodeInfo sni = s.ReadStruct<SimulatedNodeInfo>();
                Nodes.Add(sni);
            }

            for (int i = 0; i < Header.NumNodeLinks; i++)
            {
                SimulatedNodeLinkInfo snli = s.ReadStruct<SimulatedNodeLinkInfo>();
                NodeLinks.Add(snli);
            }

            for (int i = 0; i < Header.NumColliders; i++)
            {
                ClothSimCollisionPrimitiveInfo cscpi = s.ReadStruct<ClothSimCollisionPrimitiveInfo>();
                CollisionPrimitives.Add(cscpi);
            }

            s.Align(8);

            for (int i = 0; i < Header.NumRopes; i++)
            {
                ClothSimRopeInfo csri = s.ReadStruct<ClothSimRopeInfo>();
                Ropes.Add(csri);
            }

            for (int i = 0; i < Header.NumRopes; i++)
            {
                ClothSimRopeInfo csri = Ropes[i];
                List<UInt32> ropeNodes = new List<uint>();

                for (int j = 0; j < csri.NumNodes; j++)
                {
                    uint ropeNode = s.ReadUInt32();
                    ropeNodes.Add(ropeNode);
                }

                RopeNodes.Add(ropeNodes);

                List<UInt32> ropeLinks = new List<uint>();

                for (int j = 0; j < csri.NumLinks; j++)
                {
                    uint ropeLink = s.ReadUInt32();
                    ropeLinks.Add(ropeLink);
                }

                RopeLinks.Add(ropeLinks);
            }
        }
        
        public ClothSimulationFile()
        {
            Header = new ClothSimulationHeader();
            Nodes = new List<SimulatedNodeInfo>();
            NodeLinks = new List<SimulatedNodeLinkInfo>();
            Ropes = new List<ClothSimRopeInfo>();
            RopeNodes = new List<List<uint>>();
            RopeLinks = new List<List<uint>>();
            CollisionPrimitives = new List<ClothSimCollisionPrimitiveInfo>();
        }

        public void Save(Stream s)
        {
            s.WriteStruct(Header);

            foreach (SimulatedNodeInfo sni in Nodes)
            {
                s.WriteStruct(sni);
            }

            foreach (SimulatedNodeLinkInfo snli in NodeLinks)
            {
                s.WriteStruct(snli);
            }

            foreach (ClothSimCollisionPrimitiveInfo cscpi in CollisionPrimitives)
            {
                s.WriteStruct(cscpi);
            }

            s.Align(8);

            foreach (ClothSimRopeInfo csri in Ropes)
            {
                s.WriteStruct(csri);
            }

            for (int i = 0; i < Header.NumRopes; i++)
            {
                List<UInt32> ropeNodes = RopeNodes[i];
                
                foreach (UInt32 ropeNode in ropeNodes)
                {
                    s.WriteUInt32(ropeNode);
                }

                List<UInt32> ropeLinks = RopeLinks[i];

                foreach (UInt32 ropeLink in ropeLinks)
                {
                    s.WriteUInt32(ropeLink);
                }
            }
        }
    }
}
