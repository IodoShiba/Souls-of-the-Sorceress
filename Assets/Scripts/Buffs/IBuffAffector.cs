namespace Buffs
{
    public interface IBuffAffector
    {
        BuffTypeID buffTypeID { get; }

        void Affect(BuffDestination destination);
    }

}
