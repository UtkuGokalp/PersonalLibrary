#nullable enable

namespace Utility.AI.FiniteStateMachine
{
    #region FiniteStateMachine
    public class FiniteStateMachine<TStateData>
    {
        #region Variables
        private State<TStateData> currentState;
        #endregion

        #region Constructor
        public FiniteStateMachine(State<TStateData> entryState)
        {
            currentState = entryState;
        }
        #endregion

        #region Tick
        /// <summary>
        /// Updates the FSM.
        /// </summary>
        public void Tick(TStateData stateData)
        {
            currentState.ExecuteState(stateData);

            Transition<TStateData>[] transitions = currentState.GetTransitions();
            foreach (Transition<TStateData> transition in transitions)
            {
                (bool conditionMet, State<TStateData>? targetState) = transition.IsTransitionConditionMet(stateData);
                if (conditionMet)
                {
                    transition.DoTransition(stateData);
                    if (targetState != null)
                    {
                        currentState = targetState;
                    }
                    break;
                }
            }
        }
        #endregion
    }
    #endregion

    #region State
    public abstract class State<TStateData>
    {
        #region ExecuteState
        /// <summary>
        /// Executes the AI when in this state.
        /// </summary>
        public abstract void ExecuteState(TStateData stateData);
        #endregion

        #region GetTransitions
        /// <summary>
        /// Returns the transitions that leads from this state other states.
        /// </summary>
        public abstract Transition<TStateData>[] GetTransitions();
        #endregion
    }
    #endregion

    #region Transition
    public abstract class Transition<TStateData>
    {
        #region IsTransitionConditionMet
        /// <summary>
        /// Checks if the transition should be done. Returns the relative data.
        /// </summary>
        public abstract (bool conditionMet, State<TStateData>? targetState) IsTransitionConditionMet(TStateData stateData);
        #endregion

        #region DoTransition
        /// <summary>
        /// Performs any work necessary to finish current state and prepares the FSM to the next state. Doesn't change the current state.
        /// </summary>
        public abstract void DoTransition(TStateData stateData);
        #endregion
    }
    #endregion
}
