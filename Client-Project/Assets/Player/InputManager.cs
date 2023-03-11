using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
#endif

namespace StarterAssets
{
	public class InputManager : MonoBehaviour
	{
		[Header("Player Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
        public bool shoot;
        public bool reload;
        public bool ads;
		public int weapon;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
            move = value.Get<Vector2>();
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
                look = value.Get<Vector2>();
			}
		}

		//Buttons
		public void OnJump(InputValue value)
		{
            jump = value.isPressed;
		}
		public void OnSprint(InputValue value)
		{
            sprint = value.isPressed;
		}
        public void OnShoot(InputValue value)
        {
            shoot = value.isPressed;
        }
        public void OnReload(InputValue value)
        {
            reload = value.isPressed;
        }
        public void OnADS(InputValue value)
        {
            ads = value.isPressed;
        }

        //Switch weapon
        public void OnWeapon(InputValue value)
        {
			float scrollValue = value.Get<float>();
            weapon += (int)scrollValue;
        }
#endif


        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}