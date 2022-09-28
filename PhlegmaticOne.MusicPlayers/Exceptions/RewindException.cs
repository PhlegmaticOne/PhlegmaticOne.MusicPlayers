namespace PhlegmaticOne.MusicPlayers.Exceptions;

public class RewindException : Exception
{
	public RewindException(TimeSpan rewindRequestedTimeSpan, TimeSpan actualTime, string message)
	{
		RewindRequestedTimeSpan = rewindRequestedTimeSpan;
		ActualTime = actualTime;
		Message = message;
	}

	public TimeSpan RewindRequestedTimeSpan { get; }
	public TimeSpan ActualTime { get; }
	public override string Message { get; }
}
