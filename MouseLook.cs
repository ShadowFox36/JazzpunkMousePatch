using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;


[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{

	private void UpdateSitCam()
	{
		Vector2 analogLook = this.GetAnalogLook();
		float num = analogLook.x * this.sensitivityX * this._globSensitivityNum * this._padLookXSensitivityNum;
		if (MouseLook._pcModeBln && !MouseLook._usingGamepadBln)
		{
			num = Input.GetAxis("Mouse X") * this.sensitivityX * this._globSensitivityNum + cInput.GetAxis(this._lookXStr) * this.sensitivityX * this._globSensitivityNum;
		}
		this._rotXNum += num * this._deltaNum;
		this._rotXNum = Mathf.Clamp(this._rotXNum, this.minimumX, this.maximumX);
		float num2 = analogLook.y * this.sensitivityY * this._globSensitivityNum * this._padLookYSensitivityNum * 0.625f;
		if (MouseLook._pcModeBln && !MouseLook._usingGamepadBln)
		{
			num2 = Input.GetAxis("Mouse Y") * this.sensitivityX * this._globSensitivityNum + cInput.GetAxis(this._lookXStr) * this.sensitivityX * this._globSensitivityNum;
			if (!this._mouseInvertBln)
			{
				num2 *= -1f;
			}
		}
		this.rotationY += num2 * this._deltaNum;
		this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
		float num3 = this.rotationY;
		this._locRot = Quaternion.Euler(num3, this._rotXNum, 0f);
	}


	public void SetSecondaryController()
	{
		this._lookXStr += "2";
		this._lookYStr += "2";
	}


	public void SetAdditionalController(int playerNum)
	{
		this._lookXStr += playerNum.ToString();
		this._lookYStr += playerNum.ToString();
		this._playerNum = playerNum;
	}


	public void MouseLookToggleInvertLocal()
	{
		if (this._mouseInvertBln)
		{
			this._mouseInvertBln = false;
			return;
		}
		this._mouseInvertBln = true;
	}


	public void ToggleInvertMultiplayer(int playerNum)
	{
		if (this._playerNum == playerNum)
		{
			Debug.Log("invert player stick y:" + playerNum.ToString());
			if (this._mouseInvertBln)
			{
				this._mouseInvertBln = false;
				return;
			}
			this._mouseInvertBln = true;
		}
	}


	public void MouseLookToggleInvert()
	{
		if (this._mouseInvertBln)
		{
			this._mouseInvertBln = false;
			PlayerPrefs.SetInt("invertMouse", 0);
			return;
		}
		this._mouseInvertBln = true;
		PlayerPrefs.SetInt("invertMouse", 1);
	}


	public void MLSetGlobSens(float valNum)
	{
		this._globSensitivityNum = valNum;
	}


	public void MLSetSmoothing(float valNum)
	{
		this._smoothingNum = valNum;
	}


	private void UpdateUsingGamepad()
	{
		MouseLook._usingGamepadBln = false;
		if (PlayerPrefs.GetInt("useGamepad", 0) == 1)
		{
			MouseLook._usingGamepadBln = true;
		}
	}


	private void FixedUpdate()
	{
	}


	public void UpdateLookRotations()
	{
		this._deltaNum = 1f;
		if (!this._activeBln)
		{
			return;
		}
		if (Time.timeSinceLevelLoad == 0f)
		{
			return;
		}
		Vector2 vector = Vector2.zero;
		this.UpdateUsingGamepad();
		if (MouseLook._usingGamepadBln)
		{
			vector = this.GetAnalogLook();
		}
		else if (!this._updateSetupBln)
		{
			return;
		}
		if (this.axes == MouseLook.RotationAxes.MouseXAndY)
		{
			if (MouseLook._usingGamepadBln)
			{
				this.UpdateSitCam();
			}
			base.transform.localRotation = this._locRot;
		}
		else if (this.axes == MouseLook.RotationAxes.MouseX)
		{
			if (MouseLook._usingGamepadBln)
			{
				float num = vector.x * this.sensitivityX * this._globSensitivityNum * this._padLookXSensitivityNum * Time.deltaTime * this._deltaCoefNum;
				this._rotXNum += num * this._deltaNum;
				if (this._rotXNum < -360f)
				{
					this._rotXNum += 360f;
				}
				if (this._rotXNum > 360f)
				{
					this._rotXNum -= 360f;
				}
				this._rotXNum = Mathf.Clamp(this._rotXNum, this.minimumX, this.maximumX);
				this._globRot = Quaternion.Euler(0f, this._rotXNum, 0f);
			}
			base.transform.rotation = this._globRot;
		}
		else
		{
			if (MouseLook._usingGamepadBln)
			{
				float num2 = vector.y * this.sensitivityY * this._globSensitivityNum * this._padLookYSensitivityNum * Time.deltaTime * this._deltaCoefNum;
				this.rotationY += num2 * this._deltaNum;
				this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
				float num3 = this.rotationY;
				this._locRot = Quaternion.Euler(num3, base.transform.localEulerAngles.y, 0f);
			}
			base.transform.localRotation = this._locRot;
		}
		this._lookUpdatedBln = true;
		if (this._externallyResetBln)
		{
			this._externallyResetBln = false;
		}
	}


	public void SetLookReset()
	{
		MonoBehaviour.print("setlOOkReset:" + Time.time.ToString());
		this.Update();
		this.UpdateLookRotations();
	}


	public bool CheckLookResetExternally()
	{
		bool flag = false;
		if (this._rotXNum != this._lastRotXNum && Time.timeSinceLevelLoad > 0.5f)
		{
			flag = true;
		}
		if (this.rotationY != this._lastRotYNum && Time.timeSinceLevelLoad > 0.5f)
		{
			flag = true;
		}
		if (flag && base.gameObject.name == "Player")
		{
			MonoBehaviour.print("ResetExternal" + Time.time.ToString());
		}
		return flag;
	}


	public void Update()
	{
		if (!this._activeBln)
		{
			return;
		}
		if (Time.timeSinceLevelLoad == 0f)
		{
			return;
		}
		if (MouseLook._usingGamepadBln)
		{
			return;
		}
		if (!this._updateSetupBln)
		{
			this._updateSetupBln = true;
		}
		this._lookUpdatedBln = false;
		if (this.axes == MouseLook.RotationAxes.MouseXAndY)
		{
			this.UpdateSitCam();
		}
		else if (this.axes == MouseLook.RotationAxes.MouseX)
		{
			float num = Input.GetAxis("Mouse X") * this.sensitivityX * this._globSensitivityNum;
			if (this._externallyResetBln)
			{
				num = 0f;
			}
			this._rotXNum += num * this._deltaNum;
			if (this._rotXNum < -360f)
			{
				this._rotXNum += 360f;
			}
			if (this._rotXNum > 360f)
			{
				this._rotXNum -= 360f;
			}
			this._rotXNum = Mathf.Clamp(this._rotXNum, this.minimumX, this.maximumX);
			this._globRot = Quaternion.Euler(0f, this._rotXNum, 0f);
		}
		else
		{
			float num2 = Input.GetAxis("Mouse Y") * this.sensitivityY * this._globSensitivityNum;
			if (this._externallyResetBln)
			{
				num2 = 0f;
			}
			if (!this._mouseInvertBln)
			{
				num2 *= -1f;
			}
			this.rotationY += num2 * this._deltaNum;
			this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
			float num3 = this.rotationY;
			bool mouseInvertBln = this._mouseInvertBln;
			this._locRot = Quaternion.Euler(num3, base.transform.localEulerAngles.y, 0f);
		}
		this._lastRotXNum = this._rotXNum;
		this._lastRotYNum = this.rotationY;
		this.UpdateLookRotations();
	}


	private void PlaySwitchSnd()
	{
		GameObject gameObject = GameObject.Find("/Menu/Main");
		if (gameObject != null)
		{
			gameObject.SendMessage("PlaySwitchSnd", SendMessageOptions.DontRequireReceiver);
		}
	}


	public void ResetMouseLook(Vector3 resetVec)
	{
		if (this.axes != MouseLook.RotationAxes.MouseXAndY)
		{
			if (this.axes == MouseLook.RotationAxes.MouseX)
			{
				this._startRotYNum = resetVec.y;
				this._rotXNum = 0f;
				base.transform.eulerAngles = resetVec;
				return;
			}
			if (this.axes == MouseLook.RotationAxes.MouseY)
			{
				this.rotationY = 0f;
				base.transform.localEulerAngles = Vector3.zero;
			}
		}
	}

	
	public void ResetMouseLook()
	{
		this._startRotYNum = base.transform.eulerAngles.y;
		this.rotationY = 0f;
	}

	
	public void SetControllable(bool controlBln)
	{
		this._activeBln = controlBln;
		if (controlBln)
		{
			base.StartCoroutine(this.NextFrameCheck());
		}
	}

	
	private IEnumerator NextFrameCheck()
	{
		float rotXNum = this._rotXNum;
		float rotYNum = this.rotationY;
		yield return new WaitForEndOfFrame();
		if (this._rotXNum != rotXNum || this.rotationY != rotYNum)
		{
			this.Update();
			this.UpdateLookRotations();
		}
		yield break;
	}

	
	private void Awake()
	{
	}

	
	private void Start()
	{
		this._isEditorBln = MouseLook._pcModeBln;
		if (!PlayerPrefs.HasKey("invertMouse"))
		{
			PlayerPrefs.SetInt("invertMouse", 0);
		}
		if (PlayerPrefs.GetInt("invertMouse") == 1)
		{
			this._mouseInvertBln = true;
		}
		this.UpdateUsingGamepad();
		if (base.transform.parent != null && base.transform.parent.name.IndexOf("Player") > -1)
		{
			string text = base.transform.parent.name.Replace("Player", string.Empty);
			int num = 1;
			if (text != string.Empty)
			{
				num = int.Parse(text);
			}
			this._playerNum = num;
			if (this._playerNum > 1)
			{
				if (!PlayerPrefs.HasKey("invertStick" + this._playerNum.ToString()))
				{
					PlayerPrefs.SetInt("invertStick" + this._playerNum.ToString(), 0);
				}
				if (PlayerPrefs.GetInt("invertStick" + this._playerNum.ToString()) == 1)
				{
					this._mouseInvertBln = true;
				}
			}
		}
		this._globSensitivityNum = 0.7682f;
		if (PlayerPrefs.HasKey("globSensitivity"))
		{
			this._globSensitivityNum = PlayerPrefs.GetFloat("globSensitivity");
		}
		if (PlayerPrefs.HasKey("mouseSmoothing"))
		{
			this._smoothingNum = PlayerPrefs.GetFloat("mouseSmoothing");
		}
		this._startRotYNum = base.transform.eulerAngles.y;
		if (base.rigidbody)
		{
			base.rigidbody.freezeRotation = true;
		}
	}

	
	public static Vector2 NaNSafeVector2(Vector2 vector, [Optional] Vector2 prevVector)
	{
		vector.x = ((!double.IsNaN((double)vector.x)) ? vector.x : prevVector.x);
		vector.y = ((!double.IsNaN((double)vector.y)) ? vector.y : prevVector.y);
		return vector;
	}

	
	private float PowerCurve(float value, float power)
	{
		return Mathf.Pow(Mathf.Abs(value), power) * Mathf.Sign(value);
	}

	
	private Vector2 CircularPowerCurve(Vector2 stickInput, float circularPower)
	{
		return stickInput.normalized * Mathf.Pow(stickInput.magnitude, circularPower);
	}

	
	private Vector2 GetAnalogLook()
	{
		Vector2 zero = Vector2.zero;
		zero.x = cInput.GetAxisRaw(this._lookXStr);
		zero.y = cInput.GetAxisRaw(this._lookYStr);
		float magnitude = zero.magnitude;
		if (magnitude < this.deadzoneRadial)
		{
			this.m_AnalogLookRawMove = Vector2.zero;
		}
		else
		{
			Vector2 vector = zero * (magnitude - this.deadzoneRadial) / ((1f - this.deadzoneRadial) * magnitude);
			this.m_AnalogLookRawMove = vector;
			float magnitude2 = this.m_AnalogLookRawMove.magnitude;
			float num = Mathf.Abs(this.m_AnalogLookRawMove.x);
			float num2 = Mathf.Abs(this.m_AnalogLookRawMove.y);
			float num3;
			float num4;
			if (!this.altBowtieMethod)
			{
				num3 = ((magnitude2 <= this.axialTransition) ? 0f : Mathf.Clamp01(this.deadzoneAxial * ((num2 - this.axialTransition) / (1f - this.axialTransition))));
				num4 = ((magnitude2 <= this.axialTransition) ? 0f : Mathf.Clamp01(this.deadzoneAxial * ((num - this.axialTransition) / (1f - this.axialTransition))));
			}
			else
			{
				num3 = Mathf.Lerp(0f, Mathf.Clamp01(this.deadzoneAxial * (num2 / 1f)), magnitude2);
				num4 = Mathf.Lerp(0f, Mathf.Clamp01(this.deadzoneAxial * (num / 1f)), magnitude2);
			}
			float num5 = ((num >= num3) ? (Mathf.Sign(this.m_AnalogLookRawMove.x) * ((num - num3) / (1f - num3))) : 0f);
			float num6 = ((num2 >= num4) ? (Mathf.Sign(this.m_AnalogLookRawMove.y) * ((num2 - num4) / (1f - num4))) : 0f);
			Vector2 vector2 = Vector2.zero;
			if (!this.skipBowtie)
			{
				vector2 = new Vector2(num5, num6);
			}
			else
			{
				vector2 = this.m_AnalogLookRawMove;
			}
			this.m_AnalogLookRawMove = vector2;
			this.m_AnalogLookRawMove = Vector2.ClampMagnitude(this.m_AnalogLookRawMove, 1f);
			this.m_AnalogLookRawMove = this.CircularPowerCurve(this.m_AnalogLookRawMove, this.AnalogLookPowerCurve);
		}
		this.m_CurrentAnalogLook = MouseLook.NaNSafeVector2(this.m_AnalogLookRawMove, default(Vector2));
		this.m_CurrentAnalogLook.x = this.m_CurrentAnalogLook.x * (this.aim_turnrate_yaw * this.AnalogLookSensitivity * Time.fixedDeltaTime);
		this.m_CurrentAnalogLook.y = this.m_CurrentAnalogLook.y * (this.aim_turnrate_pitch * this.AnalogLookSensitivity * Time.fixedDeltaTime);
		this.m_CurrentAnalogLook.y = ((!this._mouseInvertBln) ? (-this.m_CurrentAnalogLook.y) : this.m_CurrentAnalogLook.y);
		if (this.skipEverything)
		{
			this.m_CurrentAnalogLook = zero;
		}
		return this.m_CurrentAnalogLook;
	}

	
	public MouseLook.RotationAxes axes;

	
	public float sensitivityX = 15f;

	
	public float sensitivityY = 15f;

	
	public float minimumX = -360f;

	
	public float maximumX = 360f;

	
	public float minimumY = -60f;

	
	public float maximumY = 60f;

	
	[HideInInspector]
	public bool _mouseInvertBln;

	
	private float _globSensitivityNum = 0.7682f;

	
	private float _maxGlobSenseNum = 3f;

	
	private float _incSenseNum = 0.2f;

	
	private float _gamepadSenseNum = 115f;

	
	private float _smoothingNum = 0.5f;

	
	private float _smoothDivNum = 0.2f;

	
	private float _deltaCoefNum = 50f;

	
	[HideInInspector]
	public bool _activeBln = true;

	
	public float rotationY;

	
	public float _rotXNum;

	
	private float _startRotYNum;

	
	private float _deltaNum;

	
	[HideInInspector]
	public string _lookXStr = "LookStickX";

	
	[HideInInspector]
	public string _lookYStr = "LookStickY";

	
	private bool _isEditorBln;

	
	public static bool _pcModeBln = true;

	
	public bool altBowtieMethod;

	
	public bool skipBowtie;

	
	public bool skipEverything;


	public float AnalogLookSensitivity = 1f;

	
	public float AnalogLookPowerCurve = 2f;


	public float AnalogMovePowerCurve = 1.7f;


	public float deadzoneRadial = 0.03f;


	public float deadzoneAxial = 0.2f;


	public float axialTransition = 0.7f;


	private float aim_turnrate_pitch = 90f;


	private float aim_turnrate_pitch_ads = 55f;


	private float aim_turnrate_yaw = 260f;


	private float aim_turnrate_yaw_ads = 90f;


	protected Vector2 m_AnalogLookRawMove = Vector2.zero;


	protected Vector2 m_CurrentAnalogLook = Vector2.zero;


	private float _padLookXSensitivityNum = 0.308f;


	private float _padLookYSensitivityNum = 0.62f;


	public static bool _usingGamepadBln;


	private int _playerNum = 1;


	private Quaternion _globRot;


	private Quaternion _locRot;


	private bool _updateSetupBln;


	private bool _externallyResetBln;


	private bool _lookUpdatedBln;


	private float _lastRotXNum;


	private float _lastRotYNum;


	private float _storedRotXNum;


	private float _storedRotYNum;


	public enum RotationAxes
	{
		
		MouseXAndY,
		
		MouseX,
		
		MouseY
	}
}
