using System.Collections;
using System.Collections.Generic;
using DiractionTeam.Utils;
using UnityEngine;

public static class DrawUtils
{
    public static GizmosDrawer gizmosDrawer { get; } = new();
    public static DebugDrawer debugDrawer { get; } = new();
    
    public abstract class Drawer
    {
        private const float _sphereConst3 = 0.57735026919f;
        private const float _sphereConst2 = 0.5f;
        public virtual Color Color { get; set; } = Color.white;
        public abstract void DrawLine(Vector3 from, Vector3 to);

        public virtual void DrawMesh(Mesh mesh, Vector3 position)
        {
            DrawWireMesh(mesh, position);
        }

        public virtual void DrawWireMesh(Mesh mesh, Vector3 position)
        {
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;
            
            for (int i = 0; i < triangles.Length / 3; i++)
            {
                var v0 = vertices[triangles[i * 3]] + position;
                var v1 = vertices[triangles[i * 3 + 1]] + position;
                var v2 = vertices[triangles[i * 3 + 2]] + position;
                
                DrawLine(v0, v1);
                DrawLine(v1, v2);
                DrawLine(v2, v0);
            }
        }

        public virtual void DrawWireCube(Vector3 center, Vector3 size)
        {
            /*
             * . . v0 --- v1
             * . /  |     / |
             * . v3 --- v2  |
             * . |  v7-- | -v6
             * . | /     | /
             * . v4 --- v5
             */
            var v0 = new Vector3(- size.x / 2, + size.y / 2,+ size.z / 2);
            var v1 = new Vector3(+ size.x / 2, + size.y / 2, + size.z / 2);
            var v2 = new Vector3(+ size.x / 2, + size.y / 2, - size.z / 2);
            var v3 = new Vector3(- size.x / 2, + size.y / 2, - size.z / 2);
            var v4 = new Vector3(- size.x / 2, - size.y / 2, - size.z / 2);
            var v5 = new Vector3(+ size.x / 2, - size.y / 2, - size.z / 2);
            var v6 = new Vector3(+ size.x / 2, - size.y / 2, + size.z / 2);
            var v7 = new Vector3(- size.x / 2, - size.y / 2,+ size.z / 2);

            var mesh = new Mesh();

            mesh.vertices = new[]
            {
                v0, v1, v2, v3, v4, v5, v6, v7
            };
            mesh.triangles = new[]
            {
                3, 0, 1, // top
                1, 2, 3,
                4, 6, 7, // bottom
                4, 5, 6,
                7, 1, 0, // front,
                7, 6, 1,
                4, 3, 2, // back
                2, 5, 4,
                5, 2, 1, // right,
                1, 6, 5,
                0, 3, 4, // left
                4, 7, 0 
            };
            
            DrawWireMesh(mesh, center);
        }

        public void DrawBounds(Bounds bounds)
        {
            DrawWireQuad(bounds.TopLeft(), bounds.TopRight(), bounds.BottomRight(), bounds.BottomLeft());
        }

        public void DrawWireQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            DrawLine(v0, v1);
            DrawLine(v1, v2);
            DrawLine(v2, v3);
            DrawLine(v3, v0);
        }
        
        public void DrawWireTriangle2D(Vector2 position, Vector2 direction, float height, float angle)
        {
            var edgeLength = height / Mathf.Cos(angle * Mathf.Deg2Rad / 2);

            var p1 = position + (Vector2)(Quaternion.Euler(0, 0, -angle / 2f) * direction * edgeLength);
            var p2 = position + (Vector2)(Quaternion.Euler(0, 0, angle / 2f) * direction * edgeLength); 
        
            DrawLine(position, p1);
            DrawLine(p1, p2);
            DrawLine(position, p2);
        }
        
