/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace extOSC.CUSTOM
{
	public class SimpleMessageReceiver : MonoBehaviour
	{
		#region Public Vars

		public string Address = "/xy1";

		[Header("OSC Settings")]
		public OSCReceiver Receiver;

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			Receiver.Bind(Address, ReceivedMessage);
		}

		#endregion

		#region Private Methods

		private void ReceivedMessage(OSCMessage message)
		{
			// Debug.LogFormat("Received: {0}", message);
			float x = (message.Values[0].FloatValue - 0.5f) * 2f;
			float y = (message.Values[1].FloatValue - 0.5f) * 2f;

			List<Vector2> positions = new List<Vector2>();
			for(int i = 0; i<(int)Random.Range(1f, 3f); i++) {
				positions.Add(new Vector2(x,y));
			}
 			PositionManager.Instance.SetPositions(positions);
		}

		#endregion
	}
}