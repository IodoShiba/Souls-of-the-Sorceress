namespace Buffs
{
    public interface IBuffAffector
    {
        BuffTypeID buffTypeID { get; }

        void Affect(BuffFunctor destination);
    }

}
