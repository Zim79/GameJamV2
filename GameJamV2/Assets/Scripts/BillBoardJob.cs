using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;

namespace Shooter.Jobsystem
{
	public struct BillBoardJob : IJobParallelForTransform {

		public Vector3 camPos;
		public NativeArray<int> spriteNum;
		public NativeArray<float> rot;
		public int spriteLength;
		public void Execute (int index, TransformAccess transform)
		{
			Quaternion qua = Quaternion.LookRotation(transform.position - camPos);
			transform.rotation = qua;
			qua = Quaternion.Euler(qua.eulerAngles.x, qua.eulerAngles.y - rot[index], qua.eulerAngles.z);
			spriteNum[index] = Mathf.Clamp(Mathf.RoundToInt((qua.eulerAngles.y / 360) * spriteLength), 0, spriteLength - 1);
		}
	}

}
