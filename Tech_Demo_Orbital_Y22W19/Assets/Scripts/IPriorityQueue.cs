public interface IPriorityQueue<T>
{
    public abstract T Peek();

    public abstract T Extract();

    public abstract void DecreaseKey(T element);

    public abstract void IncreaseKey(T element);

    public abstract void Add(T element);

    public abstract bool Contains(T element);

    public abstract bool IsEmpty();

    public abstract void Build(T[] elements);

    public abstract void Clear();

    public abstract int Count();

    public abstract void Remove(T element);
}