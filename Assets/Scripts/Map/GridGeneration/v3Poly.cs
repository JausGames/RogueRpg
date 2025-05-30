using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WCF;

namespace GridGenerator
{
    [Serializable]
    public class v3
    {
        public Vector3[] pts;
        [SerializeField]
        private List<Neighbour> neighbours;
        public List<Neighbour> Neighbours { get => neighbours; set => neighbours = value; }

        [SerializeField]
        private List<CrossNeighbour> crossNeighbours = new List<CrossNeighbour>();
        public List<CrossNeighbour> CrossNeighbours { get => crossNeighbours; set => crossNeighbours = value; }

        public bool ContainNeighbour(v3 v3)
        {
            foreach (var neigh in neighbours)
            {
                if (neigh.neighbour == v3) return true;
            }
            return false;
        }
        public virtual List<v3Quad> Subdivide(MeshData data)
        {
            throw new NotImplementedException();
        }
        public void FindNeighbours(MeshData data)
        {
            Neighbours.Clear();
            CrossNeighbours.Clear();
            var polys = new List<v3>();
            foreach (v3 tr in data.TrianglesV3)
                if (!polys.Contains(tr) && tr != this) polys.Add(tr);
            foreach (v3 pl in data.V3Poly)
                if (!polys.Contains(pl) && pl != this) polys.Add(pl);
            foreach (v3 qd in data.Quads)
                if (!polys.Contains(qd) && qd != this) polys.Add(qd);


            foreach (v3 tris in polys)
            {
                var commonPts = new List<int>();
                var neighbourPts = new List<int>();
                for (int i = 0; i < tris.pts.Length; i++)
                {
                    var commonPtIndex = FindCommonPointIndex(tris.pts[i]);
                    if (commonPtIndex != -1)
                    {
                        neighbourPts.Add(i);
                        commonPts.Add(commonPtIndex);
                    }
                }

                if (commonPts.Count == 2)
                {
                    Neighbours.Add(new Neighbour(this, tris, commonPts, neighbourPts));
                }
                else if (commonPts.Count == 1)
                {
                    crossNeighbours.Add(new CrossNeighbour(this, tris, commonPts[0], neighbourPts[0]));
                }
            }

        }
        public bool haveCommonPoint(Vector3 pt)
        {
            foreach (var ownPt in pts)
                if (isSamePoint(pt, ownPt)) return true;
            return false;
        }
        public bool haveCommonPoint(List<Vector3> list, Vector3 checkedPt)
        {
            foreach (var pt in list)
                if (isSamePoint(checkedPt, pt)) return true;
            return false;
        }
        public int FindCommonPointIndex(Vector3 pt)
        {
            var result = -1;
            for (int i = 0; i < pts.Length; i++)
                if (isSamePoint(pt, pts[i])) result = i;
            return result;
        }
        static public bool isSamePoint(Vector3 pt1, Vector3 pt2)
        {
            if (pt1.x == pt2.x
                && pt1.y == pt2.y
                && pt1.z == pt2.z) return true;
            return false;
        }
        static public List<v3> GetQuadWithCommonPt(Vector3 pt, MeshData meshData)
        {
            List<v3> result = new List<v3>();

            foreach (var quad in meshData.Quads)
                foreach (var ptIt in quad.pts)
                    if (isSamePoint(pt, ptIt) && !result.Contains(quad)) result.Add(quad);

            return result;
        }
        public List<Vector3> GetCommonPoints(v3 poly1, v3 poly2)
        {
            List<Vector3> result = new List<Vector3>();

            foreach (var pt1 in poly1.pts)
                foreach (var pt2 in poly2.pts)
                    if (isSamePoint(pt1, pt2)) result.Add(pt1);
            return result;
        }
        public List<Vector3> GetAdjacentPoints(Vector3 point, MeshData meshData)
        {
            List<Vector3> result = new List<Vector3>();
            var id = -1;
            for (int i = 0; i < pts.Length; i++)
                if (isSamePoint(point, pts[i])) id = i;

            result.AddRange(new Vector3[] { pts[(id + 1) % pts.Length], pts[(id + pts.Length - 1) % pts.Length] });

            foreach (var quads in meshData.Quads)
            {
                for (int i = 0; i < quads.pts.Length; i++)
                    if (isSamePoint(point, quads.pts[i]))
                    {
                        var add = true;
                        foreach (var pt in result)
                            if (isSamePoint(pt, quads.pts[(i + 1) % quads.pts.Length])) add = false;
                        if (add) result.Add(quads.pts[(i + 1) % quads.pts.Length]);

                        add = true;
                        foreach (var pt in result)
                            if (isSamePoint(pt, quads.pts[(i + quads.pts.Length - 1) % quads.pts.Length])) add = false;
                        if (add) result.Add(quads.pts[(i + quads.pts.Length - 1) % quads.pts.Length]);
                    }


            }

            return result;
        }
    }


