using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;

public class ConnectionQueue
{
	private class ReservedServerSlot : IPooled
	{
		public ulong UserId;

		public float Expiry;

		public void EnterPool()
		{
		}

		public void LeavePool()
		{
			UserId = 0uL;
			Expiry = 0f;
		}
	}

	private List<Connection> queue = new List<Connection>();

	private List<Connection> joining = new List<Connection>();

	private List<ReservedServerSlot> reservedSlots = new List<ReservedServerSlot>();

	private float nextCleanupReservedSlots;

	private float nextMessageTime;

	public int ReservedCount => reservedSlots.Count;

	public int Queued => queue.Count;

	public int Joining => joining.Count;

	public void SkipQueue(ulong userid)
	{
		for (int i = 0; i < queue.Count; i++)
		{
			Connection val = queue[i];
			if (val.userid == userid)
			{
				JoinGame(val);
				break;
			}
		}
	}

	internal void Join(Connection connection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		connection.state = (State)2;
		queue.Add(connection);
		nextMessageTime = 0f;
		if (CanJumpQueue(connection))
		{
			JoinGame(connection);
		}
	}

	public void Cycle(int availableSlots)
	{
		if (queue.Count != 0)
		{
			if (availableSlots - Joining > 0)
			{
				JoinGame(queue[0]);
			}
			if (Time.realtimeSinceStartup > nextCleanupReservedSlots)
			{
				nextCleanupReservedSlots = Time.realtimeSinceStartup + 1f;
				CleanupExpiredReservedSlots();
			}
			SendMessages();
		}
	}

	private void SendMessages()
	{
		if (!(nextMessageTime > Time.realtimeSinceStartup))
		{
			nextMessageTime = Time.realtimeSinceStartup + 10f;
			for (int i = 0; i < queue.Count; i++)
			{
				SendMessage(queue[i], i);
			}
		}
	}

	private void SendMessage(Connection c, int position)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		empty = ((position <= 0) ? string.Format("YOU'RE NEXT - {1:N0} PLAYERS BEHIND YOU", position, queue.Count - position - 1) : $"{position:N0} PLAYERS AHEAD OF YOU, {queue.Count - position - 1:N0} PLAYERS BEHIND");
		NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
		obj.PacketID((Type)16);
		obj.String("QUEUE", false);
		obj.String(empty, false);
		obj.Send(new SendInfo(c));
	}

	public void RemoveConnection(Connection connection)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		if (queue.Remove(connection))
		{
			nextMessageTime = 0f;
		}
		joining.Remove(connection);
		if ((int)connection.state == 4 || (int)connection.state == 3)
		{
			AddReservation(connection.userid);
		}
	}

	private void JoinGame(Connection connection)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		queue.Remove(connection);
		connection.state = (State)3;
		RemoveReservedSlot(connection.userid);
		nextMessageTime = 0f;
		joining.Add(connection);
		SingletonComponent<ServerMgr>.Instance.JoinGame(connection);
	}

	public void JoinedGame(Connection connection)
	{
		RemoveConnection(connection);
	}

	private void AddReservation(ulong userId)
	{
		ReservedServerSlot reservedServerSlot = FindQueueSpot(userId);
		if (reservedServerSlot == null)
		{
			reservedServerSlot = Pool.Get<ReservedServerSlot>();
			reservedSlots.Add(reservedServerSlot);
		}
		reservedServerSlot.UserId = userId;
		reservedServerSlot.Expiry = Time.realtimeSinceStartup + (float)Server.rejoin_delay;
	}

	private void CleanupExpiredReservedSlots()
	{
		if (reservedSlots.Count == 0)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		for (int i = 0; i < reservedSlots.Count; i++)
		{
			ReservedServerSlot reservedServerSlot = reservedSlots[i];
			if (realtimeSinceStartup > reservedServerSlot.Expiry)
			{
				reservedSlots.RemoveAt(i);
				i--;
				Pool.Free<ReservedServerSlot>(ref reservedServerSlot);
			}
		}
	}

	private ReservedServerSlot FindQueueSpot(ulong userId)
	{
		foreach (ReservedServerSlot reservedSlot in reservedSlots)
		{
			if (reservedSlot.UserId == userId)
			{
				return reservedSlot;
			}
		}
		return null;
	}

	private void RemoveReservedSlot(ulong userId)
	{
		for (int i = 0; i < reservedSlots.Count; i++)
		{
			ReservedServerSlot reservedServerSlot = reservedSlots[i];
			if (reservedServerSlot.UserId == userId)
			{
				reservedSlots.RemoveAt(i);
				i--;
				Pool.Free<ReservedServerSlot>(ref reservedServerSlot);
			}
		}
	}

	private bool CanJumpQueue(Connection connection)
	{
		if (DeveloperList.Contains(connection.userid))
		{
			return true;
		}
		ServerUsers.User user = ServerUsers.Get(connection.userid);
		if (user != null && user.group == ServerUsers.UserGroup.Moderator)
		{
			return true;
		}
		if (user != null && user.group == ServerUsers.UserGroup.Owner)
		{
			return true;
		}
		if (user != null && user.group == ServerUsers.UserGroup.SkipQueue)
		{
			return true;
		}
		for (int i = 0; i < reservedSlots.Count; i++)
		{
			if (reservedSlots[i].UserId == connection.userid)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsQueued(ulong userid)
	{
		for (int i = 0; i < queue.Count; i++)
		{
			if (queue[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsJoining(ulong userid)
	{
		for (int i = 0; i < joining.Count; i++)
		{
			if (joining[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}
}
