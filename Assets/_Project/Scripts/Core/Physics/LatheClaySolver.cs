using UnityEngine;

namespace ClayForge.Core.Physics
{
    /// <summary>
    /// Visco-plastic lathe prototype: a stack of radii with volume conservation.
    /// Pressing a ring sends mass upward (walls rise). Off-center mass is a 2D offset.
    /// </summary>
    public sealed class LatheClaySolver : IClaySolver
    {
        private readonly int _ringCount;
        private readonly int _radialSegments;
        private readonly float[] _radii;
        private float _height;
        private Vector2 _centerOffset;
        private float _moisture = 0.65f;
        private Mesh _ownedMesh;

        public LatheClaySolver(int ringCount = 28, int radialSegments = 36)
        {
            _ringCount = Mathf.Max(8, ringCount);
            _radialSegments = Mathf.Max(12, radialSegments);
            _radii = new float[_ringCount];
        }

        public float Height => _height;
        public float MaxRadius
        {
            get
            {
                var max = 0f;
                for (var i = 0; i < _radii.Length; i++)
                    max = Mathf.Max(max, _radii[i]);
                return max;
            }
        }

        public Vector2 CenterOffset => _centerOffset;
        public float Moisture
        {
            get => _moisture;
            set => _moisture = Mathf.Clamp01(value);
        }

        public void ResetToLump(float radius, float height)
        {
            _height = Mathf.Max(0.08f, height);
            _centerOffset = new Vector2(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f));
            _moisture = 0.7f;
            for (var i = 0; i < _ringCount; i++)
            {
                var t = i / (float)(_ringCount - 1);
                var bulge = 1f - 0.18f * t * t;
                var bottom = Mathf.SmoothStep(0.72f, 1f, t);
                _radii[i] = radius * bulge * bottom;
            }

            _radii[0] *= 0.82f;
        }

        public void ApplyContact(in ClayContact contact, Transform clayTransform, Data.ClayType clayType)
        {
            if (clayType == null)
                return;

            var local = clayTransform.InverseTransformPoint(contact.WorldPoint);
            var localVel = clayTransform.InverseTransformVector(contact.WorldVelocity);
            var t = Mathf.InverseLerp(0f, _height, local.y);
            var centerRing = t * (_ringCount - 1);
            var plasticity = clayType.Plasticity * Mathf.Lerp(0.45f, 1.15f, _moisture);
            var strength = Mathf.Max(0.12f, clayType.Strength);
            var influence = Mathf.Max(0.04f, contact.InfluenceRadius);

            if (contact.Mode == ClayToolMode.Cut)
            {
                Cut(centerRing, influence, contact.Pressure);
                ClampRadii();
                return;
            }

            var radial = new Vector2(local.x, local.z);
            var radialDir = radial.sqrMagnitude > 0.0001f ? radial.normalized : Vector2.right;
            var inward = -radialDir;
            var inwardAmount = contact.Pressure * plasticity * Time.deltaTime;

            if (contact.Mode == ClayToolMode.Stretch || localVel.y > 0.12f)
                inwardAmount *= 0.35f;

            var massMoved = 0f;
            for (var i = 0; i < _ringCount; i++)
            {
                var dist = Mathf.Abs(i - centerRing) / (_ringCount * (influence / Mathf.Max(0.08f, _height)));
                var falloff = Mathf.Exp(-dist * dist * 3.2f);
                if (falloff < 0.02f)
                    continue;

                var delta = inwardAmount * falloff;
                var maxCut = _radii[i] - 0.03f;
                delta = Mathf.Min(delta, maxCut);
                if (delta <= 0f)
                    continue;

                _radii[i] -= delta;
                massMoved += delta * _radii[i];
            }

            // Volume conservation: squeezed mass climbs the wall.
            if (massMoved > 0f)
            {
                var lift = massMoved * (0.55f + plasticity * 0.6f);
                var target = Mathf.Clamp(Mathf.CeilToInt(centerRing + 1), 1, _ringCount - 1);
                for (var i = target; i < _ringCount; i++)
                {
                    var w = 1f - (i - target) / (float)_ringCount;
                    _radii[i] += lift * w * 0.18f;
                }

                _height += lift * 0.45f;
                _height = Mathf.Clamp(_height, 0.12f, 1.35f);
            }

            if (localVel.y > 0.05f)
            {
                _height += localVel.y * plasticity * Time.deltaTime * 0.25f;
                _height = Mathf.Clamp(_height, 0.12f, 1.35f);
            }

            // One-sided pressure walks the lump off-axis; opposing pressure is handled by CenteringSystem.
            var push = new Vector2(local.x, local.z) * (contact.Pressure * (1.15f - strength) * Time.deltaTime * 0.35f);
            _centerOffset += push;
            _centerOffset = Vector2.ClampMagnitude(_centerOffset, 0.18f);

            _moisture = Mathf.Max(0.12f, _moisture - Time.deltaTime * 0.01f);
            ClampRadii();
        }

