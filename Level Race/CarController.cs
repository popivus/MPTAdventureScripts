using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	private float m_horizontalInput;
	private float m_verticalInput;
	private float m_steeringAngle;

	[SerializeField] private WheelCollider frontDriverWheel, frontPassengerWheel;
	[SerializeField] private WheelCollider rearDriverWheel, rearPassengerWheel;
	[SerializeField] private Transform frontDriverWheelModel, frontPassengerWheelModel;
	[SerializeField] private Transform rearDriverWheelModel, rearPassengerWheelModel;
	[SerializeField] private float maxSteerAngle = 30;
	[SerializeField] private float motorForce = 50;

    
    public void GetInput()
	{
		m_horizontalInput = Input.GetAxis("Horizontal");
		m_verticalInput = Input.GetAxis("Vertical");
	}

	private void Steer()
	{
		m_steeringAngle = maxSteerAngle * m_horizontalInput;
		frontDriverWheel.steerAngle = m_steeringAngle;
		frontPassengerWheel.steerAngle = m_steeringAngle;
	}

	private void Accelerate()
	{
		frontDriverWheel.motorTorque = m_verticalInput * motorForce;
		frontPassengerWheel.motorTorque = m_verticalInput * motorForce;
	}

	private void UpdateWheelPoses()
	{
		UpdateWheelPose(frontDriverWheel, frontDriverWheelModel);
		UpdateWheelPose(frontPassengerWheel, frontPassengerWheelModel);
		UpdateWheelPose(rearDriverWheel, rearDriverWheelModel);
		UpdateWheelPose(rearPassengerWheel, rearPassengerWheelModel);
	}

	private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
	{
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}

	private void FixedUpdate()
	{
        AudioManager.instance.CarSound(m_verticalInput != 0 ? true : false);
        if (UIController.instance.isPaused) return;
		GetInput();
		Steer();
		Accelerate();
		UpdateWheelPoses();
	}

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