    [Serializable]
    public class v3Tris : v3
    {
        public int layerNb, neighbourLayer = 0;
        public v3Tris(Vector3[] pts, int layerNb = 0, int neighbourLayer = 0)
        {
            this.pts = pts;
            Neighbours = new List<Neighbour>();
            this.layerNb = layerNb;
            this.neighbourLayer = neighbourLayer;
        }
        public override List<v3Quad> Subdivide(MeshData data)
        {
            var middle = (pts[0] + pts[1] + pts[2]) / 3f;

            var result = new List<v3Quad>();
            for (int i = 0; i < pts.Length; i++)
            {
                var ptss = new Vector3[] { pts[i], (pts[i] + pts[(i + 1) % pts.Length]) / 2f, middle, (pts[(i + pts.Length - 1) % pts.Length] + pts[i]) / 2f };
                var newQuad = new v3Quad(ptss);
                newQuad.pts = data.CorrectQuad(newQuad);
                result.Add(newQuad);
            }
            return result;
        }
    }
    [Serializable]
    public class v3Quad : v3
    {
        [HideInInspector]
        public v3Tris[] internalTris = new v3Tris[2];

        public Vector3 Position
        {
            get
            {
                var mid = Vector3.zero;

                foreach (var pt in pts)
                {
                    mid += pt;
                }
                mid /= pts.Length;

                return mid;
            }
        }


        public v3Quad(Vector3[] pts, List<Neighbour> neighbours, v3Tris tris1, v3Tris tris2)
        {
            this.pts = pts;
            Neighbours = neighbours;
            internalTris[0] = tris1;
            internalTris[1] = tris2;
        }
        public v3Quad(Vector3[] pts)
        {
            this.pts = pts;
            Neighbours = new List<Neighbour>();
        }
        public void FindNeighbours(v3Quad[] data)
        {
            Neighbours.Clear();
            foreach (v3Quad quad in data)
            {
                if (quad != this && !ContainNeighbour(quad))
                {
                    var commonPts = new List<int>();
                    var neighbourPts = new List<int>();
                    for (int i = 0; i < quad.pts.Length; i++)
                    {
                        var commonPtIndex = FindCommonPointIndex(quad.pts[i]);
                        neighbourPts.Add(i);
                        if (commonPtIndex != -1) commonPts.Add(commonPtIndex);
                    }

                    if (commonPts.Count == 2)
                    {
                        Neighbours.Add(new Neighbour(this, quad, commonPts, neighbourPts));
                    }
                    else if (commonPts.Count == 1)
                    {
                        CrossNeighbours.Add(new CrossNeighbour(this, quad, commonPts[0], neighbourPts[0]));
                    }
                }
            }
        }
        public override List<v3Quad> Subdivide(MeshData data)
        {
            var middle = (pts[0] + pts[1] + pts[2] + pts[3]) / 4f;

            var result = new List<v3Quad>();
            for (int i = 0; i < pts.Length; i++)
            {
                var ptss = new Vector3[] { pts[i], (pts[i] + pts[(i + 1) % pts.Length]) / 2f, middle, (pts[(i + pts.Length - 1) % pts.Length] + pts[i]) / 2f };
                var newQuad = new v3Quad(ptss);
                newQuad.pts = data.CorrectQuad(newQuad);
                result.Add(newQuad);
            }
            return result;
        }

