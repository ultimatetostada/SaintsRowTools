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

        public void Dump(StreamWriter sw)
        {
            sw.WriteLine("Header:");
            sw.WriteLine("  Version: {0}", Header.Version);
            sw.WriteLine("  DataSize: {0}", Header.DataSize);
            sw.WriteLine("  Name: {0}", Header.Name);
            sw.WriteLine("  NumPasses: {0}", Header.NumPasses);
            sw.WriteLine("  AirResistance: {0}", Header.AirResistance);
            sw.WriteLine("  WindMultiplier: {0}", Header.WindMultiplier);
            sw.WriteLine("  WindConst: {0}", Header.WindConst);
            sw.WriteLine("  GravityMultiplier: {0}", Header.GravityMultiplier);
            sw.WriteLine("  ObjectVelocityInheritance: {0}", Header.ObjectVelocityInheritance);
            sw.WriteLine("  ObjectPositionInheritance: {0}", Header.ObjectPositionInheritance);
            sw.WriteLine("  ObjectRotationInheritance: {0}", Header.ObjectRotationInheritance);
            sw.WriteLine("  WindType: {0}", Header.WindType);
            sw.WriteLine("  NumNodes: {0}", Header.NumNodes);
            sw.WriteLine("  NumAnchorNodes: {0}", Header.NumAnchorNodes);
            sw.WriteLine("  NumNodeLinks: {0}", Header.NumNodeLinks);
            sw.WriteLine("  NumRopes: {0}", Header.NumRopes);
            sw.WriteLine("  NumColliders: {0}", Header.NumColliders);
            sw.WriteLine("  BoundingSphereRadius: {0}", Header.BoundingSphereRadius);
            sw.WriteLine("  NodesPtr: {0}", Header.NodesPtr);
            sw.WriteLine("  NodeLinksPtr: {0}", Header.NodeLinksPtr);
            sw.WriteLine("  RopesPtr: {0}", Header.RopesPtr);
            sw.WriteLine("  CollidersPtr: {0}", Header.CollidersPtr);
            sw.WriteLine();

            sw.WriteLine("Nodes:");
            for (int i = 0; i < Nodes.Count; i++)
            {
                SimulatedNodeInfo sni = Nodes[i];
                sw.WriteLine("  Node {0}:", i);
                sw.WriteLine("    BoneIndex: {0}", sni.BoneIndex);
                sw.WriteLine("    ParentNodeIndex: {0}", sni.ParentNodeIndex);
                sw.WriteLine("    GravityLink: {0}", sni.GravityLink);
                sw.WriteLine("    Anchor: {0}", sni.Anchor);
                sw.WriteLine("    Collide: {0}", sni.Collide);
                sw.WriteLine("    WindMultiplier: {0}", sni.WindMultiplier);
                sw.WriteLine("    Pos: ({0}, {1}, {2}, {3})", sni.Pos.X, sni.Pos.Y, sni.Pos.Z, sni.Pos.DuplicateZ);
                sw.WriteLine("    LocalSpacePos: ({0}, {1}, {2}, {3})", sni.LocalSpacePos.X, sni.LocalSpacePos.Y, sni.LocalSpacePos.Z, sni.LocalSpacePos.DuplicateZ);
            }
            sw.WriteLine();

            sw.WriteLine("Node Links:");
            for (int i = 0; i < NodeLinks.Count; i++)
            {
                SimulatedNodeLinkInfo snli = NodeLinks[i];
                sw.WriteLine("  Node Link {0}:", i);
                sw.WriteLine("    NodeIndex1: {0}", snli.NodeIndex1);
                sw.WriteLine("    NodeIndex2: {0}", snli.NodeIndex2);
                sw.WriteLine("    Collide: {0}", snli.Collide);
                sw.WriteLine("    Length: {0}", snli.Length);
                sw.WriteLine("    StretchLen: {0}", snli.StretchLen);
                sw.WriteLine("    Twist: {0}", snli.Twist);
                sw.WriteLine("    Spring: {0}", snli.Spring);
                sw.WriteLine("    Damp: {0}", snli.Damp);
            }
            sw.WriteLine();

            sw.WriteLine("Collision Primitives:");
            for (int i = 0; i < CollisionPrimitives.Count; i++)
            {
                ClothSimCollisionPrimitiveInfo cscpi = CollisionPrimitives[i];
                sw.WriteLine("  Collision Primitive: {0}:", i);
                sw.WriteLine("    BoneIndex: {0}", cscpi.BoneIndex);
                sw.WriteLine("    IsCapsule: {0}", cscpi.IsCapsule);
                sw.WriteLine("    DoScale: {0}", cscpi.DoScale);
                sw.WriteLine("    Radius: {0}", cscpi.Radius);
                sw.WriteLine("    Height: {0}", cscpi.Height);
                sw.WriteLine("    Pos: ({0}, {1}, {2}, {3})", cscpi.Pos.X, cscpi.Pos.Y, cscpi.Pos.Z, cscpi.Pos.DuplicateZ);
                sw.WriteLine("    Axis: ({0}, {1}, {2}, {3})", cscpi.Axis.X, cscpi.Axis.Y, cscpi.Axis.Z, cscpi.Axis.DuplicateZ);
                sw.WriteLine("    LocalSpacePos: ({0}, {1}, {2}, {3})", cscpi.LocalSpacePos.X, cscpi.LocalSpacePos.Y, cscpi.LocalSpacePos.Z, cscpi.LocalSpacePos.DuplicateZ);
            }
            sw.WriteLine();

        }
    }
}
