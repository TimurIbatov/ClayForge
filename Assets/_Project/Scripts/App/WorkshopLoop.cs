using ClayForge.Core;
using ClayForge.Input;
using ClayForge.Save;
using ClayForge.Systems;
using UnityEngine;

namespace ClayForge.App
{
    public sealed class WorkshopLoop : MonoBehaviour
    {
        private GameplayInput _input;
        private PotteryWheel _wheel;
        private ClayBody _clay;
        private DryingSystem _drying;
        private WorkshopCamera _camera;
        private WorkshopDirector _director;

        public void Bind(
            GameplayInput input,
            PotteryWheel wheel,
            ClayBody clay,
            DryingSystem drying,
            WorkshopCamera camera,
            WorkshopDirector director)
        {
            _input = input;
            _wheel = wheel;
            _clay = clay;
            _drying = drying;
            _camera = camera;
            _director = director;
        }

        private void Update()
        {
            if (_input == null)
                return;

            if (_input.ToggleWheel)
                _wheel.Toggle();
            if (Mathf.Abs(_input.Scroll) > 0.01f)
                _wheel.AddSpeed(_input.Scroll * 12f);
            if (_input.AddWater)
                _clay.AddWater(0.12f);
            if (UnityEngine.Input.GetKeyDown(KeyCode.V))
                _camera.Cycle();
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
                _clay.ResetLump();
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
                _director.NextClay();
            if (UnityEngine.Input.GetKeyDown(KeyCode.G))
                ClaySaveService.SaveCurrent(_clay);
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
                _drying.Begin();
        }
    }
}