        internal void SelfSmooth(List<Vector3> blackList)
        {
            var edge1 = pts[0] - pts[2];
            var edge2 = pts[1] - pts[3];
            var smoothingValue = 1f;
            //bool[] canMove = new bool[] { true,true,true,true };
            bool[] canMove = new bool[] { !haveCommonPoint(blackList, pts[0]), !haveCommonPoint(blackList, pts[1]), !haveCommonPoint(blackList, pts[2]), !haveCommonPoint(blackList, pts[3]) };

            var differenceSign = edge1.magnitude > edge2.magnitude ? -1 : +1;

            if (edge1.magnitude > edge2.magnitude)
            {
                /*if (canMove[0]) pts[0] -= edge1.normalized * (edge1.magnitude - edge2.magnitude) * 0.5f;
                if (canMove[1]) pts[1] += edge2.normalized * (edge1.magnitude - edge2.magnitude) * 0.5f;
                if (canMove[2]) pts[2] += edge1.normalized * (edge1.magnitude - edge2.magnitude) * 0.5f;
                if (canMove[3]) pts[3] -= edge2.normalized * (edge1.magnitude - edge2.magnitude) * 0.5f;*/
                if (canMove[0]) pts[0] -= edge1.normalized * smoothingValue;
                if (canMove[1]) pts[1] += edge2.normalized * smoothingValue;
                if (canMove[2]) pts[2] += edge1.normalized * smoothingValue;
                if (canMove[3]) pts[3] -= edge2.normalized * smoothingValue;
            }
            else
            {
                /*if (canMove[0]) pts[0] += edge1.normalized * (edge2.magnitude - edge1.magnitude) * 0.5f;
                if (canMove[1]) pts[1] -= edge2.normalized * (edge2.magnitude - edge1.magnitude) * 0.5f;
                if (canMove[2]) pts[2] -= edge1.normalized * (edge2.magnitude - edge1.magnitude) * 0.5f;
                if (canMove[3]) pts[3] += edge2.normalized * (edge2.magnitude - edge1.magnitude) * 0.5f;*/
                if (canMove[0]) pts[0] += edge1.normalized * smoothingValue;
                if (canMove[1]) pts[1] -= edge2.normalized * smoothingValue;
                if (canMove[2]) pts[2] -= edge1.normalized * smoothingValue;
                if (canMove[3]) pts[3] += edge2.normalized * smoothingValue;
            }
        }

    }
    //[Serializable]
    public class Neighbour
    {
        [HideInInspector]
        public v3 self;
        [HideInInspector]
        public v3 neighbour;
        public int[] edgesIndex;
        public int[] neighbourEdgedIndex;
        public int edge
        {
            get
            {
                if (edgesIndex[0] == 0 && edgesIndex[1] == 1
                    || edgesIndex[1] == 0 && edgesIndex[0] == 1)
                    // 0 up
                    return 0;
                else if (edgesIndex[0] == 1 && edgesIndex[1] == 2
                    || edgesIndex[1] == 1 && edgesIndex[0] == 2)
                    // 1 left
                    return 1;
                else if (edgesIndex[0] == 2 && edgesIndex[1] == 3
                    || edgesIndex[1] == 2 && edgesIndex[0] == 3)
                    // 2 back
                    return 2;
                /*else if (edgesIndex[0] == 2 && edgesIndex[1] == 0
                    || edgesIndex[1] == 2 && edgesIndex[0] == 0)
                    // tris last edge
                    return 2;*/
                else if (edgesIndex[0] == 3 && edgesIndex[1] == 0
                    || edgesIndex[1] == 3 && edgesIndex[0] == 0)
                    // 3 right
                    return 3;
                else
                    throw new Exception("no edge found");
            }
        }
        public int oppositeEdge
        {
            get
            {
                if (neighbourEdgedIndex[0] == 0 && neighbourEdgedIndex[1] == 1
                    || neighbourEdgedIndex[1] == 0 && neighbourEdgedIndex[0] == 1)
                    // 0 up
                    return 0;
                else if (neighbourEdgedIndex[0] == 1 && neighbourEdgedIndex[1] == 2
                    || neighbourEdgedIndex[1] == 1 && neighbourEdgedIndex[0] == 2)
                    // 1 left
                    return 1;
                else if (neighbourEdgedIndex[0] == 2 && neighbourEdgedIndex[1] == 3
                    || neighbourEdgedIndex[1] == 2 && neighbourEdgedIndex[0] == 3)
                    // 2 back
                    return 2;
                /*else if (neighbourEdgedIndex[0] == 2 && neighbourEdgedIndex[1] == 0
                    || neighbourEdgedIndex[1] == 2 && neighbourEdgedIndex[0] == 0)
                    // tris last edge
                    return 2;*/
                else if (neighbourEdgedIndex[0] == 3 && neighbourEdgedIndex[1] == 0
                    || neighbourEdgedIndex[1] == 3 && neighbourEdgedIndex[0] == 0)
                    // 3 right
                    return 3;
                else
                    throw new Exception("no edge found");
            }
        }

        public Neighbour(v3 self, v3 tris, List<int> commonPts, List<int> neighbourPts)
        {
            this.edgesIndex = commonPts.ToArray();
            this.neighbour = tris;
            this.self = self;
            this.neighbourEdgedIndex = neighbourPts.ToArray();
        }
        public Neighbour(v3 self, v3 tris, List<int> commonPts)
        {
            this.neighbour = tris;
            this.self = self;
            this.edgesIndex = commonPts.ToArray();
        }

        public bool IsSameQuad(v3 quad)
        {
            return quad == neighbour;
        }

    }
    //[Serializable]
    public class CrossNeighbour
    {
        [HideInInspector]
        public v3 self;
        [HideInInspector]
        public v3 neighbour;
        public int ptIndex;
        public int neighbourPtIndex;

        public CrossNeighbour(v3 self, v3 neighbour, int ptIndex, int neighbourPtIndex)
        {
            this.self = self;
            this.neighbour = neighbour;
            this.ptIndex = ptIndex;
            this.neighbourPtIndex = neighbourPtIndex;
        }
    }
}
