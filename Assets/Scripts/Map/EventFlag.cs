using System.Collections.Generic;

[System.Serializable]
public class EventFlags<T> {

	private class FlagTuple {
		public T value;
		public bool activated;
	}

	private List<FlagTuple> flags = new List<FlagTuple>();
	public int Count {get => flags.Count; }


	public void Reset() {
		flags.Clear();
	}

	public void AddFlag(T flag) {
		flags.Add(new FlagTuple() { value = flag });
	}

	public bool IsActivated(int index) {
		return flags[index].activated;
	}

	public T GetEvent(int index) {
		return flags[index].value;
	}

	public void Activate(int index) {
		flags[index].activated = true;
	}
}