        public void NudgeCenter(Vector2 delta)
        {
            _centerOffset = Vector2.ClampMagnitude(_centerOffset + delta, 0.22f);
        }

        public void PullTowardCenter(float amount)
        {
            _centerOffset = Vector2.MoveTowards(_centerOffset, Vector2.zero, Mathf.Max(0f, amount));
        }

        public void AddWater(float amount)
        {
            _moisture = Mathf.Clamp01(_moisture + amount);
        }

        public void Shrink(float factor)
        {
            factor = Mathf.Clamp(factor, 0.7f, 1f);
            _height *= factor;
            for (var i = 0; i < _radii.Length; i++)
                _radii[i] *= factor;
        }

        public Mesh RebuildMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                mesh = new Mesh { name = "ClayBody_Lathe" };
                _ownedMesh = mesh;
            }

            var rings = _ringCount;
            var segs = _radialSegments;
            var vertCount = rings * segs + 2;
            var vertices = new Vector3[vertCount];
            var uvs = new Vector2[vertCount];

            for (var y = 0; y < rings; y++)
            {
                var t = y / (float)(rings - 1);
                var h = t * _height;
                var r = _radii[y];
                for (var x = 0; x < segs; x++)
                {
                    var a = x / (float)segs * Mathf.PI * 2f;
                    var index = y * segs + x;
                    vertices[index] = new Vector3(Mathf.Cos(a) * r, h, Mathf.Sin(a) * r);
                    uvs[index] = new Vector2(x / (float)segs, t);
                }
            }

            var bottom = rings * segs;
            var top = bottom + 1;
            vertices[bottom] = Vector3.zero;
            vertices[top] = new Vector3(0f, _height, 0f);
            uvs[bottom] = new Vector2(0.5f, 0f);
            uvs[top] = new Vector2(0.5f, 1f);

            var triCount = (rings - 1) * segs * 6 + segs * 6;
            var tris = new int[triCount];
            var ti = 0;
            for (var y = 0; y < rings - 1; y++)
            {
                for (var x = 0; x < segs; x++)
                {
                    var n = (x + 1) % segs;
                    var a = y * segs + x;
                    var b = y * segs + n;
                    var c = (y + 1) * segs + x;
                    var d = (y + 1) * segs + n;
                    tris[ti++] = a;
                    tris[ti++] = c;
                    tris[ti++] = b;
                    tris[ti++] = b;
                    tris[ti++] = c;
                    tris[ti++] = d;
                }
            }

            for (var x = 0; x < segs; x++)
            {
                var n = (x + 1) % segs;
                tris[ti++] = bottom;
                tris[ti++] = x;
                tris[ti++] = n;
                tris[ti++] = top;
                tris[ti++] = (rings - 1) * segs + n;
                tris[ti++] = (rings - 1) * segs + x;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public ClayShapeSnapshot Capture()
        {
            var copy = new float[_radii.Length];
            System.Array.Copy(_radii, copy, _radii.Length);
            return new ClayShapeSnapshot
            {
                Height = _height,
                Radii = copy,
                CenterOffset = _centerOffset,
                Moisture = _moisture
            };
        }

        public void Restore(ClayShapeSnapshot snapshot)
        {
            if (snapshot.Radii == null)
                return;

            _height = snapshot.Height;
            _centerOffset = snapshot.CenterOffset;
            _moisture = snapshot.Moisture;
            var n = Mathf.Min(_radii.Length, snapshot.Radii.Length);
            for (var i = 0; i < n; i++)
                _radii[i] = snapshot.Radii[i];
        }

        private void Cut(float centerRing, float influence, float pressure)
        {
            for (var i = 0; i < _ringCount; i++)
            {
                var dist = Mathf.Abs(i - centerRing);
                var falloff = Mathf.Exp(-(dist * dist) / (8f * influence + 0.01f));
                _radii[i] = Mathf.Max(0.025f, _radii[i] - pressure * falloff * Time.deltaTime * 0.4f);
            }
        }

        private void ClampRadii()
        {
            for (var i = 0; i < _radii.Length; i++)
                _radii[i] = Mathf.Clamp(_radii[i], 0.025f, 0.85f);

            _radii[0] = Mathf.Min(_radii[0], _radii[1] + 0.04f);
        }
    }
}
