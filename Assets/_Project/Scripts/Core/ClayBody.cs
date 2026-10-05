using System;
using ClayForge.Core.Physics;
using ClayForge.Data;
using UnityEngine;

namespace ClayForge.Core
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public sealed class ClayBody : MonoBehaviour
    {
        [SerializeField] private ClayType clayType;
        [SerializeField] private float lumpRadius = 0.28f;
        [SerializeField] private float lumpHeight = 0.38f;
        [SerializeField] private int ringCount = 28;
        [SerializeField] private int radialSegments = 36;

        public event Action ShapeChanged;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        private Mesh _mesh;
        private IClaySolver _solver;
        private ClayPhysics _physics;
        private bool _dirty = true;

        public ClayType ClayType => clayType;
        public IClaySolver Solver => _solver;
        public float Moisture => _solver?.Moisture ?? 0f;
        public Vector2 CenterOffset => _solver?.CenterOffset ?? Vector2.zero;
        public float Height => _solver?.Height ?? 0f;

        public void Configure(ClayType type, ClayPhysics physics)
        {
            clayType = type;
            _physics = physics;
            EnsureSolver();
            ApplyColor();
        }

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
            _meshCollider.convex = false;
            EnsureSolver();
            RebuildIfNeeded(force: true);
        }

        public void ApplyContact(in ClayContact contact)
        {
            EnsureSolver();
            _solver.ApplyContact(contact, transform, clayType);
            _dirty = true;
            ShapeChanged?.Invoke();
        }

        public void AddWater(float amount)
        {
            _solver?.AddWater(amount);
            ApplyColor();
        }

        public void ShrinkFromProcess(float factor)
        {
            _solver?.Shrink(factor);
            _dirty = true;
            ApplyColor();
        }

        public void SetProcessColor(Color color)
        {
            if (_meshRenderer == null)
                return;
            _meshRenderer.material.SetColor("_BaseColor", color);
        }

        public void ResetLump()
        {
            EnsureSolver();
            _solver.ResetToLump(lumpRadius, lumpHeight);
            _dirty = true;
            ApplyColor();
            RebuildIfNeeded(force: true);
        }

        private void LateUpdate() => RebuildIfNeeded(force: false);

        private void EnsureSolver()
        {
            if (_solver != null)
                return;

            _solver = _physics != null
                ? _physics.CreateSolver(ringCount, radialSegments)
                : new LatheClaySolver(ringCount, radialSegments);
            _solver.ResetToLump(lumpRadius, lumpHeight);
        }

        private void RebuildIfNeeded(bool force)
        {
            if (!force && !_dirty)
                return;

            _mesh = _solver.RebuildMesh(_mesh);
            _meshFilter.sharedMesh = _mesh;
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
            _dirty = false;
        }

        private void ApplyColor()
        {
            if (_meshRenderer == null || clayType == null)
                return;

            var c = Color.Lerp(clayType.LeatherHardColor, clayType.WetColor, Moisture);
            if (_meshRenderer.material != null)
                _meshRenderer.material.color = c;
        }
    }
}
