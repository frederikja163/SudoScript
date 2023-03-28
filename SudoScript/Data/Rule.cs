namespace SudoScript.Data;

public interface IRule
{
    public abstract void EliminateCandidates(Unit unit);
    public abstract bool ValidateRules(Unit unit);
}
