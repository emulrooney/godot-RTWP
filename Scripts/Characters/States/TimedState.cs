using Godot;

public class TimedState : CharacterState
{
	private Timer timer;
	
	public override void _Ready()
	{
		timer = (Timer)GetNode("Timer");
		timer.WaitTime = StateLength;
	}

	public override void OnStart()
	{
		Done = false;
		timer.WaitTime = StateLength;
		timer.Start();
	}

	public override void OnFinish()
	{
		NextAnimation();
	}

    public void OverrideStateLength(float newStateLength)
    {
        if (newStateLength <= 0)
            Timeout(); //Short circuit.
        else
        {
            timer.Stop();
            timer.WaitTime = newStateLength;
            timer.Start();
        }

    }

	public void Timeout()
	{
		Done = true;
		timer.WaitTime = StateLength;        
	}

}
