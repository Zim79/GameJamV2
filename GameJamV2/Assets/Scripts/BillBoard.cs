using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;

namespace Shooter.Jobsystem
{

	public class BillBoard : MonoBehaviour {

		public Material[] sprites;
		private Quaternion qua;
		public GameObject[] spr;
		private int num;
		private int tNum;
		private List<Transform>[] transforms;
		BillBoardJob billJob;
		JobHandle handle;
		private NativeArray<float> fl;
		private List<float>[] vs;
		private TransformAccessArray trans;

		void Start()
		{
			spr = GameObject.FindGameObjectsWithTag("Billboard");
			if (spr.Length > 50)
			{
				transforms = new List<Transform>[10];
				vs = new List<float>[10];
			}
			else
			{
				transforms = new List<Transform>[1];
				vs = new List<float>[1];
			}
			for (int i = 0; i < transforms.Length; i++)
			{
				transforms[i] = new List<Transform>();
				vs[i] = new List<float>();
			}
			for (int i = 0; i < spr.Length; i++)
			{
				if (spr.Length > 50)
				{
					transforms[Mathf.RoundToInt((i * 10) / spr.Length)].Add(spr[i].transform);
					if (spr[i].transform.parent.rotation != null)
					{
						vs[Mathf.RoundToInt((i * 10) / spr.Length)].Add(spr[i].transform.parent.rotation.eulerAngles.y);
					}
				}
				else
				{
					transforms[0].Add(spr[i].transform);
				}
				qua = Quaternion.LookRotation(spr[i].transform.parent.GetComponent<Renderer>().bounds.center - transform.position);
				spr[i].transform.rotation = Quaternion.Euler(new Vector3(0, qua.eulerAngles.y, 0));
				qua = Quaternion.Euler(qua.eulerAngles.x, qua.eulerAngles.y - spr[i].transform.parent.rotation.eulerAngles.y, qua.eulerAngles.z);
				spr[i].GetComponent<Renderer>().material = sprites[Mathf.Clamp(Mathf.RoundToInt((qua.eulerAngles.y / 360) * sprites.Length), 0, sprites.Length - 1)];
			}
			num = 0;
		}

		private void LateUpdate()
		{
			trans = new TransformAccessArray(transforms[num].Count, -1);
			trans.SetTransforms(transforms[num].ToArray());
			fl = new NativeArray<float>(vs[num].Count, Allocator.Temp);
			fl.CopyFrom(vs[num].ToArray());
			billJob = new BillBoardJob() {
				camPos = transform.position,
				spriteNum = new NativeArray<int>(spr.Length, Allocator.Temp),
				spriteLength = sprites.Length,
				rot = fl
			};
			handle = billJob.Schedule(trans);
			handle.Complete();
			for (int i = 0; i < trans.length; i++)
			{
				transforms[num][i].GetComponent<Renderer>().material = sprites[billJob.spriteNum[i]];
			}
			if (num < 9)
			{
				num++;
			}
			else
			{
				num = 0;
			}
			fl.Dispose();
			trans.Dispose();
			billJob.spriteNum.Dispose();
		}
	}
}
