using System;
using System.Timers;

public class StatblockModifier
{
	private Statblock Owner;
	public StatType StatModified { get; private set; }
	public int Amount { get; private set; }

	public StatblockModifier(Statblock owner, StatType statType, int amount, float modifierLength = -1)
	{
		Owner = owner;

		if (modifierLength > 0)
		{
			Timer t = new Timer(modifierLength);
			t.AutoReset = false;
			t.Elapsed += ModifierComplete;
			t.Start();
		}
		StatModified = statType;
		Amount = amount;
	}

	private void ModifierComplete(object sender, ElapsedEventArgs e)
	{
		Owner.RemoveModifier(this);
	}

}

public enum StatType
{
	HP,
	MOVESPEED,
	ACCURACY,
	DAMAGE,
	DEFENSE
}
