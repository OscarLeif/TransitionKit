using System.Collections;

namespace AtaGames.TransitionKit
{
    public interface ITransition
    {
        IEnumerator YieldTransition();
    }

    public enum TransitionState
    {
        StateIn, LoadScene, Hold, StateOut
    }

    public enum LoadState
    {
        Begin, Loading, Completed
    }
}