        public void DrawWireTriangle2D(Vector2 position, Vector2 height, float baseLength)
        {
            var heightValue = height.magnitude;
            var angle = (Mathf.PI - 2 * Mathf.Atan(heightValue / (baseLength / 2))) * Mathf.Rad2Deg;
            
            DrawWireTriangle2D(position, height.normalized, heightValue, angle);
        }

        public void DrawWireCircle(Vector3 position, float radius, Vector3 axis, int segmentsCount = 30)
        {
            var angleStep = 360f / segmentsCount;
            var radiusNormal = Vector3.zero;

            if (axis.z == 0)
                radiusNormal = Vector3.forward;
            else
                radiusNormal = new Vector3(1, 1, (-axis.x - axis.y) / axis.z).normalized;

            radiusNormal *= radius;

            var v0 = CalculateVertex(0);
            var v1 = Vector3.zero;
            var v2 = v0;
            
            for (int i = 1; i < segmentsCount; i++)
            {
                v1 = v2;
                v2 = CalculateVertex(angleStep * i);
                DrawLine(v1, v2);
            }
            
            DrawLine(v2, v0);

            Vector3 CalculateVertex(float angle)
            {
                return position + Quaternion.AngleAxis(angle, axis) * radiusNormal;
            }
        }

        public void DrawCircle(Vector3 center, float radius, Vector3 axis, int segmentsCount = 16)
        {
            var mesh = new Mesh();
            var vertices = new Vector3[segmentsCount];
            
            var angleStep = 360f / segmentsCount;
            var radiusNormal = Vector3.zero;

            if (axis.z == 0)
                radiusNormal = Vector3.forward;
            else
                radiusNormal = new Vector3(1, 1, (-axis.x - axis.y) / axis.z).normalized;

            radiusNormal *= radius;
            
            for (int i = 0; i < segmentsCount; i++)
            {
                vertices[i] = CalculateVertex(angleStep * i);
            }

            var trianglesCount = segmentsCount - 2;
            var indices = new int[trianglesCount * 3];
            
            for (int i = 0; i < trianglesCount; i++)
            {
                var curTr0VertexIndex = i * 3;
                
                indices[curTr0VertexIndex] = 0;
                indices[curTr0VertexIndex + 1] = i + 1;
                indices[curTr0VertexIndex + 2] = i + 2;
            }
            
            mesh.SetVertices(vertices);
            mesh.RecalculateNormals();
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            DrawMesh(mesh, center);

            Vector3 CalculateVertex(float angle)
            {
                return center + Quaternion.AngleAxis(angle, axis) * radiusNormal;
            }
        }

        public void DrawWireCircleXZ(Vector3 position, float radius, int segmentsCount = 16)
        {
            DrawWireCircle(position, radius, Vector3.up, segmentsCount);
        }
    
        public void DrawWireCircleXY(Vector3 position, float radius, int segmentsCount = 16)
        {
            DrawWireCircle(position, radius, Vector3.back, segmentsCount);
        }

        public virtual void DrawWireSphere(Vector3 center, float radius)
        {
            /*
                         *            V10
                         * . .v14  v0 --- v1  v15
                         * .    /  | v12  / |
                         * .v16  v3 --- v2  | v17
                         * . v8    v7--   -v6 - v9
                         * . v18   /  v13  /  v19
                         * .    v4 --- v5
                         *   v20     v11   v21
                         * 
                         */
                        var v0 = new Vector3(- _sphereConst3, + _sphereConst3,+ _sphereConst3) * radius;
                        var v1 = new Vector3(+ _sphereConst3, + _sphereConst3, + _sphereConst3) * radius;
                        var v2 = new Vector3(+ _sphereConst3, + _sphereConst3, - _sphereConst3) * radius;
                        var v3 = new Vector3(- _sphereConst3, + _sphereConst3, - _sphereConst3) * radius;
                        var v4 = new Vector3(- _sphereConst3, - _sphereConst3, - _sphereConst3) * radius;
                        var v5 = new Vector3(+ _sphereConst3, - _sphereConst3, - _sphereConst3) * radius;
                        var v6 = new Vector3(+ _sphereConst3, - _sphereConst3, + _sphereConst3) * radius;
                        var v7 = new Vector3(- _sphereConst3, - _sphereConst3,+ _sphereConst3) * radius;
                        var v8 = new Vector3(- radius, 0, 0);
                        var v9 = new Vector3(+ radius, 0, 0);
                        var v10 = new Vector3(0, +radius, 0);
                        var v11 = new Vector3(0, -radius, 0);
                        var v12 = new Vector3(0, 0, +radius);
                        var v13 = new Vector3(0, 0, -radius);
            
                        var mesh = new Mesh();
                        
            
                        mesh.vertices = new[]
                        {
                            v0, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13
                        };
                        mesh.triangles = new[]
                        {
                            10, 0, 1, // top
                            10, 0, 3,
                            10, 1, 2,
                            10, 2, 3,
                            11, 6, 7, // bottom
                            11, 6, 5,
                            11, 5, 4,
                            11, 4, 7,
                            12, 0, 1, // front
                            12, 0, 7,
                            12, 1, 6,
                            12, 6, 7,
                            13, 3, 2, // back
                            13, 3, 4,
                            13, 4, 5,
                            13, 5, 2,
                            9, 2, 1, // right
                            9, 2, 5,
                            9, 5, 6,
                            9, 6, 1,
                            8, 3, 4, // left
                            8, 3, 0,
                            8, 0, 7,
                            8, 7, 4 
                        };
                        
                        DrawWireMesh(mesh, center);
        }

        public void DrawArrow(Vector3 start, Vector3 end)
        {
            var arrayLine = end - start;
            var arrowLeftPart = arrayLine * 0.1f;
            var arrowRightPart = Quaternion.Euler(0, 0, -135f) * arrowLeftPart;
            arrowLeftPart = Quaternion.Euler(0, 0, 135f) * arrowLeftPart;

            DrawLine(start, end);
            DrawLine(end, end + arrowLeftPart);
            DrawLine(end, end + arrowRightPart);
        }
    }
    
    public class GizmosDrawer : Drawer
    {
        public override Color Color
        {
            get => Gizmos.color;
            set => Gizmos.color = value;
        }

        public override void DrawLine(Vector3 from, Vector3 to)
        {
            Gizmos.DrawLine(from, to);
        }

        public override void DrawMesh(Mesh mesh, Vector3 position)
        {
            Gizmos.DrawMesh(mesh, position);
        }
        
        public void DrawWireCylinder (Vector3 position, Vector3 up, float height, float radius) {
            
            var tangent = Vector3.Cross(up, Vector3.one).normalized;

            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.LookRotation(tangent, up), new Vector3(radius, height, radius));
            DrawWireCircleXZ(Vector3.zero, 1);

            if (height > 0) {
                DrawWireCircleXZ(Vector3.up, 1);
                Gizmos.DrawLine(new Vector3(1, 0, 0), new Vector3(1, 1, 0));
                Gizmos.DrawLine(new Vector3(-1, 0, 0), new Vector3(-1, 1, 0));
                Gizmos.DrawLine(new Vector3(0, 0, 1), new Vector3(0, 1, 1));
                Gizmos.DrawLine(new Vector3(0, 0, -1), new Vector3(0, 1, -1));
            }

            Gizmos.matrix = Matrix4x4.identity;
        }

        public override void DrawWireCube(Vector3 center, Vector3 size)
        {
            Gizmos.DrawWireCube(center, size);
        }

        public override void DrawWireSphere(Vector3 center, float radius)
        {
            Gizmos.DrawWireSphere(center, radius);
        }
    }

    public class DebugDrawer : Drawer
    {
        public float Duration { get; set; }

        public override void DrawLine(Vector3 from, Vector3 to)
        {
            Debug.DrawLine(from, to, Color, Duration);
        }
    }
}
